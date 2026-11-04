using Chess.Domain.Entities;
using Chess.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;

namespace Chess.Domain.Helpers
{
    public static class ChessHelper
    {
        public static King FindMyKing(in IChess[,] matrix)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (matrix[i, j].IsMy && matrix[i, j].ChessType == CMT.King)
                    {
                        return matrix[i, j] as King;
                    }
                }
            return null;
        }
    }
}
