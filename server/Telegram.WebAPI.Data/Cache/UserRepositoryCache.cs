using System;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Telegram.WebAPI.Data.Cache
{
    public class UserRepositoryCache : IUserRepositoryCache
    {
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;
        public UserRepositoryCache(IUserRepository userRepository, IMemoryCache cache)
        {
            _cache = cache;
            _userRepository = userRepository;
            _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
        }

        public async Task<User> GetUserByTelegramIdAsync(long telegramId)
        {
            string cacheKey = GenerateCacheKeyByUser(telegramId);

            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                return cachedUser;
            }

            var userFromDatabase = await _userRepository.GetUserByTelegramIdAsync(telegramId);
            _cache.Set(cacheKey, userFromDatabase, _memoryCacheEntryOptions);
            return userFromDatabase;
        }

        public void SetUserToCacheAsync(User user)
        {
            var cacheKey = GenerateCacheKeyByUser(user.TelegramChatId);
            _cache.Set(cacheKey, user, _memoryCacheEntryOptions);
        }

        private string GenerateCacheKeyByUser(long telegramId)
        {
            return $"usertelegramid-{telegramId.ToString()}";
        }
    }
}
