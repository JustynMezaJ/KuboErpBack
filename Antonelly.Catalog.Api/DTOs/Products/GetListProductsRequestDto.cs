using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.DTOs.Products
{
    public class GetListProductsRequestDto
    {
        public Guid? CategoryId { get; set; }
        public ProductStatus? Status { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsFeatured { get; set; }
        public string? Search { get; set; }
    }
}
