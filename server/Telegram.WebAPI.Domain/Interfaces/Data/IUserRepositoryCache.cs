using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Interfaces;

public interface IUserRepositoryCache
{
    Task<User> GetUserByTelegramIdAsync(long id);
    void SetUserToCacheAsync(User user);
}
