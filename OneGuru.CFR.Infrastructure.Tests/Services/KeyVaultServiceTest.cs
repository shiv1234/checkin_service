using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OneGuru.CFR.Infrastructure.Services;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities;
using System;
using Xunit;

namespace OneGuru.CFR.Infrastructure.Tests.Services
{
    public class KeyVaultServiceTest
    {
        private readonly Mock<IServicesAggregator> _mockIServicesAggregator;
        private readonly Mock<ISystemService> _mockSystemService;
        private readonly Mock<ICommonService> _mockICommonService;

        public KeyVaultServiceTest()
        {
            _mockIServicesAggregator = new Mock<IServicesAggregator>();
            var mockILoggerFactory = new Mock<ILoggerFactory>();

            _mockICommonService = new Mock<ICommonService>();
            _mockSystemService = new Mock<ISystemService>();
            _mockIServicesAggregator.Setup(c => c.LoggerFactory).Returns(mockILoggerFactory.Object);
        }
        [Obsolete]
        public KeyVaultService ObjKeyVaultService()
        {
            return new KeyVaultService(_mockIServicesAggregator.Object, _mockICommonService.Object, _mockSystemService.Object);
        }

        #region GetAzureBlobKeys
        [Fact]
        [Obsolete]
        public void GetAzureBlobKeys_IsTokenActiveFalse_Null()
        {
            //Arrange
            _mockICommonService.Setup(c => c.IsTokenActive).Returns(false);
            KeyVaultService keyVaultService = ObjKeyVaultService();

            //Act
            var result = keyVaultService.GetAzureBlobKeysAsync();

            //Assert
            Assert.Null(result.Result);
        }
        [Fact]
        [Obsolete]
        public void GetAzureBlobKeys_IsSuccessTrue()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["TenantId"] = "6+XUkhjhXDVjBiCqODikcVJrho0PY3FwijTIdAdbywQlayt+AbCpwj9WwbVSXPpG";
            _mockICommonService.Setup(c => c.IsTokenActive).Returns(true);

            _mockSystemService.Setup(c => c.HttpContext).Returns(httpContext);
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("AzureBlob:BlobAccountKey").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("AzureBlob:BlobAccountName").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("AzureBlob:BlobCdnUrl").Value).Returns("test");

            KeyVaultService keyVaultService = ObjKeyVaultService();

            //Act
            var result = keyVaultService.GetAzureBlobKeysAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal("test", result.Result.BlobAccountKey);
            Assert.Equal("test", result.Result.BlobAccountName);
            Assert.Equal("test", result.Result.BlobCdnUrl);
            Assert.Equal("testcommon/", result.Result.BlobCdnCommonUrl);
        }
        #endregion
        #region GetSettingsAndUrls
        [Fact]
        [Obsolete]
        public void GetSettingsAndUrls_IsTokenActiveFalse_Null()
        {
            //Arrange
            _mockICommonService.Setup(c => c.IsTokenActive).Returns(false);
            KeyVaultService keyVaultService = ObjKeyVaultService();

            //Act
            var result = keyVaultService.GetSettingsAndUrlsAsync();

            //Assert
            Assert.Null(result.Result);
        }
        [Fact]
        [Obsolete]
        public void GetSettingsAndUrls_IsSuccessTrue()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["TenantId"] = "6+XUkhjhXDVjBiCqODikcVJrho0PY3FwijTIdAdbywQlayt+AbCpwj9WwbVSXPpG";
            httpContext.Request.Headers["OriginHost"] = "https:\\test.com";
            _mockICommonService.Setup(c => c.IsTokenActive).Returns(true);

            _mockSystemService.Setup(c => c.HttpContext).Returns(httpContext);
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("OkrService:UnlockLog").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("OkrService:BaseUrl").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("OkrService:UnlockTime").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("ResetPassUrl").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("Notification:BaseUrl").Value).Returns("test");
            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("TenantService:BaseUrl").Value).Returns("test");

            KeyVaultService keyVaultService = ObjKeyVaultService();

            //Act
            var result = keyVaultService.GetSettingsAndUrlsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal("test", result.Result.OkrBaseAddress);
            Assert.Equal("test", result.Result.OkrUnlockTime);
            Assert.Equal("test", result.Result.OkrUnlockTime);
            Assert.Equal("test", result.Result.NotificationBaseAddress);
            Assert.Equal("test", result.Result.TenantBaseAddress);
        }
        #endregion
    }
}
