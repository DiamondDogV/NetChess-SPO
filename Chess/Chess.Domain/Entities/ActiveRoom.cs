using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.Entities
{
    public class ActiveRoom
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Elo { get; set; }

        public string Special { get; set; }
    }
}
