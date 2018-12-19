namespace ExaltedCharm.Api.Models
{
    public class CasteDto : LinkedResourceBaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ExaltedTypeId { get; set; }
    }
}