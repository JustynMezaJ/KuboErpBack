namespace Antonelly.Catalog.Api.DTOs.PublicCatalog
{
    public class PublicCatalogHomeResponseDto
    {
        public PublicCompanyDto Company { get; set; } = null!;
        public PublicHeroDto Hero { get; set; } = null!;
        public List<PublicTrustBenefitDto> TrustBenefits { get; set; } = new();
        public List<PublicCategoryDto> Categories { get; set; } = new();
        public List<PublicProductCardDto> FeaturedProducts { get; set; } = new();
        public List<PublicProductCardDto> NewArrivals { get; set; } = new();
        public PublicWhatsappDto Whatsapp { get; set; } = null!;
    }
}
