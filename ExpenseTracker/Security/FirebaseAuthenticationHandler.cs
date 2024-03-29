﻿using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ExpenseTracker.Security
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserService userService) : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Context.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            string bearerToken = Context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Invalid Scheme");
            }

            try
            {
                string token = bearerToken.Substring("Bearer ".Length);
                var firebaseToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);
                var claims = await ToClaims(firebaseToken.Claims);
 
                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(claims, nameof(FirebaseAuthenticationHandler))
                }), JwtBearerDefaults.AuthenticationScheme));

            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private async Task<IEnumerable<Claim>> ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            var email = claims["email"]?.ToString();
            var name = claims["name"]?.ToString();
            var uniqueId = claims["user_id"]?.ToString();
            var userId = (await _userService.GetByEmail(email))?.Id;
            return new List<Claim>
            {
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Name, name ?? string.Empty),
                new Claim("UniqueId", uniqueId ?? string.Empty),
                new Claim("UserId", userId?.ToString() ?? string.Empty),
            };
        }
    }
}
