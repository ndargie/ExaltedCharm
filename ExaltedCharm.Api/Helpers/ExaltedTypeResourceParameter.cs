namespace ExaltedCharm.Api.Helpers
{
    public class ExaltedTypeResourceParameter : ResourceParameters
    {
        public ExaltedTypeResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}