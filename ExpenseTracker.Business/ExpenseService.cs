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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Expense> _expenseRepository;
        private readonly IRepository<ExpenseTag> _expenseTagRepository;
        private readonly IRepository<Tag> _tagRepository;
        public ExpenseService(IUnitOfWork unitOfWork,
                              IUserRepository userRepository,
                              IMapper mapper,
                              IRepository<Expense> expenseRepository,
                              IRepository<ExpenseTag> expenseTagRepository,
                              IRepository<Tag> tagRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
            _expenseRepository = expenseRepository;
            _expenseTagRepository = expenseTagRepository;
            _tagRepository = tagRepository;
        }

        public async Task<ExpenseDto> Get(Guid id)
        {
            return await _expenseRepository.Get<ExpenseDto>(x => x.Id == id);
        }

        // TODO: Correct the UserId value
        public async Task<PaginatedList<ExpenseListResult>> GetAll(BaseSearchParameter searchParam)
        {
            // Get current user
            var currentUser = _userRepository.GetCurrentUser();
            if (currentUser == null)
            {
                throw new ApplicationException("User not found");
            }

            Expression<Func<Expense, bool>> predicate = v => v.UserId != null
                    && (searchParam.DateFrom == null || (searchParam.DateFrom <= v.ExpenseDate))
                    && (searchParam.DateTo == null || (v.ExpenseDate <= searchParam.DateTo))
                    && (searchParam.CategoryId == null || searchParam.CategoryId == v.CategoryId)
                    && (searchParam.SourceId == null || searchParam.SourceId == v.SourceId)
                    && (currentUser.UserId == v.UserId);

            // query
            var query = _expenseRepository.GetAll<ExpenseListResult>(predicate);
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
                    var result = await _expenseRepository.Create(expense);
                    dto.Id = result.Id;

                    await AddOrDeleteTags(dto.Id.Value, dto.Tags);

                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }

            return await _expenseRepository.Get<ExpenseListResult>(x => x.Id == dto.Id);
        }

        public async Task<ExpenseListResult> Update(ExpenseDto dto)
        {
            if (dto?.Id == null)
                throw new ApplicationException("Invalid request model");

            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;
                    var expense = _mapper.Map<Expense>(dto);

                    var props = new List<string>
                    {
                        nameof(Expense.Amount),
                        nameof(Expense.ExpenseDate),
                        nameof(Expense.Description),
                        nameof(Expense.CategoryId),
                        nameof(Expense.SourceId)
                    };
                    await _expenseRepository.Update(expense.Id, expense, props);
                    await AddOrDeleteTags(dto.Id.Value, dto.Tags);

                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            

            return await _expenseRepository.Get<ExpenseListResult>(x => x.Id == dto.Id);
        }

        public async Task Delete(Guid id)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _expenseRepository.Delete(id);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }


        private async Task AddOrDeleteTags(Guid expenseId, List<string> tags)
        {
            // get existing tags
            var existingTags = await _expenseTagRepository.GetAll(x => x.ExpenseId == expenseId)
                                                                    .Include(x => x.Tag)
                                                                    .ToListAsync();
            // remove existing tags if not in current list
            var toRemoveTags = existingTags.Where(x => !tags.Contains(x.Tag.Name)).ToList();
            foreach(var tag in toRemoveTags)
            {
                _expenseTagRepository.Delete(tag);
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
                int? tagId = (await _tagRepository.Get(x => x.Name == tag))?.Id;
                if (tagId != null)
                {
                    expenseTag.TagId = tagId.Value;
                }
                else
                {
                    expenseTag.Tag = new Tag { Name = tag };
                }

                // add expense tag relationship
                await _expenseTagRepository.Create(expenseTag);
            }
        }
    }
}
