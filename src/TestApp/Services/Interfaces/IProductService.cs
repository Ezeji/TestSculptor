using TestApp.Models.DTO;

namespace TestApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<string> CreateProductAsync(ProductDTO productDTO);

        Task<ProductDTO> GetProductByIdAsync(int productId);

        Task<string> UpdateProductAsync(ProductUpdateDTO productUpdateDTO);

        Task<string> DeleteProductAsync(int productId);
    }
}
