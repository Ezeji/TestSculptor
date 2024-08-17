using System.ComponentModel.DataAnnotations;

namespace TestApp.Models.DTO
{
    public class ProductDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }
    }

    public class ProductUpdateDTO : ProductDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
