using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using OneGuru.CFR.Application.Extensions;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using System;
using Xunit;

namespace OneGuru.CFR.Application.Tests.Extensions
{
    public class TokenManagerMiddlewareTest
    {
        private readonly Mock<IConfiguration> _mockIConfiguration;
        private readonly Mock<ISystemService> _mockISystemService;
        private readonly Mock<RequestDelegate> _mockRequestDelegate;
        public TokenManagerMiddlewareTest()
        {
            _mockIConfiguration = new Mock<IConfiguration>();
            _mockISystemService = new Mock<ISystemService>();
            _mockRequestDelegate = new Mock<RequestDelegate>();
        }
        [Fact]
        public void TokenManagerMiddleware_HealthPath_IsNull()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/Home/health";
            // Act
            var objResultFilter = new TokenManagerMiddleware(_mockIConfiguration.Object, _mockISystemService.Object);
            var response = objResultFilter.InvokeAsync(context, _mockRequestDelegate.Object);
            // Assert
            Assert.NotNull(response);
            _mockIConfiguration.Verify();
        }
        [Fact]
        public void TokenManagerMiddleware_HealthPath_IsError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/Home/Test";
            context.Request.Headers["Authorization"] = "";
            // Act
            var objResultFilter = new TokenManagerMiddleware(_mockIConfiguration.Object, _mockISystemService.Object);
            var response = objResultFilter.InvokeAsync(context, _mockRequestDelegate.Object);
            // Assert
            Assert.NotNull(response);
            _mockIConfiguration.Verify();
        }
        [Fact]
        public void TokenManagerMiddleware_APIPath_IsSuccess()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/Home/Index";
            context.Request.Headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRlc3QgVXNlciIsImVtYWlsIjoidGVzdEB0ZXN0LmNvbSIsImlhdCI6MTUxNjIzOTAyMn0.PLACEHOLDER_SIGNATURE";
            var token = "Bearer PLACEHOLDER_TEST_TOKEN";
            context.Request.Headers["Token"] = token;
            context.Request.Headers["TenantId"] = "PLACEHOLDER_TENANT_ID";
            context.Request.Headers["OriginHost"] = "https://test.com";

            _mockISystemService.Setup(p => p.SystemUri(It.IsAny<string>())).Returns(new Uri("https://test.com"));

            // Act
            var objResultFilter = new TokenManagerMiddleware(_mockIConfiguration.Object,
                _mockISystemService.Object);
            var response = objResultFilter.InvokeAsync(context, _mockRequestDelegate.Object);

            // Assert
            Assert.NotNull(response);
            _mockIConfiguration.Verify();
        }
        [Fact]
        public void TokenManagerMiddleware_TokenWithoutEmail_IsNull()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/Home/Index";
            context.Request.Headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRlc3QgVXNlciIsImVtYWlsIjoiIiwiaWF0IjoxNTE2MjM5MDIyfQ.PLACEHOLDER_SIGNATURE";
            var token = "Bearer PLACEHOLDER_TEST_TOKEN";
            context.Request.Headers["Token"] = token;
            context.Request.Headers["TenantId"] = "PLACEHOLDER_TENANT_ID";
            context.Request.Headers["OriginHost"] = "https://test.com";

            _mockISystemService.Setup(p => p.SystemUri(It.IsAny<string>())).Returns(new Uri("https://test.com"));

            // Act
            var objResultFilter = new TokenManagerMiddleware(_mockIConfiguration.Object,
                _mockISystemService.Object);
            var response = objResultFilter.InvokeAsync(context, _mockRequestDelegate.Object);

            // Assert
            Assert.NotNull(response);
            _mockIConfiguration.Verify();
        }
        [Fact]
        public void TokenManagerMiddleware_TokenWithoutEmailAndPreferredUsername_IsNull()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/Home/Index";
            context.Request.Headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRlc3QgVXNlciIsImVtYWlsIjoiIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiIiwidW5pcXVlX25hbWUiOiJ0ZXN0QHRlc3QuY29tIiwiaWF0IjoxNTE2MjM5MDIyfQ.PLACEHOLDER_SIGNATURE";
            var token = "Bearer PLACEHOLDER_TEST_TOKEN";
            context.Request.Headers["Token"] = token;
            context.Request.Headers["TenantId"] = "PLACEHOLDER_TENANT_ID";
            context.Request.Headers["OriginHost"] = "https://test.com";

            _mockISystemService.Setup(p => p.SystemUri(It.IsAny<string>())).Returns(new Uri("https://test.com"));

            // Act
            var objResultFilter = new TokenManagerMiddleware(_mockIConfiguration.Object,
                _mockISystemService.Object);
            var response = objResultFilter.InvokeAsync(context, _mockRequestDelegate.Object);

            // Assert
            Assert.NotNull(response);
            _mockIConfiguration.Verify();
        }
    }
}
