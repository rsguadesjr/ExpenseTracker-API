using ExpenseTracker.Business.Interfaces;
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
        private readonly IAccountService _userService;
        private readonly IConfiguration _configuration;
        private readonly AppUserManagementSetting _settings;
        public AuthController(IAccountService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _settings = _configuration.GetSection("AppUserManagement").Get<AppUserManagementSetting>();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] AuthRequest token)
        {
            return Ok(await _userService.Login(token.Token));
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithPassword([FromBody] AuthRequest token)
        {
            if (_settings.DisableEmailPasswordLogin)
                return BadRequest();

            return Ok(await _userService.Login(token.Token));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterWithEmailAndPassword([FromBody] EmailPasswordRegistrationRequest request)
        {
            if (_settings.DisableRegistration)
                return BadRequest();

            return Ok(await _userService.Register(request));
        }

        //[HttpPost("[action]")]
        //public async Task<IActionResult> Register([FromBody] AuthRequest token)
        //{
        //    return Ok(await _userService.Register(token.Token));
        //}
    }
}
