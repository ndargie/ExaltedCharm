namespace ExaltedCharm.Api.Helpers
{
    public class AbilityResourceParameter : ResourceParameters
    {
        public AbilityResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}