using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Domain.ResponseModels;
using OneGuru.CFR.Infrastructure.Services;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OneGuru.CFR.Infrastructure.Tests.Services
{
    public class CommonServiceTest
    {
        private readonly Mock<IServicesAggregator> _mockIServicesAggregator;
        private readonly Mock<ISystemService> _mockSystemService;

        public CommonServiceTest()
        {
            _mockIServicesAggregator = new Mock<IServicesAggregator>();

            var mockILoggerFactory = new Mock<ILoggerFactory>();
            _mockSystemService = new Mock<ISystemService>();
            var mockHttpClient = new Mock<HttpClient>();

            
            _mockIServicesAggregator.Setup(c => c.LoggerFactory).Returns(mockILoggerFactory.Object);
            _mockSystemService.Setup(p => p.SystemHttpClient()).Returns(mockHttpClient.Object);
        }

        #region GetHttpClient
        [Fact]
        [Obsolete]
        public void GetHttpClient_HttpClient_Success()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["TenantId"] = "6+XUkhjhXDVjBiCqODikcVJrho0PY3FwijTIdAdbywQlayt+AbCpwj9WwbVSXPpG";
            httpContext.Request.Headers["OriginHost"] = "https:\\localhost:9000.com";
            _mockSystemService.Setup(c => c.HttpContext).Returns(httpContext);

            ICommonService objCommonService = new CommonService(_mockIServicesAggregator.Object, _mockSystemService.Object);

            //Act
            var result = objCommonService.GetHttpClient("http://msdn.microsoft.com/en-us/library/456dfw4f.aspx");

            //Assert
            Assert.NotNull(result.BaseAddress);
            Assert.Equal("http://msdn.microsoft.com/en-us/library/456dfw4f.aspx", result.BaseAddress.ToString());
        }
        #endregion

        #region GetUserIdentity
        [Fact]
        [Obsolete]
        public void GetUserIdentity_hasIdentityTrue_Success()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["UserIdentity"] = "test";
            _mockSystemService.Setup(c => c.HttpContext).Returns(httpContext);

            ICommonService objCommonService = new CommonService(_mockIServicesAggregator.Object, _mockSystemService.Object);

            //Act
            var result = objCommonService.GetUserIdentity();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        [Obsolete]
        public void GetUserIdentity_hasIdentityFalse_Success()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["TenantId"] = "6+XUkhjhXDVjBiCqODikcVJrho0PY3FwijTIdAdbywQlayt+AbCpwj9WwbVSXPpG";
            httpContext.Request.Headers["OriginHost"] = "https:\\localhost:9000.com";
            _mockSystemService.Setup(c => c.HttpContext).Returns(httpContext);

            _mockIServicesAggregator.Setup(p => p.Configuration.GetSection("OkrUser:BaseUrl").Value).Returns("http://test.aspx");

            var mockUserIdentity = new UserIdentity()
            {
                EmailId = "test@test.com"
            };

            var mockPayloadUserIdentity = new Payload<UserIdentity>()
            {
                Entity = mockUserIdentity
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            // Setup Protected method on HttpMessageHandler mock.
            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    response.StatusCode = System.Net.HttpStatusCode.OK;//Setting statuscode    
                    response.Content = new StringContent(JsonConvert.SerializeObject(mockPayloadUserIdentity)); // configure your response here    
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); //Setting media type for the response    
                    return response;
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockSystemService.Setup(p => p.SystemHttpClient()).Returns(httpClient);

            ICommonService objCommonService = new CommonService(_mockIServicesAggregator.Object, _mockSystemService.Object);

            //Act
            var result = objCommonService.GetUserIdentity();

            //Assert
            Assert.NotNull(result);
            Assert.Equal("test@test.com", result.EmailId);
        }
        #endregion

    }
}
