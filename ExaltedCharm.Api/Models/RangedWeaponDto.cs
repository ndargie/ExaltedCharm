using System.Collections.Generic;

namespace ExaltedCharm.Api.Models
{
    public class RangedWeaponDto
    {
        public int Range { get; set; }
        public ICollection<RangedWeaponAmmoDto> AmmoTypes { get; set; } = new List<RangedWeaponAmmoDto>();
    }
}