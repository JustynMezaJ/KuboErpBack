using Microsoft.AspNetCore.Mvc;
using Antonelly.Catalog.Api.Common;

namespace Antonelly.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public HealthController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Antonelly Catalog API is running",
                Data = new
                {
                    Service = "Antonelly.Catalog.Api",
                    Environment = _env.EnvironmentName
                }
            });
        }
    }
}
