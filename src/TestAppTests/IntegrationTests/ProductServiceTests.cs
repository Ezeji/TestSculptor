using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Models.DTO;
using TestApp.Services.Interfaces;
using TestApp.Models;
using TestApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TestAppTests.IntegrationTests
{
    [Collection(nameof(TestAppSharedFixture))]
    public class ProductServiceTests
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IProductService _productService;

        public ProductServiceTests(TestAppSharedFixture fixture)
        {
            _repository = fixture.ServiceProvider.GetRequiredService<IGenericRepository<Product>>();
            _productService = fixture.ServiceProvider.GetRequiredService<IProductService>();
        }

        [Fact]
        public async Task CreateProductAsync_Should_Create_Product_If_Parameter_Is_Not_Null()
        {
            // Arrange
            ProductDTO productDTO = new()
            {
                Name = "Milk",
                Price = 10
            };

            // Act
            string result = await _productService.CreateProductAsync(productDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual("Parameter empty", result);
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Product_If_Product_Is_Found()
        {
            // Arrange
            string result = await CreateProductAsync();
            int convertedResult = Convert.ToInt32(result);

            // Act
            ProductDTO productDTO = await _productService.GetProductByIdAsync(convertedResult);

            // Assert
            Assert.NotNull(productDTO);
            Assert.Equal("Milk", productDTO.Name);
            Assert.Equal(10, productDTO.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_Should_Update_Product_If_Product_Is_Found()
        {
            // Arrange
            string result = await CreateProductAsync();
            int convertedResult = Convert.ToInt32(result);

            ProductUpdateDTO productUpdateDTO = new()
            {
                Id = convertedResult,
                Name = "Sugar",
                Price = 20
            };

            // Act
            string updateResult = await _productService.UpdateProductAsync(productUpdateDTO);

            // Assert
            Assert.NotNull(updateResult);
            Assert.NotEqual("Parameter empty", result);
            Assert.NotEqual("Product not found", result);

            // Get recently updated product from db
            Product? product = await _repository.Query()
                                                .OrderByDescending(product => product.DateUpdated)
                                                .FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(product);
            Assert.Equal(product.Name, productUpdateDTO.Name);
            Assert.Equal(product.Price, productUpdateDTO.Price);
        }

        [Fact]
        public async Task DeleteProductAsync_Should_Delete_Product_If_Product_Is_Found()
        {
            // Arrange
            string result = await CreateProductAsync();
            int convertedResult = Convert.ToInt32(result);

            // Act
            string deleteResult = await _productService.DeleteProductAsync(convertedResult);

            // Assert
            Assert.NotNull(deleteResult);
            Assert.NotEqual("Product not found", result);
        }

        private async Task<string> CreateProductAsync()
        {
            ProductDTO productDTO = new()
            {
                Name = "Milk",
                Price = 10
            };

            string result = await _productService.CreateProductAsync(productDTO);

            return result;
        }
    }
}