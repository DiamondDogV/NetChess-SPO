using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;
using System.Drawing;

namespace Chess.Domain.Interfaces
{
    public interface IChess
    {
        CMT ChessType { get; set; }
        bool IsBlack { get; set; }

        byte Price { get; set; }
        bool IsMy { get; set; }
        Point Position { get; set; }

        bool IsFirstMove { get; set; }

        void ShowMoves(in IChess[,] matrix, out MovesType[,] movesMatrix);
        bool CanBeat(in IChess[,] matrix, Point to);
        bool CanMove(in IChess[,] matrix, Point to);
    }
}
