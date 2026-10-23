namespace Antonelly.Catalog.Api.DTOs.PublicCatalog
{
    public class PublicProductImageDto
    {
        public string? AltText { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string CardImageUrl { get; set; } = string.Empty;
        public string DetailImageUrl { get; set; } = string.Empty;
        public string ZoomImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; }
    }
}
