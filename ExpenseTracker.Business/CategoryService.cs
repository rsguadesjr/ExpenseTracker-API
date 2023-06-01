using AutoMapper;
using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Category;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly CurrentUserDetails _currentUser;

        public CategoryService(IRepository<Category> categoryRepository,
                                IUnitOfWork unitOfWork,
                                IUserRepository userRepository,
                                IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;

            _currentUser = _userRepository.GetCurrentUser();
        }

        public async Task<List<CategoryResponseModel>> GetAll()
        {  
            // Get current user
            var currentUser = _userRepository.GetCurrentUser();
            if (currentUser == null)
            {
                throw new ApplicationException("User not found");
            }

            var result = _categoryRepository.GetAll<CategoryResponseModel>(x => x.UserId == currentUser.UserId);
            return await result.ToListAsync();
        }

        public async Task<CategoryResponseModel> Get(int id)
        {
            return await _categoryRepository.Get<CategoryResponseModel>(x => x.Id == id);
        }
        public async Task<CategoryResponseModel> Create(CategoryRequestModel data)
        {

            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {

                    var category = _mapper.Map<Category>(data);
                    category.UserId = _currentUser.UserId;

                    var created = await _unitOfWork.CategoryRepository.Create(category);
                    
                    await _unitOfWork.SaveChangesAsync();
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

        Task ICategoryService.Delete(int id)
        {
            throw new NotImplementedException();
        }

        Task ICategoryService.Sort()
        {
            throw new NotImplementedException();
        }

        public async Task<CategoryResponseModel> Update(CategoryRequestModel data)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {

                    var category = _mapper.Map<Category>(data);
                    category.UserId = _currentUser.UserId;

                    var created = await _unitOfWork.CategoryRepository.Update(category);
                    data.Id = created.Id;


                    await _unitOfWork.SaveChangesAsync();
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
    }
}
