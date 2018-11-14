using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class SaveDurationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}