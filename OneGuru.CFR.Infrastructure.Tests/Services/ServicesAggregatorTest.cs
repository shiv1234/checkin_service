using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OneGuru.CFR.Infrastructure.Services;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;
using System;
using Xunit;

namespace OneGuru.CFR.Infrastructure.Tests.Services
{
    public class ServicesAggregatorTest
    {
        private readonly Mock<IUnitOfWorkAsync> _mockIUnitOfWorkAsync;
        private readonly Mock<IOperationStatus> _mockIOperationStatus;
        private readonly Mock<IConfiguration> _mockIConfiguration;
        private readonly Mock<IMapper> _mockIMapper;
        private readonly Mock<IWebHostEnvironment> _mockIWebHostEnvironment;
        private readonly Mock<ILoggerFactory> _mockILoggerFactory;
        public ServicesAggregatorTest()
        {
            _mockIUnitOfWorkAsync = new Mock<IUnitOfWorkAsync>();
            _mockIOperationStatus = new Mock<IOperationStatus>();
            _mockIConfiguration = new Mock<IConfiguration>();
            _mockIMapper = new Mock<IMapper>();
            _mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockILoggerFactory = new Mock<ILoggerFactory>();
        }
        [Obsolete]
        public ServicesAggregator ObjServicesAggregator()
        {
            return new ServicesAggregator(_mockIUnitOfWorkAsync.Object,
                _mockIOperationStatus.Object, _mockIConfiguration.Object,
                _mockIMapper.Object, _mockIWebHostEnvironment.Object,
                _mockILoggerFactory.Object);
        }
        [Fact]
        [Obsolete]
        public void ServicesAggregator_VerifyProp()
        {
            //Arrange
            _mockIWebHostEnvironment.Setup(p => p.ApplicationName).Returns("test");
            var servicesAggregatorObj = ObjServicesAggregator();
            //Assert
            _mockIUnitOfWorkAsync.Verify();
            _mockIOperationStatus.Verify();
            _mockIConfiguration.Verify();
            _mockIMapper.Verify();
            _mockIWebHostEnvironment.Verify();
            _mockILoggerFactory.Verify();
            Assert.Equal("test", servicesAggregatorObj.HostingEnvironment.ApplicationName);
        }
    }
}
