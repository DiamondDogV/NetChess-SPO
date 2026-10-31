using Chess.Domain.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Service.Interfaces
{
    public interface IBaseService<T>
    {
        public Task<IBaseResponce<bool>> CreateAsync(T entity);
        public Task<IBaseResponce<bool>> UpdateAsync(T entity);
        public Task<IBaseResponce<bool>> RemoveAsync(T entity);

        public Task<IBaseResponce<T>> GetByIdAsync(long id);
        public Task<IBaseResponce<IEnumerable<T>>> GetAllAsync();
    }
}
