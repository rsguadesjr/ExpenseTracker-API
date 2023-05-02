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
        private readonly IExpenseService _transactionService;
        public ExpensesController(IExpenseService transactionService)
        {
            _transactionService= transactionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(await _transactionService.Get(id));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetExpenses([FromBody] BaseSearchParameter searchParam)
        {
            return Ok(await _transactionService.GetAll(searchParam));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _transactionService.Create(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ExpenseDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id)
            {
                return BadRequest();
            }

            return Ok(await _transactionService.Update(dto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            await _transactionService.Delete(id);
            return Ok();
        }


    }
}
