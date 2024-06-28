using ProtekTiv.Core.Interfaces.Logger.ISLogger;

namespace ProtekTiv.Core.Logger.ISLogger;

public class ISLogger : IISLogger
{
    public void LogAndThrowArgumentNullException(string message)
    {
        Serilog.Log.Error(message);
        throw new ArgumentNullException(message);
    }
}
