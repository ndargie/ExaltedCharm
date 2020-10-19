namespace ExaltedCharm.Api.Helpers
{
    public class WeaponResourceParameter : ResourceParameters
    {
        public WeaponResourceParameter()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}