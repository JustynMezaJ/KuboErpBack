namespace Antonelly.Catalog.Api.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public Company Company { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
