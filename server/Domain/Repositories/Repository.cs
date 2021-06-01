using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.WebAPI.Data;

namespace Telegram.WebAPI.Domain.Repositories
{
    public class Repository<T> : IRepository<T> where T: class
    {
        protected readonly TelegramContext _context;
        public Repository(TelegramContext context)
        {
            _context = context;
        }
        public void Add(T entity) 
        {
            _context.Set<T>().Add(entity);
        }

        public void Remove(T entity) 
        {
            _context.Set<T>().Remove(entity);
        }
        public void Update(T entity) 
        {
            _context.Set<T>().Update(entity);
        }
    }
}
