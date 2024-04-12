using Newtonsoft.Json;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using StackExchange.Redis;


namespace OriginIAM.Infrastructure.Repositories
{
    public class EligibilityRepository : IEligibilityRepository
    {
        private readonly IDatabase _redisDatabase;

        public EligibilityRepository(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
        }

        public async Task<User> GetAndDeleteUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email address cannot be null or empty.", nameof(email));

            RedisKey key = email;

            //Upsert FromCache
            string jsonValue = await _redisDatabase.StringGetDeleteAsync(key);

            if (jsonValue == null)
            {
                return null;
            }

            User user = JsonConvert.DeserializeObject<User>(jsonValue);

            user.PasswordHash = password;

            return user;
        }

        public async Task<bool> DeleteUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email address cannot be null or empty.", nameof(email));

            RedisKey key = email;

            return await _redisDatabase.KeyDeleteAsync(key);
        }

        public Task SaveOrUpdateEligibleUser(User elegibleUser)
        {
            if (elegibleUser == null)
                throw new ArgumentNullException(nameof(elegibleUser));

            RedisKey key = elegibleUser.Email;

            string jsonValue = JsonConvert.SerializeObject(elegibleUser);

            _redisDatabase.StringSet(key, jsonValue);

            return Task.CompletedTask;
        }
    }
}
