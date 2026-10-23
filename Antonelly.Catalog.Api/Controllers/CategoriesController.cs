using Microsoft.AspNetCore.Mvc;
using Antonelly.Catalog.Api.Common;
using Antonelly.Catalog.Api.DTOs.Categories;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetListCategories/{companySlug}")]
        public async Task<IActionResult> GetListCategories(string companySlug)
        {
            var result = await _categoryService.GetListCategoriesAsync(companySlug);
            return Ok(new ApiResponse<IEnumerable<CategoryAdminDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = result
            });
        }
    }
}
