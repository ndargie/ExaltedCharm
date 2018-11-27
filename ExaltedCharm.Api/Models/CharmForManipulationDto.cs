using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public abstract class CharmForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name must be less than 50 characters")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters")]
        public string Name { get; set; }
        [MaxLength(600, ErrorMessage = "Description must be less than 600 characters")]
        public virtual string Description { get; set; }
        [Required(ErrorMessage = "Cost in Essancse is required")]
        public int MoteCost { get; set; }
        [Required(ErrorMessage = "Cost in Willpower is required")]
        public int WillpowerCost { get; set; }
        [Required(ErrorMessage = "Cost in Health is required")]
        public int HealthCost { get; set; }
        [Required(ErrorMessage = "Essance Requirement is required")]
        public int EssanseRequirement { get; set; }
    }
}