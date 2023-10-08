using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CurrentUserDetails _currentUser;
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<GroupUser> _groupUserRepository;
        private readonly IStoredProcedure _storedProcedure;
        public AccountService(IHttpContextAccessor httpContextAccessor,
                            IUserRepository userRepository,
                            IConfiguration configuration,
                            IUnitOfWork unitOfWork,
                            IRepository<Group> groupRepository,
                            IRepository<GroupUser> groupUserRepository,
                            IStoredProcedure storedProcedure)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _configuration = configuration;
            _unitOfWork = unitOfWork;

            _currentUser = _userRepository.GetCurrentUser();
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
            _storedProcedure = storedProcedure;
        }

        public Task<UserVM> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserVM> GetByEmail(string email)
        {
            return await _userRepository.Get<UserVM>(x => x.Email == email);
        }

        /// <summary>
        /// Will send an invite or request to the user to join or form a group
        /// </summary>
        /// <param name="email">Email of the user receiving the invite or request</param>
        /// <returns></returns>
        public async Task SendUserGroupRequest(string email)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;

                    var recepientUser = await _userRepository.Get(x => x.Email == email);
                    if (recepientUser == null)
                        throw new ApplicationException("Invalid Recepient");

                    // get current group
                    var groupId = (await _groupUserRepository.Get(x => x.UserId == _currentUser.UserId))?.Id;
                    if (groupId == null)
                    {
                        // TODO: determine default name 
                        var group = new Group
                        {
                            Name = "Group"
                        };
                        await _groupRepository.Create(group);
                        await _unitOfWork.SaveChangesAsync(transactionDate);

                        groupId = group.Id;
                    }

                    // create user group
                    var userGroup = new GroupUser
                    {
                        UserId = recepientUser.Id,
                        GroupId = groupId.Value,
                        IsAccepted = false
                    };
                    await _groupUserRepository.Create(userGroup); 
                    await _unitOfWork.SaveChangesAsync(transactionDate);

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw ex;
                }
            }
        }

        public async Task AcceptUserGroupRequest(int groupId)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var transactionDate = DateTime.UtcNow;

                    var userGroup = await _groupUserRepository.Get(x => x.GroupId == groupId && x.UserId == _currentUser.UserId);
                    if (userGroup == null)
                        throw new ApplicationException("Invalid Group");

                    // if already accepted, do nothing??
                    if (userGroup.IsAccepted)
                        return;


                    var props = new List<string>
                    {
                        nameof(GroupUser.IsAccepted),
                        nameof(GroupUser.AcceptedDate)
                    };

                    userGroup.IsAccepted = true;
                    userGroup.AcceptedDate = transactionDate;
                    await _groupUserRepository.Update(userGroup.Id!, userGroup, props);

                    await _unitOfWork.SaveChangesAsync(transactionDate);
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw ex;
                }
            }
        }

        public async Task<UserRegistrationResponse> Register(EmailPasswordRegistrationRequest requestData)
        {
            var result = new UserRegistrationResponse();
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(requestData.Email)
                    || string.IsNullOrWhiteSpace(requestData.Password)
                    || string.IsNullOrWhiteSpace(requestData.DisplayName))
                    {
                        throw new ApplicationException("Invalid request model.");
                    }

                    // create user record to firebase
                    UserRecordArgs userRecord = new UserRecordArgs
                    {
                        Email = requestData.Email.Trim(),
                        Password = requestData.Password.Trim(),
                        DisplayName = requestData.DisplayName.Trim()

                    };
                    var firebaseUser = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).CreateUserAsync(userRecord);

                    // save to db
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = firebaseUser.Email,
                        DisplayName = firebaseUser.DisplayName,
                        UniqueId = firebaseUser.Uid,
                    };
                    await _userRepository.Create(user);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    var parameters = new List<StoredProcedureRequestParameter>
                    {
                        new StoredProcedureRequestParameter("UserId", user.Id)
                    };
                    await _storedProcedure.ExecuteStoredProcedure("CreateUserDefaults", parameters);


                    result.IsSuccess = true;
                    result.AllowToLogin = true;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    if (ex is FirebaseAuthException && (ex as FirebaseAuthException).AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                    {
                        
                        result.ErrorMessages.Add("User already exists. Please try to login or reset your password.");
                        result.AllowToLogin = true;
                    }

                    result.ErrorMessages.Add(ex.Message);
                    result.IsSuccess = false;
                }
            }


            return result;
            
        }

        private async Task<UserResponseModel> Register(string token)
        {
            Guid userId = Guid.NewGuid();
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var firebaseToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);
                    var firebaseClaims = firebaseToken.Claims;

                    var email = firebaseClaims.FirstOrDefault(x => x.Key == "email");
                    var picture = firebaseClaims.FirstOrDefault(x => x.Key == "picture");
                    var isEmailVerified = firebaseClaims.FirstOrDefault(x => x.Key == "email_verified");
                    var displayName = firebaseClaims.FirstOrDefault(x => x.Key == "name");

                    var newUser = new User
                    {
                        Id = userId,
                        Email = email.Value.ToString(),
                        UniqueId = firebaseToken.Uid.ToString(),
                        DisplayName = displayName.Value.ToString(),
                    };
                    await _userRepository.Create(newUser);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    var parameters = new List<StoredProcedureRequestParameter>
                    {
                        new StoredProcedureRequestParameter("UserId", userId)
                    };
                    await _storedProcedure.ExecuteStoredProcedure("CreateUserDefaults", parameters);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;

                }
            }

            return await _userRepository.Get<UserResponseModel>(x => x.Id == userId);
        }

        public async Task<AuthRequestResult> Login(string token)
        {
            var result = new AuthRequestResult();

            try
            {
                var firebaseToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);

                var firebaseClaims = firebaseToken.Claims;

                var claims = new List<Claim>();
                var user = await _userRepository.Get<UserResponseModel>(x => x.UniqueId == firebaseToken.Uid);

                // Register if not yet exists in db
                if (user == null)
                {
                    user = await Register(token);
                }

                if (user != null)
                {
                    var name = firebaseClaims.FirstOrDefault(x => x.Key == "name");
                    var picture = firebaseClaims.FirstOrDefault(x => x.Key == "picture");
                    var isEmailVerified = firebaseClaims.FirstOrDefault(x => x.Key == "email_verified");

                    claims.Add(new Claim("Email", user.Email));
                    claims.Add(new Claim("UserId", user.Id.ToString()));
                    claims.Add(new Claim("Name", user.DisplayName ?? name.Value?.ToString() ?? string.Empty));
                    claims.Add(new Claim("PhotoUrl", picture.Value?.ToString() ?? string.Empty));
                    claims.AddRange(user.Roles.Select(r => new Claim("Role", r.Name)));

                    result.Token = GenerateAccessToken(claims);
                    result.IsAuthorized = true;
                    result.IsEmailVerified = bool.Parse(isEmailVerified.Value.ToString());
                }

            }
            catch (Exception ex)
            {
                result.IsAuthorized = false;
                // throw ?
            }

            return result;
        }


        private string GenerateAccessToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int.TryParse(_configuration["JwtSettings:Duration"], out int duration);
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
           return tokenString;
        }

    }
}
