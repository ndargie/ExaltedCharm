using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Entities
{
    public class ExaltedType : BaseEntity
    {
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string NecromancyLimit { get; set; }
        [MaxLength(100)]
        public string SorceryLimit { get; set; }
        public ICollection<Caste> Castes { get; set; }
    }
}