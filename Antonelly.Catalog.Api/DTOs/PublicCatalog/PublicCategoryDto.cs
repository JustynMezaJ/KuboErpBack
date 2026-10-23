namespace Antonelly.Catalog.Api.DTOs.PublicCatalog
{
    public class PublicCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
