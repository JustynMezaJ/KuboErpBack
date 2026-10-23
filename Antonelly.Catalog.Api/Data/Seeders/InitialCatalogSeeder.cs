using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Data;
using Antonelly.Catalog.Api.Entities;

namespace Antonelly.Catalog.Api.Data.Seeders
{
    public static class InitialCatalogSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1. Seed Company
            var companySlug = "antonelly";
            var company = await context.Companies
                .FirstOrDefaultAsync(c => c.Slug == companySlug);

            if (company == null)
            {
                company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ANTONELLY",
                    Slug = companySlug,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                context.Companies.Add(company);
                await context.SaveChangesAsync();
            }

            // 2. Seed Categories
            var baseCategories = new List<(string Name, string Slug, int Order)>
            {
                ("Relojes", "relojes", 1),
                ("Joyas", "joyas", 2),
                ("Accesorios", "accesorios", 3)
            };

            foreach (var catData in baseCategories)
            {
                var categoryExists = await context.Categories
                    .AnyAsync(c => c.CompanyId == company.Id && c.Slug == catData.Slug);

                if (!categoryExists)
                {
                    context.Categories.Add(new Category
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = company.Id,
                        Name = catData.Name,
                        Slug = catData.Slug,
                        IsActive = true,
                        DisplayOrder = catData.Order,
                        CreatedAt = DateTimeOffset.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
