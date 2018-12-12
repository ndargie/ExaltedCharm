using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class ExaltedTypeForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(40, ErrorMessage = "Name must be less than 50 characters")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters")]
        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = "Description must be less than 200 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "NecromancyLimit is required")]
        [Range(0, 4, ErrorMessage = "Range between 0 and 4 for Necromancy")]
        
        public int NecromancyLimit { get; set; }
        [Range(0, 4, ErrorMessage = "Range between 0 and 4 for Sorcery")]
        public int SorceryLimit { get; set; }
    }
}