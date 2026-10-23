namespace Antonelly.Catalog.Api.DTOs.Categories
{
    public class CategoryAdminDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
