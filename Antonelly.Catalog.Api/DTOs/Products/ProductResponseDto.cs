using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.DTOs.Products
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ReferenceCode { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool ShowPrice { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsVisible { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
