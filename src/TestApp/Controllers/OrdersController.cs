using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestApp.Models.DTO;
using TestApp.Services.Interfaces;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PostOrder([FromBody] OrderDTO orderDTO)
        {
            string response = await _orderService.CreateOrderAsync(orderDTO);

            if (response == "Parameter empty")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
        {
            OrderDTO response = await _orderService.GetOrderByIdAsync(orderId);

            return Ok(response);
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PutOrder([FromBody] OrderUpdateDTO updateDTO)
        {
            string response = await _orderService.UpdateOrderAsync(updateDTO);

            if (response != "Order updated")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{orderId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteOrder([FromRoute] int orderId)
        {
            string response = await _orderService.DeleteOrderAsync(orderId);

            if (response != "Order deleted")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
