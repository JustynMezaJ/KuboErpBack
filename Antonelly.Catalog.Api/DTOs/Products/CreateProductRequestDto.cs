using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.DTOs.Products
{
    public class CreateProductRequestDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ReferenceCode { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; } = "COP";
        public bool ShowPrice { get; set; } = false;
        public ProductStatus Status { get; set; } = ProductStatus.Available;
        public bool IsFeatured { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
