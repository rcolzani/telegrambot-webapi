using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.WebAPI.Domain.Entities;
using Telegram.WebAPI.Domain.Repositories;

namespace Telegram.WebAPI.Data.Cache
{
    public class UserRepositoryCache
    {
        private readonly IDistributedCache _cache;
        private readonly UserRepository _userRepository;

        public UserRepositoryCache(UserRepository userRepository, IDistributedCache cache)
        {
            _cache = cache;
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            string cacheKey = $"userid-{id.ToString()}";
            var result = await _cache.GetAsync(cacheKey);

            if (result != null)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<User>(System.Text.Encoding.UTF8.GetString(result));

            var userFromDatabase = await _userRepository.GetUserByIdAsync(id);
            var bytesObject = System.Text.Encoding.UTF8.GetBytes( Newtonsoft.Json.JsonConvert.SerializeObject(userFromDatabase));
            await _cache.SetAsync(cacheKey, bytesObject, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)});
            return userFromDatabase;
        }

        public async Task<User> GetUserByTelegramIdAsync(long id, bool asNoTracking = false)
        {
            string cacheKey = $"usertelegramid-{id.ToString()}";
            var result = await _cache.GetAsync(cacheKey);

            if (result != null)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<User>(System.Text.Encoding.UTF8.GetString(result));

            var userFromDatabase = await _userRepository.GetUserByTelegramIdAsync(id);
            var bytesObject = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(userFromDatabase));
            await _cache.SetAsync(cacheKey, bytesObject, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
            return userFromDatabase;
        }


    }
}
