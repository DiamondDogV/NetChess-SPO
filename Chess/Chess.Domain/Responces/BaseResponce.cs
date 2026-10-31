using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.Domain.Enums.Enums;

namespace Chess.Domain.Responces
{
    public class BaseResponce<T> : IBaseResponce<T>
    {
        public T Data { get; set; }
        public string Responce { get; set; }
        public ResponceCodes Code { get; set; }
    }

    public interface IBaseResponce<T>
    {
        public T Data { get; set; }
        public string Responce { get; set; }
        public ResponceCodes Code { get; set; }

    }
}
