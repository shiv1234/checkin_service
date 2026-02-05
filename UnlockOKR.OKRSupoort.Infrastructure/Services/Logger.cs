using Microsoft.Extensions.Logging;
using UnlockOKR.OKRSupoort.Domain.Common;
namespace UnlockOKR.OKRSupoort.Infrastructure.Services
{
    public class Logger : Domain.Ports.ILogger
    {
        private readonly ILogger<BaseService> _logger;
        public Logger(ILogger<BaseService> logger)
        {
            _logger = logger;
        }

        public void LoggingInfo(string controller, string method, MessageType messageType, string message)
        {
            var msg = messageType + " AuthService: " + " in controller: " + controller + " in method: " + method + " message: " + message;
            switch (messageType)
            {
                case MessageType.Info:
                    _logger.LogInformation(msg);
                    break;
                case MessageType.Warning:
                    _logger.LogWarning(msg);
                    break;
                default:
                    _logger.LogError(msg);
                    break;
            }
        }
    }
}
