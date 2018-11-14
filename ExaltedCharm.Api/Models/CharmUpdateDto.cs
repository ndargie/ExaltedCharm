using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class CharmUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name must be less than 50 characters")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters")]
        public string Name { get; set; }
        [MaxLength(600, ErrorMessage = "Description must be less than 600 characters")]
        public string Description { get; set; }
    }
}