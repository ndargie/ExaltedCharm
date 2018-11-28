namespace ExaltedCharm.Api.Models
{
    public class CharmTypeWithoutCharmsDto : LinkedResourceBaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}