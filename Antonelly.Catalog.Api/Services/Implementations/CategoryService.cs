using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Data;
using Antonelly.Catalog.Api.DTOs.Categories;
using Antonelly.Catalog.Api.Services.Interfaces;

namespace Antonelly.Catalog.Api.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryAdminDto>> GetListCategoriesAsync(string companySlug)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Company.Slug == companySlug)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryAdminDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    IsActive = c.IsActive,
                    DisplayOrder = c.DisplayOrder
                })
                .ToListAsync();

            return categories;
        }
    }
}
