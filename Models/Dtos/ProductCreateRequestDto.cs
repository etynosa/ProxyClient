using System.ComponentModel.DataAnnotations;

namespace Practice.Models.Dtos
{
    public class ProductCreateRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Dictionary<string, object> Data { get; set; }
    }
}
