namespace ExaltedCharm.Api.Helpers
{
    public class KeywordResourceParameter : ResourceParameters
    {
        public KeywordResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}