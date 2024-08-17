using TestApp.Models.DTO;
using TestApp.Models;
using TestApp.Repository.Interfaces;
using TestApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TestApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _repository;

        public OrderService(IGenericRepository<Order> repository)
        {
            _repository = repository;
        }

        public async Task<string> CreateOrderAsync(OrderDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return "Parameter empty";
            }

            Order order = new()
            {
               Name = orderDTO.Name,
               Price = orderDTO.Price
            };

            await _repository.CreateAsync(order);

            //get the id of the recently created order
            int orderId = await _repository.Query()
                                             .OrderByDescending(order => order.DateCreated)
                                             .Select(order => order.Id)
                                             .FirstOrDefaultAsync();

            return orderId.ToString();
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            Order? order = await _repository.GetByIdAsync(orderId);

            if (order == null)
            {
                return new OrderDTO();
            }

            OrderDTO orderDTO = new()
            {
               Name = order.Name,
               Price = order.Price
            };

            return orderDTO;
        }

        public async Task<string> UpdateOrderAsync(OrderUpdateDTO orderUpdateDTO)
        {
            if (orderUpdateDTO == null)
            {
                return "Parameter empty";
            }

            Order? order = await _repository.GetByIdAsync(orderUpdateDTO.Id);

            if (order == null)
            {
                return "Order not found";
            }

            order.Name = orderUpdateDTO.Name;
            order.Price = orderUpdateDTO.Price;
            order.DateUpdated = DateTime.UtcNow;

            await _repository.SaveChangesToDbAsync();

            return "Order updated";
        }

        public async Task<string> DeleteOrderAsync(int orderId)
        {
            Order? order = await _repository.GetByIdAsync(orderId);

            if (order == null)
            {
                return "Order not found";
            }

            await _repository.DeleteAsync(orderId);

            return "Order deleted";
        }
    }
}
