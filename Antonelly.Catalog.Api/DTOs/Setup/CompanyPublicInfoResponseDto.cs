namespace Antonelly.Catalog.Api.DTOs.Setup
{
    public class CompanyPublicInfoResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? WhatsappNumber { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
