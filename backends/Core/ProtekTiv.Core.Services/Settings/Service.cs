using System.Configuration;

namespace ProtekTiv.Core.Services.Settings;

public class Service<T> where T : class
{
    public Provider<T> ProtekTivSettings { get; private set; }
    public Service()
    {
        ProtekTivSettings = new Provider<T>();
    }
}

public class Provider<T> where T : class
{
    public T? this[string key]
    {
        get
        {
            var settings = ConfigurationManager.AppSettings;
            if (settings == null)
            {
                var setting = settings?[key];
                if (setting != null)
                {
                    return (T)Convert.ChangeType(setting, typeof(T));
                }
                return null;
            }
            return null;
        }
    }
}
