namespace ProtekTiv.Core.Interfaces.Caching.MemoryCache;

public interface ICacheProvider
{
    void Insert<T>(string key, T value, DateTimeOffset expirationDateTime, CancellationToken? evictionToken = null);
    void EvictAll();
    T Get<T>(string key);

    T GetOrCacheFromResult<T>(string key, Func<T> cacheFromResult, DateTimeOffset expirationDateTime,
        CancellationToken? evictionToken = null);

    Task<T> GetOrCacheFromResultAsync<T>(string key, Func<Task<T>> cacheFromResultAsync,
        DateTimeOffset expirationDateTime, CancellationToken? evictionToken = null);
}
