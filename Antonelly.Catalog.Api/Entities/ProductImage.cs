namespace Antonelly.Catalog.Api.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid ProductId { get; set; }
        public string PublicId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
        public string OptimizedUrl { get; set; } = string.Empty;
        public string? OriginalFilename { get; set; }
        public string? Format { get; set; }
        public long Bytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public string? AltText { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public Company Company { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
