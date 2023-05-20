using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Expense;
using ExpenseTracker.Model.Models.Reminder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly IReminderService _reminderService;
        public RemindersController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _reminderService.Get(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetReminders(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return BadRequest();
            }
            return Ok(await _reminderService.GetAll(startDate.Value, endDate.Value));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReminderRequestModel reminder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _reminderService.Create(reminder));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ReminderRequestModel reminder)
        {
            if (!ModelState.IsValid || id != reminder.Id)
            {
                return BadRequest();
            }

            return Ok(await _reminderService.Update(reminder));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _reminderService.Delete(id);
            return Ok();
        }
    }
}
