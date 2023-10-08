using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _userService;
        private readonly IConfiguration _configuration;
        private readonly AppUserManagementSetting _settings;
        public AccountController(IAccountService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _settings = _configuration.GetSection("AppUserManagement").Get<AppUserManagementSetting>();
        }


        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPost("[action]/{email}")]
        public async Task<IActionResult> SendUserGroupRequest(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid Email");

            await _userService.SendUserGroupRequest(email.Trim());
            return Ok();
        }

        [HttpPost("[action]/{groupId}")]
        public async Task<IActionResult> AcceptUserGroupRequest(int groupId)
        {
            await _userService.AcceptUserGroupRequest(groupId);
            return Ok();
        }
    }
}
