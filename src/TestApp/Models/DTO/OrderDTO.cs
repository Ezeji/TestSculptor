using System.ComponentModel.DataAnnotations;

namespace TestApp.Models.DTO
{
    public class OrderDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }
    }

    public class OrderUpdateDTO : OrderDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
