namespace Antonelly.Catalog.Api.DTOs.PublicCatalog
{
    public class PublicCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? WhatsappNumber { get; set; }
        public string? InstagramUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
