using ExpenseTracker.Model.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppUserManagementSetting _settings;
        public ConfigurationsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _settings = _configuration.GetSection("AppUserManagement").Get<AppUserManagementSetting>();
        }

        [HttpGet]
        public IActionResult GetConfigurations()
        {
            var config = new { AppUserManagementSettings = _settings };
            return Ok(config);
        }
    }
}
