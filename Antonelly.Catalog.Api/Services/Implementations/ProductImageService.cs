using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Configuration;
using Antonelly.Catalog.Api.Data;
using Antonelly.Catalog.Api.DTOs.ProductImages;
using Antonelly.Catalog.Api.Entities;
using Antonelly.Catalog.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Antonelly.Catalog.Api.Services.Implementations
{
    public class ProductImageService : IProductImageService
    {
        private readonly AppDbContext _context;
        private readonly IImageStorageService _imageStorage;
        private const string DefaultCompanySlug = "antonelly";

        public ProductImageService(AppDbContext context, IImageStorageService imageStorage)
        {
            _context = context;
            _imageStorage = imageStorage;
        }

        public async Task<ProductImageResponseDto> UploadImageAsync(Guid productId, UploadProductImageRequestDto request)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new KeyNotFoundException("Product not found.");

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == DefaultCompanySlug)
                ?? throw new InvalidOperationException("Default company not found.");

            if (product.CompanyId != company.Id)
            {
                throw new InvalidOperationException("Product does not belong to the company.");
            }

            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("File is required.");
            }

            if (request.File.Length > 8 * 1024 * 1024)
            {
                throw new ArgumentException("File size must not exceed 8 MB.");
            }

            var allowedFormats = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedFormats.Contains(request.File.ContentType))
            {
                throw new ArgumentException("Only JPEG, PNG and WebP images are allowed.");
            }

            // Generar carpeta de Cloudinary: antonelly/products/{referenceCode}-{productSlug}
            string sanitizedRef = Regex.Replace(product.ReferenceCode.ToLower(), @"[^a-z0-9-]", "-");
            string sanitizedSlug = Regex.Replace(product.Slug.ToLower(), @"[^a-z0-9-]", "-");
            string cloudFolder = $"antonelly/products/{sanitizedRef}-{sanitizedSlug}";

            var uploadResult = await _imageStorage.UploadImageAsync(request.File, cloudFolder);

            var existingImages = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            bool isMain = request.IsMain ?? false;
            int displayOrder = existingImages.Count + 1;

            if (existingImages.Count == 0)
            {
                isMain = true;
            }
            else if (isMain)
            {
                // Desmarcar otras imágenes como principales
                foreach (var img in existingImages)
                {
                    img.IsMain = false;
                }
            }

            var productImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                CompanyId = company.Id,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url,
                SecureUrl = uploadResult.SecureUrl,
                OptimizedUrl = uploadResult.OptimizedUrl,
                OriginalFilename = uploadResult.OriginalFilename,
                Format = uploadResult.Format,
                Bytes = uploadResult.Bytes,
                Width = uploadResult.Width,
                Height = uploadResult.Height,
                IsMain = isMain,
                DisplayOrder = displayOrder,
                AltText = request.AltText,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            return MapToDto(productImage);
        }

        public async Task<IEnumerable<ProductImageResponseDto>> GetListImagesAsync(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            var images = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .OrderBy(pi => pi.DisplayOrder)
                .ToListAsync();

            return images.Select(MapToDto);
        }

        public async Task<ProductImageResponseDto> SetMainImageAsync(Guid productId, Guid imageId)
        {
            var images = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            if (!images.Any()) throw new KeyNotFoundException("No images found for this product.");

            var targetImage = images.FirstOrDefault(pi => pi.Id == imageId)
                ?? throw new KeyNotFoundException("Image not found.");

            foreach (var img in images)
            {
                img.IsMain = false;
            }

            targetImage.IsMain = true;
            targetImage.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(targetImage);
        }

        public async Task<IEnumerable<ProductImageResponseDto>> UpdateImageOrderAsync(Guid productId, UpdateProductImageOrderRequestDto request)
        {
            var images = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            if (!images.Any()) throw new KeyNotFoundException("No images found for this product.");

            foreach (var item in request.Images)
            {
                var img = images.FirstOrDefault(i => i.Id == item.ImageId)
                    ?? throw new KeyNotFoundException($"Image {item.ImageId} not found for this product.");

                img.DisplayOrder = item.DisplayOrder;
                img.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();

            return images.Select(MapToDto).OrderBy(i => i.DisplayOrder);
        }

        public async Task<bool> DeleteImageAsync(Guid productId, Guid imageId)
        {
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.Id == imageId)
                ?? throw new KeyNotFoundException("Image not found for this product.");

            var publicId = productImage.PublicId;
            var wasMain = productImage.IsMain;

            _context.ProductImages.Remove(productImage);

            if (wasMain)
            {
                var nextMain = await _context.ProductImages
                    .Where(pi => pi.ProductId == productId && pi.Id != imageId)
                    .OrderBy(pi => pi.DisplayOrder)
                    .FirstOrDefaultAsync();

                if (nextMain != null)
                {
                    nextMain.IsMain = true;
                    nextMain.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            // Eliminar de Cloudinary
            await _imageStorage.DeleteImageAsync(publicId);

            return true;
        }

        private static ProductImageResponseDto MapToDto(ProductImage pi)
        {
            return new ProductImageResponseDto
            {
                Id = pi.Id,
                ProductId = pi.ProductId,
                PublicId = pi.PublicId,
                Url = pi.Url,
                SecureUrl = pi.SecureUrl,
                OptimizedUrl = pi.OptimizedUrl,
                OriginalFilename = pi.OriginalFilename,
                Format = pi.Format,
                Bytes = pi.Bytes,
                Width = pi.Width,
                Height = pi.Height,
                IsMain = pi.IsMain,
                DisplayOrder = pi.DisplayOrder,
                AltText = pi.AltText,
                CreatedAt = pi.CreatedAt,
                UpdatedAt = pi.UpdatedAt
            };
        }
    }
}
