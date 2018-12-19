namespace ExaltedCharm.Api.Helpers
{
    public class CasteResourceParameter : ResourceParameters
    {
        public CasteResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}