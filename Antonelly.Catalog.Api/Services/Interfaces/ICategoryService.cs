using Antonelly.Catalog.Api.DTOs.Categories;

namespace Antonelly.Catalog.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryAdminDto>> GetListCategoriesAsync(string companySlug);
    }
}
