using Microsoft.EntityFrameworkCore;
using System.Linq;
using TestApp.Models;
using TestApp.Models.DTO;
using TestApp.Repository.Interfaces;
using TestApp.Services.Interfaces;

namespace TestApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;

        public ProductService(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<string> CreateProductAsync(ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return "Parameter empty";
            }

            Product product = new()
            {
                Name = productDTO.Name,
                Price = productDTO.Price
            };

            await _repository.CreateAsync(product);

            //get the id of the recently created product
            int productId = await _repository.Query()
                                             .OrderByDescending(product => product.DateCreated)
                                             .Select(product => product.Id)
                                             .FirstOrDefaultAsync();

            return productId.ToString();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            Product? product = await _repository.GetByIdAsync(productId);

            if (product == null)
            {
                return new ProductDTO();
            }

            ProductDTO productDTO = new()
            {
                Name = product.Name,
                Price = product.Price
            };

            return productDTO;
        }

        public async Task<string> UpdateProductAsync(ProductUpdateDTO productUpdateDTO)
        {
            if (productUpdateDTO == null)
            {
                return "Parameter empty";
            }

            Product? product = await _repository.GetByIdAsync(productUpdateDTO.Id);

            if (product == null)
            {
                return "Product not found";
            }

            product.Name = productUpdateDTO.Name;
            product.Price = productUpdateDTO.Price;
            product.DateUpdated = DateTime.UtcNow;

            await _repository.SaveChangesToDbAsync();

            return "Product updated";
        }

        public async Task<string> DeleteProductAsync(int productId)
        {
            Product? product = await _repository.GetByIdAsync(productId);

            if (product == null)
            {
                return "Product not found";
            }

            await _repository.DeleteAsync(productId);

            return "Product deleted";
        }
    }
}
