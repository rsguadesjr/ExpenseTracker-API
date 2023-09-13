﻿using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] AuthRequest token)
        {
            return Ok(await _userService.Login(token.Token));
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithPassword([FromBody] AuthRequest token)
        {
            return Ok(await _userService.Login(token.Token));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterWithEmailAndPassword([FromBody] EmailPasswordRegistrationRequest request)
        {

            return Ok(await _userService.Register(request));
        }
    }
}
