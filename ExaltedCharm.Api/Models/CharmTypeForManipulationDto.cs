using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class CharmTypeForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name must be less than 50 characters")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "Description must be less than 200 characters")]
        public virtual string Description { get; set; }
    }
}