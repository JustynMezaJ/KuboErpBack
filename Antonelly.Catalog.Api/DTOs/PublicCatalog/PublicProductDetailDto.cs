namespace Antonelly.Catalog.Api.DTOs.PublicCatalog
{
    public class PublicProductDetailDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ReferenceCode { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusLabel { get; set; } = string.Empty;
        public bool ShowPrice { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PriceText { get; set; } = string.Empty;
        public PublicProductImageDto MainImage { get; set; } = null!;
        public List<PublicProductImageDto> GalleryImages { get; set; } = new();
        public PublicWhatsappDto Whatsapp { get; set; } = null!;
        public List<PublicProductCardDto> RelatedProducts { get; set; } = new();
        public PublicSeoDto Seo { get; set; } = null!;
    }
}
