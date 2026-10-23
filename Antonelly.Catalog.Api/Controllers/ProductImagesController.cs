using Microsoft.AspNetCore.Mvc;
using Antonelly.Catalog.Api.Common;
using Antonelly.Catalog.Api.DTOs.ProductImages;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/product-images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _productImageService;

        public ProductImagesController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpPost("UploadImage/{productId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(Guid productId, [FromForm] UploadProductImageRequestDto request)
        {
            try
            {
                var result = await _productImageService.UploadImageAsync(productId, request);
                return Ok(new ApiResponse<ProductImageResponseDto>
                {
                    Success = true,
                    Message = "Product image uploaded successfully",
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
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
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

        [HttpGet("GetListImages/{productId}")]
        public async Task<IActionResult> GetListImages(Guid productId)
        {
            try
            {
                var result = await _productImageService.GetListImagesAsync(productId);
                return Ok(new ApiResponse<IEnumerable<ProductImageResponseDto>>
                {
                    Success = true,
                    Message = "Product images retrieved successfully",
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

        [HttpPut("SetMainImage/{productId}/{imageId}")]
        public async Task<IActionResult> SetMainImage(Guid productId, Guid imageId)
        {
            try
            {
                var result = await _productImageService.SetMainImageAsync(productId, imageId);
                return Ok(new ApiResponse<ProductImageResponseDto>
                {
                    Success = true,
                    Message = "Main image updated successfully",
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

        [HttpPut("UpdateImageOrder/{productId}")]
        public async Task<IActionResult> UpdateImageOrder(Guid productId, [FromBody] UpdateProductImageOrderRequestDto request)
        {
            try
            {
                var result = await _productImageService.UpdateImageOrderAsync(productId, request);
                return Ok(new ApiResponse<IEnumerable<ProductImageResponseDto>>
                {
                    Success = true,
                    Message = "Images order updated successfully",
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

        [HttpDelete("DeleteImage/{productId}/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId)
        {
            try
            {
                var result = await _productImageService.DeleteImageAsync(productId, imageId);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Product image deleted successfully",
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
