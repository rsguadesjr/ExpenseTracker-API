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
        private readonly IUserService _userService;
        private IConfiguration _configuration;
        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] AuthRequest token)
        {
            //return BadRequest("Invalid Token");
            return Ok(await _userService.Login(token.Token));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GoogleSignIn([FromBody] AuthRequest token)
        {

            return Ok(await _userService.Login(token));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterWithEmailAndPassword([FromBody] EmailPasswordRegistrationRequest request)
        {

            return Ok(await _userService.Register(request));
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetSettings()
        {
            var test = new
            {
                test = _configuration.GetConnectionString("DefaultConnection")
            };
            return Ok(test);
        }


    }
}
