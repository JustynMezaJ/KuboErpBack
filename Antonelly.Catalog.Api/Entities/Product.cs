using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ReferenceCode { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; } = "COP";
        public bool ShowPrice { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Available;
        public bool IsFeatured { get; set; }
        public bool IsVisible { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public Company Company { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
