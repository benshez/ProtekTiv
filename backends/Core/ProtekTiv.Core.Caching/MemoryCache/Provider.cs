using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ProtekTiv.Core.Interfaces.Caching.MemoryCache;

namespace ProtekTiv.Core.Caching.MemoryCache;
public class Provider(IMemoryCache memoryCache) : ICacheProvider
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private CancellationTokenSource _evictionTokenSource = new();

    public void Insert<T>(
        string key,
        T value,
        DateTimeOffset expirationDateTime,
        CancellationToken? evictionToken = null)
    {
        _memoryCache?.Set(
            key,
            value,
            new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(evictionToken ?? _evictionTokenSource.Token))
                .SetAbsoluteExpiration(expirationDateTime)
        );
    }

    /// <summary>
    ///     Does not evict any cache entries with non-default eviction token.
    /// </summary>
    public void EvictAll()
    {
        _evictionTokenSource.Cancel();
        _evictionTokenSource = new CancellationTokenSource();
    }

    public T Get<T>(string key)
    {
        return _memoryCache.Get<T>(key) ?? default!;
    }

    public T GetOrCacheFromResult<T>(
        string key,
        Func<T> resultQueryHandler,
        DateTimeOffset expirationDateTime,
        CancellationToken? evictionToken = null)
    {
        var result = Get<T>(key);
        if (result == null)
        {
            result = resultQueryHandler();
            Insert(key, result, expirationDateTime, evictionToken);
        }

        return result;
    }

    public async Task<T> GetOrCacheFromResultAsync<T>(string key, Func<Task<T>> resultQueryHandlerAsync, DateTimeOffset expirationDateTime,
        CancellationToken? evictionToken = null)
    {
        var result = Get<T>(key);
        if (result == null)
        {
            result = await resultQueryHandlerAsync();
            Insert(key, result, expirationDateTime, evictionToken);
        }

        return result;
    }
}