using Microsoft.AspNetCore.Mvc;
using Antonelly.Catalog.Api.Common;
using Antonelly.Catalog.Api.DTOs.Products;
using Antonelly.Catalog.Api.Enums;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("GetListProducts")]
        public async Task<IActionResult> GetListProducts([FromBody] GetListProductsRequestDto request)
        {
            try
            {
                var result = await _productService.GetListProductsAsync(request);
                return Ok(new ApiResponse<IEnumerable<ProductResponseDto>>
                {
                    Success = true,
                    Message = "Products retrieved successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("FindById/{id}")]
        public async Task<IActionResult> FindById(Guid id)
        {
            var result = await _productService.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<ProductResponseDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = result
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request)
        {
            try
            {
                var result = await _productService.CreateAsync(request);
                return Ok(new ApiResponse<ProductResponseDto>
                {
                    Success = true,
                    Message = "Product created successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequestDto request)
        {
            try
            {
                var result = await _productService.UpdateAsync(id, request);
                return Ok(new ApiResponse<ProductResponseDto>
                {
                    Success = true,
                    Message = "Product updated successfully",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateProductStatusRequestDto request)
        {
            try
            {
                var result = await _productService.UpdateStatusAsync(id, request.Status);
                return Ok(new ApiResponse<ProductResponseDto>
                {
                    Success = true,
                    Message = "Product status updated successfully",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("UpdateVisibility/{id}")]
        public async Task<IActionResult> UpdateVisibility(Guid id, [FromBody] UpdateProductVisibilityRequestDto request)
        {
            try
            {
                var result = await _productService.UpdateVisibilityAsync(id, request.IsVisible);
                return Ok(new ApiResponse<ProductResponseDto>
                {
                    Success = true,
                    Message = "Product visibility updated successfully",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
