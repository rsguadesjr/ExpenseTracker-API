using AutoMapper;
using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Budget;
using ExpenseTracker.Model.Models.Category;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExpenseTracker.Business
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Budget> _budgetRepository;
        private readonly IRepository<BudgetCategory> _budgetCategoryRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly CurrentUserDetails _currentUser;

        public BudgetService(IUnitOfWork unitOfWork,
                            IRepository<Budget> budgetRepository,
                            IRepository<BudgetCategory> budgetCategoryRepository,
                            IUserRepository userRepository,
                            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _budgetRepository = budgetRepository;
            _budgetCategoryRepository = budgetCategoryRepository;
            _userRepository = userRepository;
            _mapper = mapper;

            _currentUser = _userRepository.GetCurrentUser();
        }


        public async Task<List<BudgetResponseModel>> GetAll()
        {
            // Get current user
            var currentUser = _userRepository.GetCurrentUser();
            if (currentUser == null)
            {
                throw new ApplicationException("User not found");
            }

            var result = _budgetRepository.GetAll<BudgetResponseModel>(x => x.UserId == currentUser.UserId);
            return await result.ToListAsync();
        }

        public async Task<BudgetResponseModel> Get(int id)
        {
            return await _budgetRepository.Get<BudgetResponseModel>(x => x.Id == id);
        }
        public async Task<BudgetResponseModel> Create(BudgetRequestModel data)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;

                    var budget = _mapper.Map<Budget>(data);
                    budget.UserId = _currentUser.UserId;
                    foreach(var bc in budget.BudgetCategories)
                    {
                        bc.UserId = _currentUser.UserId;
                    }

                    var created = await _unitOfWork.BudgetRepository.Create(budget);

                    //foreach (var budgetCatgory in data.BudgetCategories)
                    //{
                    //    var item = _mapper.Map<BudgetCategory>(budgetCatgory);
                    //    await _unitOfWork.BudgetCategoryRepository.Create(item);
                    //}


                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();

                    data.Id = created.Id;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    // Log error
                    throw ex;
                }
            }

            return await Get(data.Id.Value);
        }

        public async Task<BudgetResponseModel> Update(BudgetRequestModel data)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;
                    var budget = _mapper.Map<Budget>(data);

                    var updatedBudget = await _unitOfWork.BudgetRepository.Update(budget);

                    var existingBudgetCategories = await _budgetCategoryRepository.GetAll(x => x.UserId == _currentUser.UserId && x.BudgetId == budget.Id).ToListAsync();
                    
                    // Delete
                    var currentCategoryIds = data.BudgetCategories.Select(bc => bc.CategoryId).ToList();
                    var removeCategories = existingBudgetCategories.Where(x => !currentCategoryIds.Contains(x.CategoryId));
                    if (removeCategories.Any())
                    {
                        await _budgetCategoryRepository.Delete(removeCategories);
                    }

                    // Create or Update
                    foreach (var budgetCatgory in data.BudgetCategories)
                    {
                        var item = _mapper.Map<BudgetCategory>(budgetCatgory);

                        // if in existing list, just update
                        var existingBudgetCategory = existingBudgetCategories.FirstOrDefault(x => x.CategoryId == budgetCatgory.CategoryId);
                        if (existingBudgetCategory != null)
                        {
                            item.Id = existingBudgetCategory.Id;
                            await _budgetCategoryRepository.Update(item);
                        }
                        // else create the entry
                        else
                        {
                            await _budgetCategoryRepository.Create(item);
                        }
                    }


                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    // Log error
                    throw ex;
                }
            }

            return await Get(data.Id.Value);
        }

        public async Task Delete(int id)
        {
            await _budgetRepository.Delete(id);
            await _budgetRepository.SaveChanges();
        }
    }
}
