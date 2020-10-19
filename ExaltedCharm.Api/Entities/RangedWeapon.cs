using System.Collections.Generic;

namespace ExaltedCharm.Api.Entities
{
    public class RangedWeapon : Weapon
    {
        public int Range { get; set; }
        public ICollection<RangedWeaponAmmo> AmmoTypes { get; set; } = new List<RangedWeaponAmmo>();
    }
}