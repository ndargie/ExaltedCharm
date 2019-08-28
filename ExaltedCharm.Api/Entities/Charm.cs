using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ExaltedCharm.Api.Entities
{
    public class Charm : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(800)]
        public string Description { get; set; }
        [ForeignKey("CharmTypeId")]
        public CharmType CharmType { get; set; }
        public int CharmTypeId { get; set; }
        [ForeignKey("AbilityId")]
        public Ability Ability { get; set; }
        public int? AbilityId { get; set; }
        public int MoteCost { get; set; }
        public int WillpowerCost { get; set; }
        public int HealthCost { get; set; }
        public int EssanseRequirement { get; set; }
        public ICollection<KeywordCharm> Keywords { get; set; } = new List<KeywordCharm>();
        [ForeignKey("ExaltedTypeId")]
        public ExaltedType ExaltedType { get; set; }
        public int ExaltedTypeId { get; set; }
        [ForeignKey("DurationId")]
        public Duration Duration { get; set; }
        public int DurationId { get; set; }
        public void AddKeyword(Keyword keyword)
        {
            if (Keywords.All(x => x.KeywordId != keyword.Id))
            {
                Keywords.Add(new KeywordCharm() {Charm = this, Keyword = keyword});
            }
        }

        public void RemoveKeyword(Keyword keyword)
        {
            if (Keywords.Any(x => x.KeywordId != keyword.Id))
            {
                Keywords.Remove(Keywords.Single(x => x.KeywordId == keyword.Id));
            }
        }
    }
}
