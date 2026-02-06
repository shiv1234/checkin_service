using Microsoft.Extensions.Logging;
using Moq;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Infrastructure.Services;
using System;
using Xunit;

namespace OneGuru.CFR.Infrastructure.Tests.Services
{
    public class LoggerTest
    {

        private readonly Mock<ILogger<BaseService>> _mockILoggerBaseService;
        public LoggerTest()
        {
            _mockILoggerBaseService = new Mock<ILogger<BaseService>>();
        }
        [Obsolete]
        public Logger ObjLogger()
        {
            return new Logger(_mockILoggerBaseService.Object);
        }
        #region NotificationsAndEmails
        [Fact]
        [Obsolete]
        public void Logger_MessageTypeInfo()
        {
            //Arrange
            var loggerObj = ObjLogger();
            //Act
            loggerObj.LoggingInfo("", "", MessageType.Info, "");
        }
        [Fact]
        [Obsolete]
        public void Logger_MessageTypeAlert()
        {
            //Arrange
            var loggerObj = ObjLogger();
            //Act
            loggerObj.LoggingInfo("", "", MessageType.Alert, "");
        }
        [Fact]
        [Obsolete]
        public void Logger_MessageTypeError()
        {
            //Arrange
            var loggerObj = ObjLogger();
            //Act
            loggerObj.LoggingInfo("", "", MessageType.Error, "");
        }
        [Fact]
        [Obsolete]
        public void Logger_MessageTypeSuccess()
        {
            //Arrange
            var loggerObj = ObjLogger();
            //Act
            loggerObj.LoggingInfo("", "", MessageType.Success, "");
        }
        [Fact]
        [Obsolete]
        public void Logger_MessageTypeWarning()
        {
            //Arrange
            var loggerObj = ObjLogger();
            //Act
            loggerObj.LoggingInfo("", "", MessageType.Warning, "");
        }
        #endregion
    }
}
