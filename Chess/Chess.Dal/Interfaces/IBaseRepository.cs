using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Dal.Interfaces
{
    public interface IBaseRepository<T>
    {
        public Task<bool> Create(T entity);
        public Task<bool> Update(T entity);
        public Task<bool> Delete(T entity);
        public Task<T> SelectById(long id);
        public Task<IEnumerable<T>> SelectAll();
    }
}
