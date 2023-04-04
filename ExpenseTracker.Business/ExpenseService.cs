using AutoMapper;
using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseTracker.Business
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IUserService _userService;
        public ExpenseService(IUnitOfWork unitOfWork,
                            IRepository<User> userRepository,
                                    IMapper mapper,
                                    IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<ExpenseDto> Get(Guid id)
        {
            return await _unitOfWork.ExpenseRepository.Get<ExpenseDto>(x => x.Id == id);
        }

        // TODO: Correct the UserId value
        public async Task<IEnumerable<ExpenseListResult>> GetAll(BaseSearchParameter searchParam)
        {
            Expression< Func<Expense, bool>> predicate = v => v.UserId != null
                    && (searchParam.DateFrom == null || (searchParam.DateFrom <= v.ExpenseDate))
                    && (searchParam.DateTo == null || (v.ExpenseDate <= searchParam.DateTo))
                    && (searchParam.CategoryId == null || searchParam.CategoryId == v.CategoryId)
                    && (searchParam.SourceId == null || searchParam.SourceId == v.SourceId);

            // query
            var query = _unitOfWork.ExpenseRepository.GetAll<ExpenseListResult>(predicate);
            if (searchParam.PageNumber != null)
            {
                query = query.Skip(searchParam.PageNumber.Value);
            }
            if (searchParam.TotalRows != null)
            {
                query = query.Take(searchParam.TotalRows.Value);
            }

            query = query.OrderByDescending(x => x.ExpenseDate);
            return await query.ToListAsync();
        }

        public async Task<ExpenseListResult> Create(ExpenseDto dto)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;



                    // TODO: need to update this part
                    var userId = Guid.Parse("1bd17662-e014-43d4-b380-097acd2c2ae6");
                    var user = await _userRepository.Get<UserVM>(x => x.Id == userId);


                    var expense = _mapper.Map<Expense>(dto);
                    expense.UserId= userId;
                    var result = await _unitOfWork.ExpenseRepository.Create(expense);
                    dto.Id = result.Id;
                
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

        }
    }
}
