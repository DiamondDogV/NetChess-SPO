using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }

        public short EloRating { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
