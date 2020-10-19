using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class WeaponTagForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(50, ErrorMessage = "The name shouldn't be more than 50 character")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "The description shouldn't be more than 200 characters")]
        public virtual string Description { get; set; }
    }
}