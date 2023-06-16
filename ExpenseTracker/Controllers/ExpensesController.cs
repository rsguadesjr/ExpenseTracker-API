using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService= expenseService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(await _expenseService.Get(id));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetExpenses([FromBody] BaseSearchParameter searchParam)
        {
            //return Unauthorized();
            return Ok(await _expenseService.GetAll(searchParam));
        }


        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _expenseService.Create(dto));
        }

        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ExpenseDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id)
            {
                return BadRequest();
            }

            return Ok(await _expenseService.Update(dto));
        }

        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            await _expenseService.Delete(id);
            return Ok();
        }


    }
}
