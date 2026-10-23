using Microsoft.AspNetCore.Mvc;
using Antonelly.Catalog.Api.Common;
using Antonelly.Catalog.Api.DTOs.Images;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageStorageService _imageStorage;
        private const long MaxFileSize = 8 * 1024 * 1024;
        private static readonly string[] AllowedFormats = ["image/jpeg", "image/png", "image/webp"];

        public ImagesController(IImageStorageService imageStorage)
        {
            _imageStorage = imageStorage;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> Upload(
            [FromForm] ImageUploadRequestDto request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "File is required"
                });
            }

            if (file.Length > MaxFileSize)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "File size must not exceed 8 MB"
                });
            }

            if (!AllowedFormats.Contains(file.ContentType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Only JPEG, PNG and WebP images are allowed"
                });
            }

            try
            {
                var result = await _imageStorage.UploadImageAsync(
                    file,
                    "antonelly/products/test");

                return Ok(new ApiResponse<ImageUploadResponseDto>
                {
                    Success = true,
                    Message = "Image uploaded successfully",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
