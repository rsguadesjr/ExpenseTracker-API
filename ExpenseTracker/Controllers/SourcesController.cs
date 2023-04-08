using ExpenseTracker.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourcesController : ControllerBase
    {
        private readonly ISourceService _sourceService;
        public SourcesController(ISourceService sourceService)
        {
            _sourceService = sourceService;
        }
        // GET: api/<SourcesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _sourceService.GetAll());
        }
    }
}
