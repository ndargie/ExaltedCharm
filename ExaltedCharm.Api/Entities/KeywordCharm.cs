using System.ComponentModel.DataAnnotations.Schema;

namespace ExaltedCharm.Api.Entities
{
    public class KeywordCharm
    {
        public int CharmId { get; set; }
        [ForeignKey("CharmId")]
        public Charm Charm { get; set; }
        public int KeywordId { get; set; }
        public Keyword Keyword { get; set; }
    }
}