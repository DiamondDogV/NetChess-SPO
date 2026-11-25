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

        //public string Special { get; set; }

        public string Token { get; set; }
        public long? OwnerUserId { get; set; }
        public long? GuestUserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastHeartbeat { get; set; }

        public bool IsWhite { get; set; }
        public bool IsWhiteTurn { get; set; }
        public bool IsMate { get; set; }

        public string? BoardState { get; set; }

        // SignalR connection ids
        public string? WhiteConnectionId { get; set; }
        public string? BlackConnectionId { get; set; }

    }
}