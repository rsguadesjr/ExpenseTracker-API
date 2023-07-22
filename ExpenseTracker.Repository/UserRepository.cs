using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserRepository(ExpenseTrackerContext context, IMapper mapper, IHttpContextAccessor httpContext) : base(context, mapper)
        {

            _httpContext = httpContext;

        }


        public override async Task<User> Create(User entity)
        {
            var existingUser = await _context.Users.AnyAsync(x => x.Email == entity.Email || x.UniqueId == entity.UniqueId);
            if (existingUser)
            {
                throw new Exception("User Already Exists");
            }

            await _context.AddAsync(entity);
            return entity;
        }

        public CurrentUserDetails GetCurrentUser()
        {
            var user = _httpContext.HttpContext.User;
            var email = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var displayName = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var uniqueId = user.Claims.FirstOrDefault(x => x.Type == "UniqueId")?.Value;
            var userId = user.Claims.FirstOrDefault(x => x.Type== "UserId")?.Value;
            Guid.TryParse(userId, out Guid parsedUserId);

            return new CurrentUserDetails
            {
                UserId = parsedUserId,
                DisplayName = displayName,
                Email = email
            };
        }
    }
}
