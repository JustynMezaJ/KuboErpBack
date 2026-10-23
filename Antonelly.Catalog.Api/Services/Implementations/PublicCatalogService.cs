using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Data;
using Antonelly.Catalog.Api.DTOs.PublicCatalog;
using Antonelly.Catalog.Api.Entities;
using Antonelly.Catalog.Api.Enums;
using Antonelly.Catalog.Api.Helpers;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Services.Implementations
{
    public class PublicCatalogService : IPublicCatalogService
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryUrlHelper _cloudinaryUrl;

        public PublicCatalogService(AppDbContext context, CloudinaryUrlHelper cloudinaryUrl)
        {
            _context = context;
            _cloudinaryUrl = cloudinaryUrl;
        }

        public async Task<PublicCompanyDto?> GetCompanyAsync(string companySlug)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == companySlug && c.IsActive);

            if (company == null) return null;

            return MapCompanyDto(company);
        }

        public async Task<IEnumerable<PublicCategoryDto>> GetCategoriesAsync(string companySlug)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Company.Slug == companySlug && c.IsActive && c.Company.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new PublicCategoryDto
                {
                    Name = c.Name,
                    Slug = c.Slug,
                    DisplayOrder = c.DisplayOrder
                })
                .ToListAsync();

            return categories;
        }

        public async Task<PublicCatalogHomeResponseDto?> GetHomeAsync(string companySlug)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == companySlug && c.IsActive);

            if (company == null) return null;

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CompanyId == company.Id && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new PublicCategoryDto
                {
                    Name = c.Name,
                    Slug = c.Slug,
                    DisplayOrder = c.DisplayOrder
                })
                .ToListAsync();

            var baseQuery = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.CompanyId == company.Id
                    && p.IsVisible
                    && p.Status != ProductStatus.Hidden
                    && p.Category.IsActive
                    && p.Images.Any(i => i.IsMain));

            var featuredProducts = await baseQuery
                .Where(p => p.IsFeatured)
                .OrderBy(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var newArrivals = await baseQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            var whatsappNumber = company.WhatsappNumber;

            return new PublicCatalogHomeResponseDto
            {
                Company = MapCompanyDto(company),
                Hero = new PublicHeroDto
                {
                    Title = "Timeless by Design",
                    Subtitle = "Piezas seleccionadas para quienes entienden el valor del tiempo.",
                    PrimaryButtonText = "Ver colección",
                    SecondaryButtonText = "Asesoría privada"
                },
                TrustBenefits = new List<PublicTrustBenefitDto>
                {
                    new() { Title = "Autenticidad verificada", Description = "Cada pieza es inspeccionada y certificada por nuestros expertos." },
                    new() { Title = "Asesoría privada", Description = "Atención personalizada para ayudarte a encontrar la pieza ideal." },
                    new() { Title = "Entrega segura", Description = "Envíos protegidos con seguro y rastreo en tiempo real." },
                    new() { Title = "Piezas seleccionadas", Description = "Solo trabajamos con las mejores marcas y modelos del mercado." }
                },
                Categories = categories,
                FeaturedProducts = featuredProducts.Select(p => MapToCardDto(p, whatsappNumber)).ToList(),
                NewArrivals = newArrivals.Select(p => MapToCardDto(p, whatsappNumber)).ToList(),
                Whatsapp = MapWhatsappDto(company, null)
            };
        }

        public async Task<PagedResponseDto<PublicProductCardDto>> GetListProductsAsync(GetListPublicProductsRequestDto request)
        {
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1) request.PageSize = 1;
            if (request.PageSize > 48) request.PageSize = 48;

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == request.CompanySlug && c.IsActive);

            if (company == null)
            {
                return new PagedResponseDto<PublicProductCardDto>
                {
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }

            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.CompanyId == company.Id
                    && p.IsVisible
                    && p.Status != ProductStatus.Hidden
                    && p.Category.IsActive
                    && p.Images.Any(i => i.IsMain));

            if (!string.IsNullOrWhiteSpace(request.CategorySlug))
                query = query.Where(p => p.Category.Slug == request.CategorySlug);

            if (request.Status.HasValue)
                query = query.Where(p => p.Status == request.Status.Value);

            if (request.OnlyFeatured == true)
                query = query.Where(p => p.IsFeatured);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.ReferenceCode.ToLower().Contains(search) ||
                    (p.Brand != null && p.Brand.ToLower().Contains(search)) ||
                    (p.Model != null && p.Model.ToLower().Contains(search)) ||
                    (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "nameasc" => query.OrderBy(p => p.Name),
                "namedesc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

            var products = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var whatsappNumber = company.WhatsappNumber;

            return new PagedResponseDto<PublicProductCardDto>
            {
                Items = products.Select(p => MapToCardDto(p, whatsappNumber)).ToList(),
                TotalItems = totalItems,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<PublicProductDetailDto?> FindProductBySlugAsync(string companySlug, string productSlug)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == companySlug && c.IsActive);

            if (company == null) return null;

            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p =>
                    p.CompanyId == company.Id &&
                    p.Slug == productSlug &&
                    p.IsVisible &&
                    p.Status != ProductStatus.Hidden &&
                    p.Category.IsActive &&
                    p.Images.Any(i => i.IsMain));

            if (product == null) return null;

            var mainImage = product.Images.First(i => i.IsMain);
            var galleryImages = product.Images.OrderBy(i => i.DisplayOrder).ToList();

            var relatedProducts = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p =>
                    p.Id != product.Id &&
                    p.CompanyId == company.Id &&
                    p.CategoryId == product.CategoryId &&
                    p.IsVisible &&
                    p.Status != ProductStatus.Hidden &&
                    p.Category.IsActive &&
                    p.Images.Any(i => i.IsMain))
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();

            var whatsappNumber = company.WhatsappNumber;

            return new PublicProductDetailDto
            {
                Name = product.Name,
                Slug = product.Slug,
                ReferenceCode = product.ReferenceCode,
                Brand = product.Brand,
                Model = product.Model,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                CategoryName = product.Category.Name,
                CategorySlug = product.Category.Slug,
                Status = product.Status.ToString(),
                StatusLabel = GetStatusLabel(product.Status),
                ShowPrice = product.ShowPrice,
                Price = product.ShowPrice ? product.Price : null,
                Currency = product.Currency,
                PriceText = GetPriceText(product),
                MainImage = MapToImageDto(mainImage),
                GalleryImages = galleryImages.Select(MapToImageDto).ToList(),
                Whatsapp = MapWhatsappDto(company, product),
                RelatedProducts = relatedProducts.Select(p => MapToCardDto(p, whatsappNumber)).ToList(),
                Seo = new PublicSeoDto
                {
                    MetaTitle = $"ANTONELLY | {product.Name}",
                    MetaDescription = product.ShortDescription ?? $"{product.Name} disponible en ANTONELLY. Pieza seleccionada con asesoría privada.",
                    OgImageUrl = _cloudinaryUrl.GetImageUrl(mainImage.PublicId, 1200, null, "limit")
                }
            };
        }

        private PublicCompanyDto MapCompanyDto(Company company)
        {
            return new PublicCompanyDto
            {
                Name = company.Name,
                Slug = company.Slug,
                LogoUrl = company.LogoUrl,
                WhatsappNumber = company.WhatsappNumber,
                InstagramUrl = company.InstagramUrl,
                IsActive = company.IsActive
            };
        }

        private PublicProductCardDto MapToCardDto(Product product, string? whatsappNumber)
        {
            var mainImage = product.Images.First(i => i.IsMain);

            return new PublicProductCardDto
            {
                Name = product.Name,
                Slug = product.Slug,
                ReferenceCode = product.ReferenceCode,
                Brand = product.Brand,
                Model = product.Model,
                ShortDescription = product.ShortDescription,
                CategoryName = product.Category.Name,
                CategorySlug = product.Category.Slug,
                Status = product.Status.ToString(),
                StatusLabel = GetStatusLabel(product.Status),
                ShowPrice = product.ShowPrice,
                Price = product.ShowPrice ? product.Price : null,
                Currency = product.Currency,
                PriceText = GetPriceText(product),
                MainImage = MapToImageDto(mainImage),
                WhatsappUrl = GenerateWhatsappUrl(whatsappNumber, product)
            };
        }

        private PublicProductImageDto MapToImageDto(ProductImage image)
        {
            return new PublicProductImageDto
            {
                AltText = image.AltText,
                ThumbnailUrl = _cloudinaryUrl.GetImageUrl(image.PublicId, 320, 320, "fill"),
                CardImageUrl = _cloudinaryUrl.GetImageUrl(image.PublicId, 700, 900, "fill"),
                DetailImageUrl = _cloudinaryUrl.GetImageUrl(image.PublicId, 1200, null, "limit"),
                ZoomImageUrl = _cloudinaryUrl.GetImageUrl(image.PublicId, 1800, null, "limit"),
                DisplayOrder = image.DisplayOrder,
                IsMain = image.IsMain
            };
        }

        private PublicWhatsappDto MapWhatsappDto(Company company, Product? product)
        {
            var message = product != null
                ? $"Hola, estoy interesado en este producto de ANTONELLY:\n\n{product.Name}\nReferencia: {product.ReferenceCode}\n\nLo vi en el catálogo y quiero recibir más información."
                : "Hola, quiero recibir asesoría privada sobre las piezas disponibles en ANTONELLY.";

            return new PublicWhatsappDto
            {
                Number = company.WhatsappNumber,
                Message = message,
                Url = !string.IsNullOrWhiteSpace(company.WhatsappNumber)
                    ? $"https://wa.me/{company.WhatsappNumber}?text={Uri.EscapeDataString(message)}"
                    : null
            };
        }

        private static string? GenerateWhatsappUrl(string? whatsappNumber, Product product)
        {
            if (string.IsNullOrWhiteSpace(whatsappNumber)) return null;

            var message = $"Hola, estoy interesado en este producto de ANTONELLY:\n\n{product.Name}\nReferencia: {product.ReferenceCode}\n\nLo vi en el catálogo y quiero recibir más información.";

            return $"https://wa.me/{whatsappNumber}?text={Uri.EscapeDataString(message)}";
        }

        private static string GetStatusLabel(ProductStatus status)
        {
            return status switch
            {
                ProductStatus.Available => "Disponible",
                ProductStatus.Reserved => "Reservado",
                ProductStatus.Sold => "Vendido",
                _ => "Disponible"
            };
        }

        private static string GetPriceText(Product product)
        {
            if (!product.ShowPrice || product.Price == null)
                return "Consultar precio";

            var formatted = ((long)product.Price.Value)
                .ToString("N0", System.Globalization.CultureInfo.InvariantCulture)
                .Replace(",", ".");

            return $"${formatted} {product.Currency}";
        }
    }
}
