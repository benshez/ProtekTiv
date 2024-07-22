namespace ProtekTiv.Core.Interfaces.Services;

public interface IApiService<T>
{
    Task<T> GetAsync(string url);
    Task<T> GetAllAsync<TRequest>(IEnumerable<TRequest> request);
    Task<T> PostAsync(string url, T item);
    Task<T> PutAsync(string url, T item);
    Task DeleteAsync(string url);
}