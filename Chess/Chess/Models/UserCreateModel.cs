using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class UserCreateModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Неверный формат логина")]
        [MaxLength(45, ErrorMessage = "Слишком длинное имя")]
        [MinLength(3, ErrorMessage = "Слишком короткое имя")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Неверный формат Email")]
        [StringLength(45, ErrorMessage = "Неккоректная длина Email", MinimumLength = 3)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Неверный формат пароля")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Неверный формат отображаемого имени")]
        public string DisplayName { get; set; }
    }
}
