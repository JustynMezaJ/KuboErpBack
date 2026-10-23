using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Antonelly.Catalog.Api.Configuration;
using Antonelly.Catalog.Api.DTOs.Images;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Services.Implementations
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public CloudinaryImageStorageService(IOptions<CloudinarySettings> settings)
        {
            _settings = settings.Value;

            ValidateConfiguration();

            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_settings.CloudName))
                throw new InvalidOperationException("Cloudinary CloudName is not configured.");

            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new InvalidOperationException("Cloudinary ApiKey is not configured.");

            if (string.IsNullOrWhiteSpace(_settings.ApiSecret))
                throw new InvalidOperationException("Cloudinary ApiSecret is not configured.");
        }

        public async Task<ImageUploadResponseDto> UploadImageAsync(
            IFormFile file,
            string folder)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return new ImageUploadResponseDto
            {
                PublicId = result.PublicId,
                Url = result.Url?.AbsoluteUri ?? string.Empty,
                SecureUrl = result.SecureUrl?.AbsoluteUri ?? string.Empty,
                OptimizedUrl = GetOptimizedImageUrl(result.PublicId),
                Width = result.Width,
                Height = result.Height,
                Format = result.Format ?? string.Empty,
                Bytes = result.Bytes,
                OriginalFilename = file.FileName
            };
        }

        public string GetOptimizedImageUrl(string publicId)
        {
            // f_auto: Cloudinary delivers the most efficient format (e.g. webp)
            // q_auto: Cloudinary automatically adjusts quality for optimal delivery
            return _cloudinary.Api.UrlImgUp
                .Secure()
                .Transform(new CloudinaryDotNet.Transformation()
                    .Quality("auto")
                    .FetchFormat("auto"))
                .BuildUrl(publicId);
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
    }
}
