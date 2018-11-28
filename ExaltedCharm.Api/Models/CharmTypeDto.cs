using System.Collections.Generic;

namespace ExaltedCharm.Api.Models
{
    public class CharmTypeDto : LinkedResourceBaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int NumberOfCharms => Charms.Count;
        public List<CharmDto> Charms { get; set; } = new List<CharmDto>();
    
    }
}
