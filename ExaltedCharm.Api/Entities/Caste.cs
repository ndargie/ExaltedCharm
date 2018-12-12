using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaltedCharm.Api.Entities
{
    public class Caste : BaseEntity
    {
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
        public ICollection<CasteAbility> Abilities { get; set; } = new List<CasteAbility>();

        [ForeignKey("ExaltedTypeId")]
        public ExaltedType ExaltedType { get; set; }
        public int ExaltedTypeId { get; set; }
    }
}