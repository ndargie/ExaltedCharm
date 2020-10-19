using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExaltedCharm.Api.Enums;

namespace ExaltedCharm.Api.Entities
{
    public abstract class Weapon : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
       
        [MaxLength(200)]
        public string Description { get; set; }
        public int Rate { get; set; }
        public int Accuracy { get; set; }
        public int Damage { get; set; }
        public DamageType DamageType { get; set; }
        public ICollection<WeaponWeaponTag> Tags { get; set; } = new List<WeaponWeaponTag>();
     
    }
}