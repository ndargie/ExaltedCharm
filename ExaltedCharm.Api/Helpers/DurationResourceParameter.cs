namespace ExaltedCharm.Api.Helpers
{
    public class DurationResourceParameter : ResourceParameters
    {
        public DurationResourceParameter()
        {
            OrderBy = "Name";
        }
        public string Name { get; set; }
    }
}