using System.ComponentModel.DataAnnotations.Schema;

namespace ExaltedCharm.Api.Entities
{
    public class CasteAbility
    {
        [ForeignKey("CasteId")]
        public Caste Caste { get; set; }
        public int CasteId { get; set; }
        [ForeignKey("AbilityId")]
        public Ability Ability { get; set; }
        public int AbilityId { get; set; }

    }
}