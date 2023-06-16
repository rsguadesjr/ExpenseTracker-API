using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Models.Budget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        // GET: api/<BudgetsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _budgetService.GetAll());
        }

        // GET api/<BudgetsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _budgetService.Get(id));
        }

        // POST api/<BudgetsController>


        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BudgetRequestModel data)
        {
            if (!ModelState.IsValid || data == null)
            {
                return BadRequest("Invalid Request");
            }

            return Ok(await _budgetService.Create(data));
        }

        // PUT api/<BudgetsController>/5
        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BudgetRequestModel data)
        {
            if (!ModelState.IsValid || data == null || id != data.Id)
            {
                return BadRequest("Invalid Request");
            }

            return Ok(await _budgetService.Update(data));
        }

        // DELETE api/<BudgetsController>/5
        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _budgetService.Delete(id);
            return Ok();
        }
    }
}
