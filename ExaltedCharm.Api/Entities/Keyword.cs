using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Entities
{
    public class Keyword : BaseEntity
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public ICollection<KeywordCharm> Charms { get; set; } = new List<KeywordCharm>();
    }
}