using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class ActiveRoomModel
    {
        [Required]
        [MaxLength(45,ErrorMessage = "Слишком длинное имя комнаты")]
        [MinLength(3, ErrorMessage = "Слишком короткое имя комнаты")]
        public string Name { get; set; }
        public int Elo { get; set; }
    }
}
