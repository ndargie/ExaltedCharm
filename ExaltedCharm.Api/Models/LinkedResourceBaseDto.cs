using System.Collections.Generic;

namespace ExaltedCharm.Api.Models
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}