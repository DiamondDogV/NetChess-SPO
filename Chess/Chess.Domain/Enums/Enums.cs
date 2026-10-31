using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.Enums
{
    public class Enums
    {
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
