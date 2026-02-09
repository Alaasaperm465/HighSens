using System.ComponentModel.DataAnnotations;

namespace HighSens.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;
    }
}
