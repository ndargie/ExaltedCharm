namespace ExaltedCharm.Api.Models
{
    public class ExaltedTypeWithoutCastesDto : LinkedResourceBaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NecromancyLimit { get; set; }
        public string SorceryLimit { get; set; }
    }
}