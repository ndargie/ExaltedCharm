namespace ExaltedCharm.Api.Helpers
{
    public class CharmTypeResourceParameter : ResourceParameters
    {
        public CharmTypeResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}