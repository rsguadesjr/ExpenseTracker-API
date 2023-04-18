using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task<AuthRequestResult> Login(string token)
        {
            var result = new AuthRequestResult();

            try
            {
                var firebaseToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);

                // TODO: remove initialization here, other value will be in the else statement below
                Guid userId = Guid.NewGuid();

                var existingUser = await _userRepository.Get<UserVM>(x => x.UniqueId == firebaseToken.Uid);
                if (existingUser != null)
                {
                    result.NeedToCompleteProfile = false;
                    result.IsNewUser = false;
                    userId = existingUser.Id;
                }
                else
                {
                    // TODO: add flag to check if new users will be auto registered
                    // TODO: implement 
                }

                // create custom claims here
                IReadOnlyDictionary<string, object> test = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>
                {
                    { "userId", userId }
                });
                await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).SetCustomUserClaimsAsync(firebaseToken.Uid, test);
                var customToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).CreateCustomTokenAsync(firebaseToken.Uid);

                result.Token = customToken;
                result.IsAuthorized = true;
            }
            catch (Exception ex)
            {
                result.IsAuthorized = false;

            }

            return result;
        }

    }
}
