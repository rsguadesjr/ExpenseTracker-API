﻿using AutoMapper;
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
                    var transactionDate = DateTime.UtcNow;
                    var user = _userRepository.GetCurrentUser();

                    var expense = _mapper.Map<Expense>(dto);
                    expense.UserId= user.UserId;

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
