using System.Collections.Generic;
using ExaltedCharm.Api.Enums;

namespace ExaltedCharm.Api.Entities
{
    public class AmmoType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public int Damage { get; set; }
        public DamageType DamageType { get; set; }
        public ICollection<RangedWeaponAmmo> RangedWeapons { get; set; } = new List<RangedWeaponAmmo>();
    }
}