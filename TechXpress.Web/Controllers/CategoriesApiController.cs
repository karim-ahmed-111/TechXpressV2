using Microsoft.AspNetCore.Mvc;
using TechXpress.Services;
using TechXpress.Data.Entities;

namespace TechXpress.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesApiController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
    }
} 