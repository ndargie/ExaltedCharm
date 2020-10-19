using System.ComponentModel.DataAnnotations.Schema;

namespace ExaltedCharm.Api.Entities
{
    public class WeaponWeaponTag
    {
        public int WeaponId { get; set; }
        [ForeignKey("WeaponId")]
        public Weapon Weapon { get; set; }
        public int WeaponTagId { get; set; }
        public WeaponTag WeaponTag { get; set; }
    }
}