using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestApp.Models.DTO;
using TestApp.Services.Interfaces;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PostProduct([FromBody] ProductDTO productDTO)
        {
            string response = await _productService.CreateProductAsync(productDTO);

            if (response == "Parameter empty")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetProductById([FromRoute] int productId)
        {
            ProductDTO response = await _productService.GetProductByIdAsync(productId);

            return Ok(response);
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PutProduct([FromBody] ProductUpdateDTO updateDTO)
        {
            string response = await _productService.UpdateProductAsync(updateDTO);

            if (response != "Product updated")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteProduct([FromRoute] int productId)
        {
            string response = await _productService.DeleteProductAsync(productId);

            if (response != "Product deleted")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
