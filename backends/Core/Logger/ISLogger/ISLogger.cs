using ProtekTiv.Core.Interfaces.Logger.ISLogger;

namespace ProtekTiv.Logger.ISLogger;

public class ISLogger : IISLogger
{

    public void LogAndThrowArgumentNullException(string message)
    {
        Serilog.Log.Error(message);
        throw new ArgumentNullException(message);
    }
}
