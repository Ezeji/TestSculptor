using TestApp.Models.DTO;

namespace TestApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(OrderDTO orderDTO);

        Task<OrderDTO> GetOrderByIdAsync(int orderId);

        Task<string> UpdateOrderAsync(OrderUpdateDTO orderUpdateDTO);

        Task<string> DeleteOrderAsync(int orderId);
    }
}
