using Microsoft.Extensions.Options;
using Antonelly.Catalog.Api.Configuration;

namespace Antonelly.Catalog.Api.Helpers
{
    public class CloudinaryUrlHelper
    {
        private readonly CloudinarySettings _settings;

        public CloudinaryUrlHelper(IOptions<CloudinarySettings> settings)
        {
            _settings = settings.Value;
        }

        public string GetImageUrl(string publicId, int? width = null, int? height = null, string? crop = null)
        {
            var parts = new List<string> { "f_auto", "q_auto" };

            if (width.HasValue)
                parts.Add($"w_{width}");

            if (height.HasValue)
                parts.Add($"h_{height}");

            if (!string.IsNullOrEmpty(crop))
                parts.Add($"c_{crop}");

            var transformation = string.Join(",", parts);

            return $"https://res.cloudinary.com/{_settings.CloudName}/image/upload/{transformation}/{publicId}";
        }
    }
}
