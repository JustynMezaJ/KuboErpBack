using Antonelly.Catalog.Api.DTOs.Products;
using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request);
        Task<IEnumerable<ProductResponseDto>> GetListProductsAsync(GetListProductsRequestDto request);
        Task<ProductResponseDto?> FindByIdAsync(Guid id);
        Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductRequestDto request);
        Task<ProductResponseDto> UpdateStatusAsync(Guid id, ProductStatus status);
        Task<ProductResponseDto> UpdateVisibilityAsync(Guid id, bool isVisible);
    }
}
