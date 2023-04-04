using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models;
using ExpenseTracker.Repository.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        public UserService(IHttpContextAccessor httpContextAccessor,
                            IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public Task<UserVM> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserVM> GetByEmail(string email)
        {
            return await _userRepository.Get<UserVM>(x => x.Email == email);
        }

        public async Task<UserRegistrationResponse> Register()
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var isEmailVerified = bool.TryParse(claims.FirstOrDefault(x => x.Type == "email_verified")?.Value, out bool result);
            var uniqueId = claims.FirstOrDefault(x => x.Type == "user_id")?.Value;


            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(uniqueId))
            {
                return new UserRegistrationResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Invalid Token" }
                };
            }

            var emailVerificationLink = string.Empty;
            if (!isEmailVerified)
            {
                emailVerificationLink = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).GenerateEmailVerificationLinkAsync(email);
            }


            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                UniqueId= uniqueId,
                DisplayName = name ?? string.Empty
            };

            await _userRepository.Create(user);
            await _userRepository.SaveChanges();

            return new UserRegistrationResponse
            {
                IsSuccess = true,
                EmailVerificationLink = emailVerificationLink
            };
        }

        public async Task Login()
        {
        }

    }
}
