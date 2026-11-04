using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.Enums
{
    public class Enums
    {
        public enum CMT
        {
            None = 1 << 0,
            Pawn = 1 << 1,
            Roock = 1 << 2,
            Knight = 1 << 3,
            Slon = 1 << 4,
            Ferz = 1 << 5,
            King = 1 << 6
        }

        public enum MovesType
        {
            None,
            Move,
            Beat
        }

        public enum ResponceCodes
        {
            Ok = 200,
            NullTable = 305,
            BoolError = 310,
            UserExist = 315,
            UserDoesntExist = 316,
            WrongPassOrEmail = 320,
            UnCatchedError = 601,
            InvalidParameter = 602
        }
    }
}
