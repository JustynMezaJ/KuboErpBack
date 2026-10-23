namespace Antonelly.Catalog.Api.DTOs.Images
{
    public class ImageDetailsDto
    {
        public string PublicId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string OptimizedUrl { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
    }
}
