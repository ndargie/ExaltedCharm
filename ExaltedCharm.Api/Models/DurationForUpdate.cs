using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class DurationForUpdate : DurationForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a description")]
        public override string Description
        {
            get => base.Description;
            set => base.Description = value;
        }
    }
}