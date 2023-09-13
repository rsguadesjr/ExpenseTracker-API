using AutoMapper;
using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Expense;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly CurrentUserDetails _currentUser;
        public ExpenseService(IUnitOfWork unitOfWork,
                              IUserRepository userRepository,
                              IMapper mapper,
                              IRepository<Expense> expenseRepository,
                              IRepository<ExpenseTag> expenseTagRepository,
                              IRepository<Tag> tagRepository,
                              IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
            _expenseRepository = expenseRepository;
            _expenseTagRepository = expenseTagRepository;
            _tagRepository = tagRepository;
            _configuration = configuration;
            _currentUser = _userRepository.GetCurrentUser();
        }

        public async Task<ExpenseRequestModel> Get(Guid id)
        {
            return await _expenseRepository.Get<ExpenseRequestModel>(x => x.Id == id);
        }

        // TODO: Correct the UserId value
        public async Task<PaginatedList<ExpenseResponseModel>> GetAll(BaseSearchParameter searchParam)
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
            var query = _expenseRepository.GetAll<ExpenseResponseModel>(predicate);
            query = query.OrderByDescending(x => x.ExpenseDate).ThenByDescending(x => x.ModifiedDate).ThenByDescending(x => x.CreatedDate);
            var totalRows = query.Count();

            if (searchParam.PageNumber != null && searchParam.TotalRows != null)
            {
                var take = searchParam.TotalRows > 0 ? searchParam.TotalRows.Value : 0;
                var skip = searchParam.PageNumber > 0 ? (searchParam.PageNumber.Value * take) : 0;
                query = query.Skip(skip);
                query = query.Take(take);
            }

            var data = await query.ToListAsync();


            return new PaginatedList<ExpenseResponseModel>
            {
                CurrentPage = searchParam.PageNumber,
                TotalRows = totalRows,
                Data = data
            };
        }

        public async Task<ExpenseResponseModel> Create(ExpenseRequestModel dto)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await RestrictedUserCheck();
                    
                    var transactionDate = DateTime.UtcNow;

                    var expense = _mapper.Map<Expense>(dto);
                    expense.UserId= _currentUser.UserId;
                    
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

            return await _expenseRepository.Get<ExpenseResponseModel>(x => x.Id == dto.Id);
        }

        public async Task<ExpenseResponseModel> Update(ExpenseRequestModel dto)
        {
            if (dto?.Id == null)
                throw new ApplicationException("Invalid request model");

            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await RestrictedUserCheck();

                    var transactionDate = DateTime.UtcNow;
                    var expense = _mapper.Map<Expense>(dto);

                    var propertiesToUpdate = new List<string>
                    {
                        nameof(Expense.Amount),
                        nameof(Expense.ExpenseDate),
                        nameof(Expense.Description),
                        nameof(Expense.CategoryId),
                        nameof(Expense.SourceId)
                    };
                    await _expenseRepository.Update(expense.Id, expense, propertiesToUpdate);
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
            

            return await _expenseRepository.Get<ExpenseResponseModel>(x => x.Id == dto.Id);
        }

        public async Task Delete(Guid id)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await RestrictedUserCheck();

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

        // check for restricted users / test users
        private async Task RestrictedUserCheck()
        {
            List<string> restrictedUsers = _configuration.GetSection("Restrictions:UserEmails").Get<List<string>>() ?? new List<string>();
            if (restrictedUsers.Contains(_currentUser.Email))
            {
                var dailyTransactionLimit = _configuration.GetSection("Restrictions:DailyTransactionLimit").Get<int>();
                var totalTransactions = await _expenseRepository.GetAll(x => x.UserId == _currentUser.UserId && x.CreatedDate >= DateTime.Today).CountAsync();
                if (totalTransactions >= dailyTransactionLimit)
                {
                    throw new ApplicationException("User has reached the daily transaction limit");
                }
            }
        }
    }
}
