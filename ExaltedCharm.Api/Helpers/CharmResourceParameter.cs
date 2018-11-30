namespace ExaltedCharm.Api.Helpers
{
    public class CharmResourceParameter : ResourceParameters
    {
        public CharmResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}