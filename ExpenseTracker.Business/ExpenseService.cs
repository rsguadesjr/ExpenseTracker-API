using AutoMapper;
using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Expense;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseTracker.Business
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public ExpenseService(IUnitOfWork unitOfWork,
                            IUserRepository userRepository,
                                    IMapper mapper,
                                    IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<ExpenseDto> Get(Guid id)
        {
            return await _unitOfWork.ExpenseRepository.Get<ExpenseDto>(x => x.Id == id);
        }

        // TODO: Correct the UserId value
        public async Task<PaginatedList<ExpenseListResult>> GetAll(BaseSearchParameter searchParam)
        {
            Expression< Func<Expense, bool>> predicate = v => v.UserId != null
                    && (searchParam.DateFrom == null || (searchParam.DateFrom <= v.ExpenseDate))
                    && (searchParam.DateTo == null || (v.ExpenseDate <= searchParam.DateTo))
                    && (searchParam.CategoryId == null || searchParam.CategoryId == v.CategoryId)
                    && (searchParam.SourceId == null || searchParam.SourceId == v.SourceId);

            // query
            var query = _unitOfWork.ExpenseRepository.GetAll<ExpenseListResult>(predicate);
            query = query.OrderByDescending(x => x.ExpenseDate).ThenBy(x => x.CreatedDate);
            var totalRows = query.Count();

            if (searchParam.PageNumber != null && searchParam.TotalRows != null)
            {
                var take = searchParam.TotalRows > 0 ? searchParam.TotalRows.Value : 0;
                var skip = searchParam.PageNumber > 0 ? (searchParam.PageNumber.Value * take) : 0;
                query = query.Skip(skip);
                query = query.Take(take);
            }

            var data = await query.ToListAsync();


            return new PaginatedList<ExpenseListResult>
            {
                CurrentPage = searchParam.PageNumber,
                TotalRows = totalRows,
                Data = data
            };
        }

        public async Task<ExpenseListResult> Create(ExpenseDto dto)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // get current user
                    var transactionDate = DateTime.UtcNow;
                    var user = _userRepository.GetCurrentUser();

                    // map user to the expense entry
                    var expense = _mapper.Map<Expense>(dto);
                    expense.UserId= user.UserId;
                    
                    // create expense entry
                    var result = await _unitOfWork.ExpenseRepository.Create(expense);
                    dto.Id = result.Id;


                    await AddOrDeleteTags(dto.Id.Value, dto.Tags);

                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw ex;

                }
            }

            return await _unitOfWork.ExpenseRepository.Get<ExpenseListResult>(x => x.Id == dto.Id);
        }

        public async Task<ExpenseListResult> Update(ExpenseDto dto)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;

                    var result = await _unitOfWork.ExpenseRepository.Update(_mapper.Map<Expense>(dto));
                    dto.Id = result.Id;


                    await AddOrDeleteTags(dto.Id.Value, dto.Tags);

                    //// get existing tags
                    //var existingTags = await _unitOfWork.ExpenseTagRepository.GetAll(x => x.ExpenseId == dto.Id)
                    //                                                        .Include(x => x.Tag)
                    //                                                        .ToListAsync();
                    //// remove existing tags if not in current list
                    //var toRemoveTags = existingTags.Where(x => !dto.Tags.Contains(x.Tag.Name)).ToList();
                    //if (toRemoveTags.Any())
                    //{
                    //    await _unitOfWork.ExpenseTagRepository.Delete(toRemoveTags);
                    //}

                    //// add new tags if not yet in db
                    //var existingTagNames = existingTags.Select(x => x.Tag.Name);
                    //var toAddTags = dto.Tags.Where(x => !existingTagNames.Contains(x)).ToList();
                    //foreach (var tag in toAddTags)
                    //{
                    //    var expenseTag = new ExpenseTag { ExpenseId = dto.Id.Value };
                    //    // get tagId
                    //    int? tagId = (await _unitOfWork.TagRepository.Get(x => x.Name == tag))?.Id;
                    //    if (tagId != null)
                    //    {
                    //        expenseTag.TagId = tagId.Value;
                    //    }
                    //    else
                    //    {
                    //        expenseTag.Tag = new Tag { Name = tag };
                    //    }

                    //    // add expense tag relationship
                    //    //var expenseTag = new ExpenseTag { TagId = tagId.Value, ExpenseId = dto.Id.Value };
                    //    await _unitOfWork.ExpenseTagRepository.Create(expenseTag);
                    //}
                    ////toAddTags.ForEach(async tag =>
                    ////{
                    ////    // get tagId
                    ////    int? tagId = (await _unitOfWork.TagRepository.Get(x => x.Name == tag))?.Id;
                    ////    if (tagId == null)
                    ////    {
                    ////        tagId = (await _unitOfWork.TagRepository.Create(new Tag { Name = tag })).Id;
                    ////    }

                    ////    // add expense tag relationship
                    ////    var expenseTag = new ExpenseTag { TagId = tagId.Value, ExpenseId = dto.Id.Value };
                    ////    await _unitOfWork.ExpenseTagRepository.Create(expenseTag);

                    ////});

                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            

            return await _unitOfWork.ExpenseRepository.Get<ExpenseListResult>(x => x.Id == dto.Id);
        }

        public async Task Delete(Guid id)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.ExpenseRepository.Delete(id);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
        }


        private async Task AddOrDeleteTags(Guid expenseId, List<string> tags)
        {
            // get existing tags
            var existingTags = await _unitOfWork.ExpenseTagRepository.GetAll(x => x.ExpenseId == expenseId)
                                                                    .Include(x => x.Tag)
                                                                    .ToListAsync();
            // remove existing tags if not in current list
            var toRemoveTags = existingTags.Where(x => !tags.Contains(x.Tag.Name)).ToList();
            if (toRemoveTags.Any())
            {
                await _unitOfWork.ExpenseTagRepository.Delete(toRemoveTags);
            }

            // add new tags if not yet in db
            var existingTagNames = existingTags.Select(x => x.Tag.Name);
            var toAddTags = tags.Where(x => !existingTagNames.Contains(x) && !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim())
                                .Distinct()
                                .ToList();
            foreach (var tag in toAddTags)
            {
                var expenseTag = new ExpenseTag { ExpenseId = expenseId };
                // get tagId
                int? tagId = (await _unitOfWork.TagRepository.Get(x => x.Name == tag))?.Id;
                if (tagId != null)
                {
                    expenseTag.TagId = tagId.Value;
                }
                else
                {
                    expenseTag.Tag = new Tag { Name = tag };
                }

                // add expense tag relationship
                await _unitOfWork.ExpenseTagRepository.Create(expenseTag);
            }
        }
    }
}
