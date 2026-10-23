using Antonelly.Catalog.Api.DTOs.PublicCatalog;

namespace Antonelly.Catalog.Api.Services.Interfaces
{
    public interface IPublicCatalogService
    {
        Task<PublicCompanyDto?> GetCompanyAsync(string companySlug);
        Task<IEnumerable<PublicCategoryDto>> GetCategoriesAsync(string companySlug);
        Task<PublicCatalogHomeResponseDto?> GetHomeAsync(string companySlug);
        Task<PagedResponseDto<PublicProductCardDto>> GetListProductsAsync(GetListPublicProductsRequestDto request);
        Task<PublicProductDetailDto?> FindProductBySlugAsync(string companySlug, string productSlug);
    }
}
