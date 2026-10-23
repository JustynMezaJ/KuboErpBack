namespace Antonelly.Catalog.Api.DTOs.ProductImages
{
    public class UpdateProductImageOrderRequestDto
    {
        public List<ProductImageOrderItemDto> Images { get; set; } = new();
    }
}
