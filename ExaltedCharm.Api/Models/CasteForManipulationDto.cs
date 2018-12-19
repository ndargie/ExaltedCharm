using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class CasteForManipulationDto
    {
        [Required]
        [MaxLength(40, ErrorMessage = "Name must be less than 40 characters")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters long")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "Description must be less than 200 characters")]
        public virtual string Description { get; set; }
    }
}