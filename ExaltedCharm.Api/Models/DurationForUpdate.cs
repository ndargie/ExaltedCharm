using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class DurationForUpdate
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}