using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Неверный формат Email")]
        [StringLength(45, ErrorMessage = "Неккоректная длина Email", MinimumLength = 3)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Неверный формат пароля")]
        public string Password { get; set; }
    }
}
