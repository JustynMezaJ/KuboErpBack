using Microsoft.AspNetCore.Http;
using Antonelly.Catalog.Api.DTOs.Images;

namespace Antonelly.Catalog.Api.Services.Interfaces
{
    public interface IImageStorageService
    {
        Task<ImageUploadResponseDto> UploadImageAsync(
            IFormFile file,
            string folder);

        string GetOptimizedImageUrl(string publicId);

        Task<bool> DeleteImageAsync(string publicId);
    }
}
