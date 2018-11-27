using System.ComponentModel.DataAnnotations;

namespace ExaltedCharm.Api.Models
{
    public class KeywordUpdateDto : KeywordForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a description")]
        public override  string Description
        {
            get => base.Description;
            set => base.Description = value;
        }
    }
}