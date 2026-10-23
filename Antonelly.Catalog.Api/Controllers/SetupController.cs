using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Common;
using Antonelly.Catalog.Api.Data;
using Antonelly.Catalog.Api.DTOs.Setup;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/setup")]
    public class SetupController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SetupController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("GetCompanyPublicInfo/{companySlug}")]
        public async Task<IActionResult> GetCompanyPublicInfo(string companySlug)
        {
            if (!_env.IsDevelopment())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Not found"
                });
            }

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == companySlug);

            if (company == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            return Ok(new ApiResponse<CompanyPublicInfoResponseDto>
            {
                Success = true,
                Message = "Company public info retrieved successfully",
                Data = new CompanyPublicInfoResponseDto
                {
                    Name = company.Name,
                    Slug = company.Slug,
                    WhatsappNumber = company.WhatsappNumber,
                    InstagramUrl = company.InstagramUrl,
                    LogoUrl = company.LogoUrl,
                    IsActive = company.IsActive
                }
            });
        }

        [HttpPut("UpdateCompanyPublicInfo/{companySlug}")]
        public async Task<IActionResult> UpdateCompanyPublicInfo(string companySlug, [FromBody] UpdateCompanyPublicInfoRequestDto request)
        {
            if (!_env.IsDevelopment())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Not found"
                });
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == companySlug);

            if (company == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            company.WhatsappNumber = request.WhatsappNumber;
            company.InstagramUrl = request.InstagramUrl;
            company.LogoUrl = request.LogoUrl;
            company.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<CompanyPublicInfoResponseDto>
            {
                Success = true,
                Message = "Company public info updated successfully",
                Data = new CompanyPublicInfoResponseDto
                {
                    Name = company.Name,
                    Slug = company.Slug,
                    WhatsappNumber = company.WhatsappNumber,
                    InstagramUrl = company.InstagramUrl,
                    LogoUrl = company.LogoUrl,
                    IsActive = company.IsActive
                }
            });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Categories retrieved",
                Data = categories
            });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Slug == "antonelly");

            if (company == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Initial company data not found in database"
                });
            }

            var categories = await _context.Categories
                .Where(c => c.CompanyId == company.Id)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => c.Name)
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Initial catalog data loaded",
                Data = new
                {
                    company = company.Name,
                    slug = company.Slug,
                    categories = categories
                }
            });
        }
    }
}
