using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Entities
{
    public class Ability : BaseEntity
    {
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }
        
        [MaxLength(200)]
        public string Description { get; set; }

        public ICollection<Charm> Charms { get; set; }
        public ICollection<CasteAbility> Castes { get; set; } = new List<CasteAbility>();
    }
}