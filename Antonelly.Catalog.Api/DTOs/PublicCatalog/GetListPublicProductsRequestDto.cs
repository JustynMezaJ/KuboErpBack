using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.DTOs.PublicCatalog
{
    public class GetListPublicProductsRequestDto
    {
        public string CompanySlug { get; set; } = string.Empty;
        public string? CategorySlug { get; set; }
        public ProductStatus? Status { get; set; }
        public string? Search { get; set; }
        public bool? OnlyFeatured { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string? SortBy { get; set; }
    }
}
