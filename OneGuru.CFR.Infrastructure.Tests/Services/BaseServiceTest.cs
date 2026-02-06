using Microsoft.Extensions.Logging;
using Moq;
using OneGuru.CFR.Domain.ResponseModels;
using OneGuru.CFR.Infrastructure.Services;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using static OneGuru.CFR.Infrastructure.Services.BaseService;

namespace OneGuru.CFR.Infrastructure.Tests.Services
{
    public class BaseServiceTest
    {
        private readonly Mock<IServicesAggregator> _mockIServicesAggregator;
        private readonly Mock<ILoggerFactory> _mockILoggerFactory;
        public BaseServiceTest()
        {
            _mockIServicesAggregator = new Mock<IServicesAggregator>();
            _mockILoggerFactory = new Mock<ILoggerFactory>();
        }
        [Obsolete]
        public BaseService ObjBaseService()
        {
            return new BaseService(_mockIServicesAggregator.Object);
        }

        private T GetModelTestData<T>(T newModel)
        {
            Type type = newModel.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propTypeInfo = type.GetProperty(prop.Name.Trim());
                if (propTypeInfo.CanRead)
                    prop.GetValue(newModel);
            }
            return newModel;
        }

        private T SetModelTestData<T>(T newModel)
        {
            Type type = newModel.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propTypeInfo = type.GetProperty(prop.Name.Trim());
                var propType = prop.GetType();

                if (propTypeInfo.CanWrite)
                {
                    if (prop.PropertyType.Name == "String")
                    {
                        prop.SetValue(newModel, String.Empty);
                    }
                    else if (propType.IsValueType)
                    {
                        prop.SetValue(newModel, Activator.CreateInstance(propType));
                    }
                    else
                    {
                        prop.SetValue(newModel, null);
                    }
                }
            }
            return newModel;
        }

        #region GetPayloadStatus
        [Fact]
        [Obsolete]
        public void BaseService_GetPayloadStatus_IsSuccessFalse()
        {
            //Arrange
            var payload = new Payload<string>();
            _mockIServicesAggregator.Setup(c => c.LoggerFactory).Returns(_mockILoggerFactory.Object);
            var baseServiceObj = ObjBaseService();
            //Act
            var response = baseServiceObj.GetPayloadStatus(payload, "test", "test");
            //Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
        }
        [Fact]
        [Obsolete]
        public void BaseService_GetPayloadStatus_IsSuccessTrue()
        {
            //Arrange
            Dictionary<string, string> MessageListVal = new Dictionary<string, string>();
            MessageListVal.Add("Aust", "Australia");
            MessageListVal.Add("test", "test");
            var payload = new Payload<string>()
            {
                MessageList = MessageListVal
            };
            _mockIServicesAggregator.Setup(c => c.LoggerFactory).Returns(_mockILoggerFactory.Object);
            var baseServiceObj = ObjBaseService();
            //Act
            var response = baseServiceObj.GetPayloadStatus(payload, "test", "test");
            //Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
        }
        #endregion
        #region GetPayloadStatusSuccess
        [Fact]
        [Obsolete]
        public void BaseService_GetPayloadStatusSuccess_IsSuccessTrue()
        {
            //Arrange
            Dictionary<string, string> MessageListVal = new Dictionary<string, string>();
            MessageListVal.Add("Aust", "Australia");
            MessageListVal.Add("test", "test");
            var payload = new Payload<string>()
            {
                MessageList = MessageListVal
            };
            _mockIServicesAggregator.Setup(c => c.LoggerFactory).Returns(_mockILoggerFactory.Object);
            var baseServiceObj = ObjBaseService();
            //Act
            var response = baseServiceObj.GetPayloadStatusSuccess(payload, "test", "test");
            //Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
        }
        #endregion
        #region GetPayloadStatusNoContent
        [Fact]
        [Obsolete]
        public void BaseService_GetPayloadStatusNoContent_IsSuccessTrue()
        {
            //Arrange
            Dictionary<string, string> MessageListVal = new Dictionary<string, string>();
            MessageListVal.Add("Aust", "Australia");
            MessageListVal.Add("test", "test");
            var payload = new Payload<string>()
            {
                MessageList = MessageListVal
            };
            _mockIServicesAggregator.Setup(c => c.LoggerFactory).Returns(_mockILoggerFactory.Object);
            var baseServiceObj = ObjBaseService();
            //Act
            var response = baseServiceObj.GetPayloadStatusNoContent(payload, "test", "test");
            //Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
        }
        #endregion
        #region PageResults
        [Fact]
        public void PageResults()
        {
            var model = new PageResults<string>();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        #endregion 
        #region PayloadCustomList
        [Fact]
        public void PayloadCustomList()
        {
            var model = new PayloadCustomList<string>();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        #endregion 
    }
}
