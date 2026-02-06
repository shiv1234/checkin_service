using OneGuru.CFR.Domain.Common;

namespace OneGuru.CFR.Domain.Ports
{
    public interface ILogger
    {
        void LoggingInfo(string controller, string method, MessageType messageType, string message);
    }
}
