using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Category;
using ExpenseTracker.Model.Models.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/<CategoriesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _categoryService.GetAll());
        }


        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequestModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _categoryService.Create(data));   
        }


        [Authorize(Roles = "SuperAdmin, Admin, Standard")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryRequestModel data)
        {
            if (!ModelState.IsValid || id != data.Id)
            {
                return BadRequest();
            }

            return Ok(await _categoryService.Update(data));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Sort(List<SortItem> data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
