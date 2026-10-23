using Microsoft.AspNetCore.Http;

namespace Antonelly.Catalog.Api.DTOs.ProductImages
{
    public class UploadProductImageRequestDto
    {
        public IFormFile File { get; set; } = null!;
        public string? AltText { get; set; }
        public bool? IsMain { get; set; }
    }
}
