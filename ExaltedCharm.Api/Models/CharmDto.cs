using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExaltedCharm.Api.Models
{
    public class CharmDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
    }
}
