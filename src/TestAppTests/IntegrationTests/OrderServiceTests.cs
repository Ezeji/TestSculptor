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
    public class OrderServiceTests
    {
        private readonly IGenericRepository<Order> _repository;
        private readonly IOrderService _orderService;

        public OrderServiceTests(TestAppSharedFixture fixture)
        {
            _repository = fixture.ServiceProvider.GetRequiredService<IGenericRepository<Order>>();
            _orderService = fixture.ServiceProvider.GetRequiredService<IOrderService>();
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Create_Order_If_Parameter_Is_Not_Null()
        {
            // Arrange
            OrderDTO orderDTO = new()
            {
               Name = "Milk",
               Price = 10
            };

            // Act
            string result = await _orderService.CreateOrderAsync(orderDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual("Parameter empty", result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Return_Order_If_Order_Is_Found()
        {
            // Arrange
            string result = await CreateOrderAsync();
            int convertedResult = Convert.ToInt32(result);

            // Act
            OrderDTO orderDTO = await _orderService.GetOrderByIdAsync(convertedResult);

            // Assert
            Assert.NotNull(orderDTO);
            Assert.Equal("Milk", orderDTO.Name);
            Assert.Equal(10, orderDTO.Price);
        }

        [Fact]
        public async Task UpdateOrderAsync_Should_Update_Order_If_Order_Is_Found()
        {
            // Arrange
            string result = await CreateOrderAsync();
            int convertedResult = Convert.ToInt32(result);

            OrderUpdateDTO orderUpdateDTO = new()
            {
               Id = convertedResult,
               Name = "Sugar",
               Price = 20
            };

            // Act
            string updateResult = await _orderService.UpdateOrderAsync(orderUpdateDTO);

            // Assert
            Assert.NotNull(updateResult);
            Assert.NotEqual("Parameter empty", result);
            Assert.NotEqual("Order not found", result);

            // Get recently updated order from db
            Order? order = await _repository.Query()
                                            .OrderByDescending(order => order.DateUpdated)
                                            .FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(order);
            Assert.Equal(order.Name, orderUpdateDTO.Name);
            Assert.Equal(order.Price, orderUpdateDTO.Price);
        }

        [Fact]
        public async Task DeleteOrderAsync_Should_Delete_Order_If_Order_Is_Found()
        {
            // Arrange
            string result = await CreateOrderAsync();
            int convertedResult = Convert.ToInt32(result);

            // Act
            string deleteResult = await _orderService.DeleteOrderAsync(convertedResult);

            // Assert
            Assert.NotNull(deleteResult);
            Assert.NotEqual("Order not found", result);
        }

        private async Task<string> CreateOrderAsync()
        {
            OrderDTO orderDTO = new()
            {
               Name = "Milk",
               Price = 10
            };

            string result = await _orderService.CreateOrderAsync(orderDTO);

            return result;
        }
    }
}