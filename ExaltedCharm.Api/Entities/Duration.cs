using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Entities
{
    public class Duration : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public ICollection<Charm> Charms { get; set; } = new List<Charm>();
    }
}