using ExpenseTracker.Business.Interfaces;
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
        public UserService(IHttpContextAccessor httpContextAccessor,
                            IUserRepository userRepository,
                            IConfiguration configuration,
                            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
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

                    var parameters = new Dictionary<string, object>
                    {
                        { "UserId", user.Id },
                    };
                    await _userRepository.ExecuteStoredProcedure("CreateUserDefaults", parameters);


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
                var user = await _userRepository.Get<UserVM>(x => x.UniqueId == firebaseToken.Uid);
                if (user != null)
                {
                    result.NeedToCompleteProfile = false;
                    result.IsNewUser = false;
                    userId = user.Id;

                    var name = firebaseClaims.FirstOrDefault(x => x.Key == "name");
                    var picture = firebaseClaims.FirstOrDefault(x => x.Key == "picture");
                    claims.AddRange(new List<Claim>() {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Name, name.Value?.ToString() ?? string.Empty),
                        new Claim("PhotoUrl", picture.Value?.ToString() ?? string.Empty)
                    });
                }
                else
                {
                    // TODO: add flag to check if new users will be auto registered
                    // TODO: implement 
                }

                //// create custom claims here
                //IReadOnlyDictionary<string, object> customClaims = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>
                //{
                //    { "userId", userId }
                //});
                //await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).SetCustomUserClaimsAsync(firebaseToken.Uid, customClaims);
                //var customToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).CreateCustomTokenAsync(firebaseToken.Uid);

                result.Token = GenerateAccessToken(claims);
                result.IsAuthorized = true;
            }
            catch (Exception ex)
            {
                result.IsAuthorized = false;

            }

            return result;
        }


        public async Task<AuthRequestResult> Login(AuthRequest auth)
        {
            var result = new AuthRequestResult();
            if (string.IsNullOrWhiteSpace(auth?.Token))
                return null;

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(auth.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "476721190749-a1iafneed5dndqaoqk5l40mo3h6tpq1m.apps.googleusercontent.com" }
                });
                if (payload == null)
                {
                    
                    result.IsAuthorized = false;
                    return result;
                }

                var user = await _userRepository.Get(x => x.Email == payload.Email);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim("PhotoUrl", payload.Picture)
                };

                //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecureKeyYeahBoy"));
                //var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //var tokenOptions = new JwtSecurityToken(
                //    issuer: _configuration["JwtSettings:Issuer"],
                //    audience: _configuration["JwtSettings:Audience"],
                //    claims: claims,
                //    expires: DateTime.Now.AddMinutes(30),
                //    signingCredentials: signingCredentials
                //);

                //var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                result.Token = GenerateAccessToken(claims);
            }
            catch(Exception ex)
            {
                throw;
            }

            return result;
        }


        private string GenerateAccessToken(List<Claim> claims)
        {
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecureKeyYeahBoy"));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
           return tokenString;
        }

    }
}
