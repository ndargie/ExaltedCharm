using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public abstract class DurationForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(50, ErrorMessage = "The name shouldn't be more than 50 characters")]
        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = "The description shouldn't be more than 200 characters")]
        public virtual string Description { get; set; }
    }
}