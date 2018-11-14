using System.Collections;
using System.Collections.Generic;

namespace ExaltedCharm.Api.Entities
{
    public class ExaltedType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SorceryLimit { get; set; }
        public string NecromancyLimit { get; set; }
        public ICollection<Charm> Charms { get; set; } = new List<Charm>();
    }
}