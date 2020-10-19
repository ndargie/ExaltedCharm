using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Entities
{
    public class WeaponTag : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public ICollection<WeaponWeaponTag> Weapons { get; set; } = new List<WeaponWeaponTag>();
    }
}