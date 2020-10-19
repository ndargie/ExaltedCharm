using System.ComponentModel.DataAnnotations.Schema;

namespace ExaltedCharm.Api.Entities
{
    public class RangedWeaponAmmo
    {
        public int RangedWeaponId { get; set; }
        [ForeignKey("RangedWeaponId")]
        public RangedWeapon RangedWeapon { get; set; }
        public int AmmoTypeId { get; set; }
        public AmmoType AmmoType { get; set; }
    }
}