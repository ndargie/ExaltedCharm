using System.Collections.Generic;

namespace ExaltedCharm.Api.Models
{
    public class ExaltedTypeDto : ExaltedTypeWithoutCastesDto
    {
         public List<CasteDto> Castes { get; set; } = new List<CasteDto>();
    }
}