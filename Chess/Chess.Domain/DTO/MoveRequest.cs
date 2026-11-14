using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DTO
{
    public class MoveRequest
    {
        public string Token { get; set; }
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
        public string Role { get; set; }
        public bool IsMate { get; set; }
    }
}
