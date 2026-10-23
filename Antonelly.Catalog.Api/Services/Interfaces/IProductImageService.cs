using Antonelly.Catalog.Api.DTOs.ProductImages;

namespace Antonelly.Catalog.Api.Services.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImageResponseDto> UploadImageAsync(Guid productId, UploadProductImageRequestDto request);
        Task<IEnumerable<ProductImageResponseDto>> GetListImagesAsync(Guid productId);
        Task<ProductImageResponseDto> SetMainImageAsync(Guid productId, Guid imageId);
        Task<IEnumerable<ProductImageResponseDto>> UpdateImageOrderAsync(Guid productId, UpdateProductImageOrderRequestDto request);
        Task<bool> DeleteImageAsync(Guid productId, Guid imageId);
    }
}
