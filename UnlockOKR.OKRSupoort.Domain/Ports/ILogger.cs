using UnlockOKR.OKRSupoort.Domain.Common;

namespace UnlockOKR.OKRSupoort.Domain.Ports
{
    public interface ILogger
    {
        void LoggingInfo(string controller, string method, MessageType messageType, string message);
    }
}
