using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Data
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Remove(T entity);
        void Update(T entity);
    }
}