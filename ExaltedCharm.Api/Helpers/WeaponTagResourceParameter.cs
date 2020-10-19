namespace ExaltedCharm.Api.Helpers
{
    public class WeaponTagResourceParameter : ResourceParameters
    {
        public WeaponTagResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}