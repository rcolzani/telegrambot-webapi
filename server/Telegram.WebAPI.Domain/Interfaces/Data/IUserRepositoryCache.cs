using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces
{
    public interface IUserRepositoryCache
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByTelegramIdAsync(long id);

    }
}
