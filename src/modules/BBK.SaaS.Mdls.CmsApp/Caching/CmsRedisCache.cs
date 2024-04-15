using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Data;
using Abp.Json;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Redis;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BBK.SaaS.Mdls.Cms.Caching
{
	public interface ICmsRedisCache : ICache
	{
		Task<long> IncrAsync(string key, long value = 1, DateTimeOffset? absoluteExpireTime = null);
		Task<IEnumerable<RedisKey>> GetKeys(string keyFilter);
		Task<ConditionalValue<object>> GetNumberValueAsync(string key);
		Task<TimeSpan> GetKeyTimeSpan(string key);
	}

	public class CmsRedisCache : AbpRedisCache, ICmsRedisCache
	{
		private const string CmsRedisCachePrefix = "CmsRedisCache:";

		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IDatabase _database;
		private readonly IRedisCacheSerializer _serializer;

		public CmsRedisCache(
			string name,
			IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider,
			IRedisCacheSerializer redisCacheSerializer,
			IHttpContextAccessor httpContextAccessor)
			: base(name, redisCacheDatabaseProvider, redisCacheSerializer)
		{
			_database = redisCacheDatabaseProvider.GetDatabase();
			_httpContextAccessor = httpContextAccessor;
			_serializer = redisCacheSerializer;
		}

		public override bool TryGetValue(string key, out object value)
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return base.TryGetValue(key, out value);
			}

			try
			{
				var localizedKey = GetPerRequestRedisCacheKey(key);

				if (httpContext.Items.ContainsKey(localizedKey))
				{
					var conditionalValue = (ConditionalValue<object>)httpContext.Items[localizedKey];
					value = conditionalValue.HasValue ? conditionalValue.Value : null;

					return conditionalValue.HasValue;
				}

				var hasValue = base.TryGetValue(key, out value);
				httpContext.Items[localizedKey] = new ConditionalValue<object>(hasValue, hasValue ? value : null);
				return hasValue;
			}
			catch (ObjectDisposedException exception)
			{
				Logger.Warn(exception.Message, exception);
				return base.TryGetValue(key, out value);
			}
		}

		public override ConditionalValue<object>[] TryGetValues(string[] keys)
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return base.TryGetValues(keys);
			}

			try
			{
				var localizedKeys = keys.ToDictionary(GetPerRequestRedisCacheKey);

				var missingKeys = localizedKeys
					.Where(kv => !httpContext.Items.ContainsKey(kv.Key))
					.Select(kv => kv.Value)
					.ToArray();

				var missingValues = base.TryGetValues(missingKeys);

				for (var i = 0; i < missingKeys.Length; i++)
				{
					httpContext.Items[GetPerRequestRedisCacheKey(missingKeys[i])] = missingValues[i];
				}

				return localizedKeys.Keys.Select(localizedKey => (ConditionalValue<object>)httpContext.Items[localizedKey]).ToArray();
			}
			catch (ObjectDisposedException exception)
			{
				Logger.Warn(exception.Message, exception);
				return base.TryGetValues(keys);
			}
		}

		public override async Task<ConditionalValue<object>> TryGetValueAsync(string key)
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return await base.TryGetValueAsync(key);
			}

			var localizedKey = GetPerRequestRedisCacheKey(key);

			try
			{
				if (httpContext.Items.ContainsKey(localizedKey))
				{
					var conditionalValue = (ConditionalValue<object>)httpContext.Items[localizedKey];
					return conditionalValue;
				}
				else
				{
					var conditionalValue = await base.TryGetValueAsync(key);
					httpContext.Items[localizedKey] = conditionalValue;
					return conditionalValue;
				}
			}
			catch (ObjectDisposedException exception)
			{
				Logger.Warn(exception.Message, exception);
				return await base.TryGetValueAsync(key);
			}
		}

		public override async Task<ConditionalValue<object>[]> TryGetValuesAsync(string[] keys)
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return await base.TryGetValuesAsync(keys);
			}

			try
			{
				var localizedKeys = keys.ToDictionary(GetPerRequestRedisCacheKey);

				var missingKeys = localizedKeys
					.Where(kv => !httpContext.Items.ContainsKey(kv.Key))
					.Select(kv => kv.Value)
					.ToArray();

				var missingValues = await base.TryGetValuesAsync(missingKeys);

				for (var i = 0; i < missingKeys.Length; i++)
				{
					httpContext.Items[GetPerRequestRedisCacheKey(missingKeys[i])] = missingValues[i];
				}

				return localizedKeys.Keys.Select(localizedKey => (ConditionalValue<object>)httpContext.Items[localizedKey]).ToArray();
			}
			catch (ObjectDisposedException exception)
			{
				Logger.Warn(exception.Message, exception);
				return await base.TryGetValuesAsync(keys);
			}
		}

		public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
		{
			base.Set(key, value, slidingExpireTime, absoluteExpireTime);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null)
			{
				httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, value);
			}
		}

		public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
		{
			await base.SetAsync(key, value, slidingExpireTime, absoluteExpireTime);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null)
			{
				httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, value);
			}
		}

		public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
		{
			base.Set(pairs, slidingExpireTime, absoluteExpireTime);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return;
			}

			for (var i = 0; i < pairs.Length; i++)
			{
				httpContext.Items[GetPerRequestRedisCacheKey(pairs[i].Key)] = new ConditionalValue<object>(true, pairs[i].Value);
			}
		}

		public override async Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
		{
			await base.SetAsync(pairs, slidingExpireTime, absoluteExpireTime);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null)
			{
				for (var i = 0; i < pairs.Length; i++)
				{
					httpContext.Items[GetPerRequestRedisCacheKey(pairs[i].Key)] = new ConditionalValue<object>(true, pairs[i].Value);
				}
			}
		}

		public override void Remove(string key)
		{
			base.Remove(key);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return;
			}

			var localizedKey = GetPerRequestRedisCacheKey(key);

			if (httpContext.Items.ContainsKey(localizedKey))
			{
				httpContext.Items.Remove(localizedKey);
			}
		}

		public override async Task RemoveAsync(string key)
		{
			await base.RemoveAsync(key);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null)
			{
				var localizedKey = GetPerRequestRedisCacheKey(key);

				if (httpContext.Items.ContainsKey(localizedKey))
				{
					httpContext.Items.Remove(localizedKey);
				}
			}
		}

		public override void Remove(string[] keys)
		{
			base.Remove(keys);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return;
			}

			foreach (var key in keys)
			{
				var localizedKey = GetPerRequestRedisCacheKey(key);

				if (httpContext.Items.ContainsKey(localizedKey))
				{
					httpContext.Items.Remove(localizedKey);
				}
			}
		}

		public override async Task RemoveAsync(string[] keys)
		{
			await base.RemoveAsync(keys);

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null)
			{
				foreach (var key in keys)
				{
					var localizedKey = GetPerRequestRedisCacheKey(key);

					if (httpContext.Items.ContainsKey(localizedKey))
					{
						httpContext.Items.Remove(localizedKey);
					}
				}
			}
		}

		public override void Clear()
		{
			base.Clear();
			//_database.key

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return;
			}

			var localizedKeyPrefix = GetPerRequestRedisCacheKey("");

			foreach (var key in httpContext.Items.Keys.OfType<string>().ToList())
			{
				if (key.StartsWith(localizedKeyPrefix))
				{
					httpContext.Items.Remove(key);
				}
			}
		}

		public async Task<IEnumerable<RedisKey>> GetKeys(string keyFilter)
		{
			var partern = $"{GetLocalizedRedisKey("*")}";
			var keys = (await _database.ExecuteAsync("SCAN", "0", "MATCH", partern));

			if (keys != null)
			{
				var valueFounds = keys.ToDictionary().ToArray();

				return ((RedisKey[])valueFounds[0].Value).ToList();
			}

			return null;
		}

		public async Task<long> IncrAsync(string key, long value = 1, DateTimeOffset? absoluteExpireTime = null)
		{
			//await _database.StringIncrement(key, value);

			if (value <= 0)
			{
				throw new AbpException("Can not insert null values to the cache!");
			}

			var redisKey = GetLocalizedRedisKey(key);
			long redisValue = 0;
			if (value == 1)
			{
				redisValue = await _database.StringIncrementAsync(redisKey);
			}
			else
			{
				//if (value == 0)

				redisValue = await _database.StringIncrementAsync(redisKey, value);
			}

			if (redisValue <= 0)
			{
				Logger.ErrorFormat("Unable to set key:{0} value:{1} asynchronously in Redis", redisKey, redisValue);
				return 0;
			}

			if (absoluteExpireTime.HasValue)
			{
				var a = await _database.KeyExpireAsync(redisKey, absoluteExpireTime.Value.UtcDateTime);
			}

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null)
			{
				httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, redisValue);
			}

			return redisValue;
		}

		protected virtual string GetPerRequestRedisCacheKey(string key)
		{
			return CmsRedisCachePrefix + GetLocalizedRedisKey(key).ToString();
		}

		public async Task<TimeSpan> GetKeyTimeSpan(string key)
		{
			var keyTime = await _database.KeyTimeToLiveAsync(GetLocalizedRedisKey(key));

			return keyTime ?? TimeSpan.Zero;
		}
		public async Task<ConditionalValue<object>> GetNumberValueAsync(string key)
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				var redisValue = await _database.StringGetAsync(GetLocalizedRedisKey(key));
				if (redisValue.HasValue)
				{
					//return long.Parse(redisValue);
					return new ConditionalValue<object>(true, long.Parse(redisValue));
				}
				//return await base.TryGetValueAsync(key);
			}

			var localizedKey = GetPerRequestRedisCacheKey(key);

			try
			{
				if (httpContext.Items.ContainsKey(localizedKey))
				{
					var conditionalValue = (ConditionalValue<object>)httpContext.Items[localizedKey];
					return conditionalValue;
				}
				else
				{
					//var conditionalValue = await base.TryGetValueAsync(key);
					var redisValue = await _database.StringGetAsync(GetLocalizedRedisKey(key));
					var conditionalValue = new ConditionalValue<object>(true, long.Parse(redisValue));

					httpContext.Items[localizedKey] = conditionalValue;
					return conditionalValue;
				}
			}
			catch (ObjectDisposedException exception)
			{
				Logger.Warn(exception.Message, exception);
				return await base.TryGetValueAsync(key);
			}
		}


	}
}
