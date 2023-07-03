using System.ComponentModel.DataAnnotations;

namespace OneTimePasswordManager.Models
{
    public class ValidPassword
    {
        [Required]
        [RegularExpression("^[0-9]+$")]
        public string UserId { get; set; }

        [Required]
        [StringLength(40)]
        public string Password { get; set; }
    }
}
