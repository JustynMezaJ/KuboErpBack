using Microsoft.AspNetCore.Mvc;
using Antonelly.Catalog.Api.Common;
using Antonelly.Catalog.Api.DTOs.PublicCatalog;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/public-catalog")]
    public class PublicCatalogController : ControllerBase
    {
        private readonly IPublicCatalogService _publicCatalogService;

        public PublicCatalogController(IPublicCatalogService publicCatalogService)
        {
            _publicCatalogService = publicCatalogService;
        }

        [HttpGet("GetCompany/{companySlug}")]
        public async Task<IActionResult> GetCompany(string companySlug)
        {
            var result = await _publicCatalogService.GetCompanyAsync(companySlug);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            return Ok(new ApiResponse<PublicCompanyDto>
            {
                Success = true,
                Message = "Company retrieved successfully",
                Data = result
            });
        }

        [HttpGet("GetHome/{companySlug}")]
        public async Task<IActionResult> GetHome(string companySlug)
        {
            var result = await _publicCatalogService.GetHomeAsync(companySlug);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            return Ok(new ApiResponse<PublicCatalogHomeResponseDto>
            {
                Success = true,
                Message = "Catalog home loaded successfully",
                Data = result
            });
        }

        [HttpGet("GetCategories/{companySlug}")]
        public async Task<IActionResult> GetCategories(string companySlug)
        {
            var result = await _publicCatalogService.GetCategoriesAsync(companySlug);

            return Ok(new ApiResponse<IEnumerable<PublicCategoryDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = result
            });
        }

        [HttpPost("GetListProducts")]
        public async Task<IActionResult> GetListProducts([FromBody] GetListPublicProductsRequestDto request)
        {
            var result = await _publicCatalogService.GetListProductsAsync(request);

            return Ok(new ApiResponse<PagedResponseDto<PublicProductCardDto>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = result
            });
        }

        [HttpGet("FindProductBySlug/{companySlug}/{productSlug}")]
        public async Task<IActionResult> FindProductBySlug(string companySlug, string productSlug)
        {
            var result = await _publicCatalogService.FindProductBySlugAsync(companySlug, productSlug);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<PublicProductDetailDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = result
            });
        }
    }
}
