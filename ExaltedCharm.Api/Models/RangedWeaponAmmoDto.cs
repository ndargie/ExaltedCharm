namespace ExaltedCharm.Api.Models
{
    public class RangedWeaponAmmoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public int Damage { get; set; }
        public string DamageType { get; set; }
    }
}