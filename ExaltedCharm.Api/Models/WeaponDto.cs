using System.Collections.Generic;

namespace ExaltedCharm.Api.Models
{
    public class WeaponDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public int Accuracy { get; set; }
        public int Damage { get; set; }
        public string DamageType { get; set; }
        public List<WeaponTagDto> Tags { get; set; }
    }
}