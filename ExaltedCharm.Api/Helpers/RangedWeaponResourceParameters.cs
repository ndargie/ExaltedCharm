namespace ExaltedCharm.Api.Helpers
{
    public class RangedWeaponResourceParameters : ResourceParameters
    {
        public RangedWeaponResourceParameters()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
    }
}