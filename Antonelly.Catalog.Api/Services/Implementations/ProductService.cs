using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Configuration;
using Antonelly.Catalog.Api.Data;
using Antonelly.Catalog.Api.DTOs.Products;
using Antonelly.Catalog.Api.Entities;
using Antonelly.Catalog.Api.Enums;
using Antonelly.Catalog.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Antonelly.Catalog.Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private const string DefaultCompanySlug = "antonelly";

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == DefaultCompanySlug)
                ?? throw new InvalidOperationException("Default company not found.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.CompanyId == company.Id)
                ?? throw new InvalidOperationException("Category not found or does not belong to the company.");

            if (await _context.Products.AnyAsync(p => p.CompanyId == company.Id && p.ReferenceCode == request.ReferenceCode))
            {
                throw new InvalidOperationException($"Product with reference code {request.ReferenceCode} already exists for this company.");
            }

            var slug = GenerateUniqueSlug(request.Name, company.Id);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                CategoryId = category.Id,
                Name = request.Name,
                Slug = slug,
                ReferenceCode = request.ReferenceCode,
                Brand = request.Brand,
                Model = request.Model,
                ShortDescription = request.ShortDescription,
                Description = request.Description,
                Price = request.Price,
                Currency = request.Currency,
                ShowPrice = request.ShowPrice,
                Status = request.Status,
                IsFeatured = request.IsFeatured,
                IsVisible = request.IsVisible,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await MapToDto(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetListProductsAsync(GetListProductsRequestDto request)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == DefaultCompanySlug)
                ?? throw new InvalidOperationException("Default company not found.");

            var query = _context.Products
                .Where(p => p.CompanyId == company.Id)
                .AsQueryable();

            if (request?.CategoryId != null)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request?.Status != null)
                query = query.Where(p => p.Status == request.Status.Value);

            if (request?.IsVisible != null)
                query = query.Where(p => p.IsVisible == request.IsVisible.Value);

            if (request?.IsFeatured != null)
                query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);

            if (!string.IsNullOrWhiteSpace(request?.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(s) || 
                    p.ReferenceCode.ToLower().Contains(s) || 
                    (p.Brand != null && p.Brand.ToLower().Contains(s)) || 
                    (p.Model != null && p.Model.ToLower().Contains(s)));
            }

            var products = await query
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var results = new List<ProductResponseDto>();
            foreach (var p in products)
            {
                results.Add(await MapToDto(p));
            }

            return results;
        }

        public async Task<ProductResponseDto?> FindByIdAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return await MapToDto(product);
        }

        public async Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductRequestDto request)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException("Product not found.");

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == DefaultCompanySlug)
                ?? throw new InvalidOperationException("Default company not found.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.CompanyId == company.Id)
                ?? throw new InvalidOperationException("Category not found or does not belong to the company.");

            if (request.ReferenceCode != product.ReferenceCode)
            {
                if (await _context.Products.AnyAsync(p => p.CompanyId == company.Id && p.ReferenceCode == request.ReferenceCode))
                {
                    throw new InvalidOperationException($"Product with reference code {request.ReferenceCode} already exists for this company.");
                }
            }

            if (request.Name != product.Name)
            {
                product.Slug = GenerateUniqueSlug(request.Name, company.Id);
            }

            product.CategoryId = category.Id;
            product.Name = request.Name;
            product.ReferenceCode = request.ReferenceCode;
            product.Brand = request.Brand;
            product.Model = request.Model;
            product.ShortDescription = request.ShortDescription;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Currency = request.Currency;
            product.ShowPrice = request.ShowPrice;
            product.IsFeatured = request.IsFeatured;
            product.IsVisible = request.IsVisible;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return await MapToDto(product);
        }

        public async Task<ProductResponseDto> UpdateStatusAsync(Guid id, ProductStatus status)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Status = status;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return await MapToDto(product);
        }

        public async Task<ProductResponseDto> UpdateVisibilityAsync(Guid id, bool isVisible)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException("Product not found.");

            product.IsVisible = isVisible;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return await MapToDto(product);
        }

        private async Task<ProductResponseDto> MapToDto(Product product)
        {
            // We need the category name. If not included, fetch it.
            var category = product.Category ?? await _context.Categories.FindAsync(product.CategoryId);

            return new ProductResponseDto
            {
                Id = product.Id,
                CompanyId = product.CompanyId,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                Name = product.Name,
                Slug = product.Slug,
                ReferenceCode = product.ReferenceCode,
                Brand = product.Brand,
                Model = product.Model,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                Currency = product.Currency,
                ShowPrice = product.ShowPrice,
                Status = product.Status,
                IsFeatured = product.IsFeatured,
                IsVisible = product.IsVisible,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        private string GenerateUniqueSlug(string name, Guid companyId)
        {
            string slug = Regex.Replace(name.ToLower(), @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

            int count = 1;
            string currentSlug = slug;

            while (_context.Products.Any(p => p.CompanyId == companyId && p.Slug == currentSlug))
            {
                count++;
                currentSlug = $"{slug}-{count}";
            }

            return currentSlug;
        }
    }
}
