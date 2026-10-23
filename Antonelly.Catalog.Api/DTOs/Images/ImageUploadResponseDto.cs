namespace Antonelly.Catalog.Api.DTOs.Images
{
    public class ImageUploadResponseDto
    {
        public string PublicId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
        public string OptimizedUrl { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Format { get; set; } = string.Empty;
        public long Bytes { get; set; }
        public string OriginalFilename { get; set; } = string.Empty;
    }
}
