﻿using ExpenseTracker.Business.Interfaces;
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
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CurrentUserDetails _currentUser;
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<GroupUser> _groupUserRepository;
        private readonly IStoredProcedure _storedProcedure;
        public UserService(IHttpContextAccessor httpContextAccessor,
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
        public async Task<AuthRequestResult> Register(EmailPasswordRegistrationRequest requestData)
        {
            var result = new AuthRequestResult();
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


                    IReadOnlyDictionary<string, object> customClaims = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>
                    {
                        { "userId", user.Id }
                    });
                    await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).SetCustomUserClaimsAsync(firebaseUser.Uid, customClaims);
                    var customToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).CreateCustomTokenAsync(firebaseUser.Uid);

                    result.Token = customToken;
                    result.IsAuthorized = true;

                }
                catch (Exception ex)
                {
                    result.IsAuthorized = false;
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }


            return result;
            
        }


        public async Task<AuthRequestResult> Login(string token)
        {
            var result = new AuthRequestResult();

            try
            {
                var firebaseToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);

                var firebaseClaims = firebaseToken.Claims;

                // TODO: remove initialization here, other value will be in the else statement below
                Guid userId = Guid.NewGuid();
                var claims = new List<Claim>();
                var user = await _userRepository.Get<UserResponseModel>(x => x.UniqueId == firebaseToken.Uid);
                if (user != null)
                {
                    result.NeedToCompleteProfile = false;
                    result.IsNewUser = false;
                    userId = user.Id;

                    var name = firebaseClaims.FirstOrDefault(x => x.Key == "name");
                    var picture = firebaseClaims.FirstOrDefault(x => x.Key == "picture");

                    claims.Add(new Claim("Email", user.Email));
                    claims.Add(new Claim("UserId", user.Id.ToString()));
                    claims.Add(new Claim("Name", user.DisplayName ?? name.Value?.ToString() ?? string.Empty));
                    claims.Add(new Claim("PhotoUrl", picture.Value?.ToString() ?? string.Empty));
                    claims.AddRange(user.Roles.Select(r => new Claim("Role", r.Name)));
                }
                else
                {
                    // TODO: add flag to check if new users will be auto registered
                    // TODO: implement 
                }

                result.Token = GenerateAccessToken(claims);
                result.IsAuthorized = true;
            }
            catch (Exception ex)
            {
                result.IsAuthorized = false;

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
