using System.ComponentModel.DataAnnotations;
using ExaltedCharm.Api.Enums;

namespace ExaltedCharm.Api.Entities
{
    public class Attribute : BaseEntity
    { 
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        public AttributeType Type { get; set; }
    }
}