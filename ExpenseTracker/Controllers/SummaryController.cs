using ExpenseTracker.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _summaryService;
        public SummaryController(ISummaryService summaryService)
        {
            _summaryService = summaryService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSummaryByRange([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return BadRequest();
            }

            return Ok(await _summaryService.GetSummaryByDateRange(startDate.Value, endDate.Value));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMonthlySummaryByYear([FromQuery] int? year)
        {
            if (year == null)
            {
                return BadRequest();
            }

            return Ok(await _summaryService.GetMonthlySummaryByYear(year.Value));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTotalAmountPerCategory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return BadRequest();
            }

            return Ok(await _summaryService.GetTotalAmountPerCategory(startDate.Value, endDate.Value));
        }
    }
}
