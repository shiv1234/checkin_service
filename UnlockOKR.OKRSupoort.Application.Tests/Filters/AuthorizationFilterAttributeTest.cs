using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using UnlockOKR.OKRSupoort.Application.Filters;
using UnlockOKR.OKRSupoort.Domain.RequestModel;
using UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static UnlockOKR.OKRSupoort.Application.Filters.AuthorizationFilterAttribute;

namespace UnlockOKR.OKRSupoort.Application.Tests.Filters
{
    public class AuthorizationFilterAttributeTest
    {
        #region AuthorizeAction
        [Fact]
        public void AuthorizeAction_AuthorizationFilterContextNull_IsSuccess()
        {
            // Arrange
            var mockIServicesAggregator = new Mock<IServicesAggregator>();
            var mockISystemService = new Mock<ISystemService>();
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            var objIFilterMetadata = new List<IFilterMetadata>();
            mockISystemService.Setup(p => p.SystemHttpClient()).Returns(new HttpClient());
            var okjAuthorizationFilterContext = new AuthorizationFilterContext(actionContext, objIFilterMetadata);

            // Act
            var objAuthorizeAction = new AuthorizeAction(new Domain.Common.Permissions(), mockIServicesAggregator.Object, mockISystemService.Object);
            objAuthorizeAction.OnAuthorization(okjAuthorizationFilterContext);

            // Assert
            mockIServicesAggregator.Verify();

        }
        [Fact]
        public void AuthorizeAction_AuthorizationFilterContextNotNull_UserIdentityNull_IsSuccess()
        {
            // Arrange
            var mockIServicesAggregator = new Mock<IServicesAggregator>();
            var mockISystemService = new Mock<ISystemService>();
            mockISystemService.Setup(p => p.SystemHttpClient()).Returns(new HttpClient());
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            var objIFilterMetadata = new List<IFilterMetadata>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer ";

            var okjAuthorizationFilterContext = new AuthorizationFilterContext(actionContext, objIFilterMetadata)
            {
                HttpContext = httpContext
            };

            // Act
            var objAuthorizeAction = new AuthorizeAction(new UnlockOKR.OKRSupoort.Domain.Common.Permissions(), mockIServicesAggregator.Object, mockISystemService.Object);
            objAuthorizeAction.OnAuthorization(okjAuthorizationFilterContext);

            // Assert
            mockIServicesAggregator.Verify();
        }
        [Fact]
        public void AuthorizeAction_AuthorizationFilterContextNotNull_UserIdentityNotNull_IsSuccess()
        {
            // Arrange
            var mockIServicesAggregator = new Mock<IServicesAggregator>();
            var mockISystemService = new Mock<ISystemService>();
            mockISystemService.Setup(p => p.SystemHttpClient()).Returns(new HttpClient());
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            var objIFilterMetadata = new List<IFilterMetadata>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer ";
            httpContext.Request.Headers["UserIdentity"] = "Fi/6LK7FVqDlghpMFjX/tpZI3NuVR1Z3iTO971LN2kGC1tXIF4gVobrkByQZtQaigi6zBZIjLlPCQ5+0E3Vt2EHpEAYwQGNjTOzjvBpLM2HYqLEf5eLDlVPhBLqaIBbR9+Id3JCBZ0GTJsf5XVDFg4ZVlgU1hYYHSzXnjtWaCKXPWymiTCypAAvTyS8B6mT9qWK2/PbeyAW6PMdfl6zuuNTnOt5jP/vAfDluTa/cIw8a2wd5uGI0LlSq3GcGhZ5quVsTSE8Vj0lUL+QOYAbJ97FDSLa8cioBQpFEnvyQnyb5kX8Vf295igOQNo/RqPehqX9yYDemFXquVdOn7jeHYM72fMKxR7YVc9+UaXfwTYYOVouY+7QELHY4RGv0NFv/fbVCsldJV3BGEcF+7xFW5R83PFSJa/Octn/t+FIHsg1gfTXswC99Rcnk9fH5WzQT";
            var okjAuthorizationFilterContext = new AuthorizationFilterContext(actionContext, objIFilterMetadata)
            {
                HttpContext = httpContext
            };

            // Act
            var objAuthorizeAction = new AuthorizeAction(new Domain.Common.Permissions(), mockIServicesAggregator.Object, mockISystemService.Object);
            objAuthorizeAction.OnAuthorization(okjAuthorizationFilterContext);

            // Assert
            mockIServicesAggregator.Verify();
        }
        [Fact]
        public void GetUserIdentity_AuthorizationFilterContextNotNull_UserIdentityNotNull_IsSuccess()
        {
            // Arrange
            var mockIServicesAggregator = new Mock<IServicesAggregator>();
            var mockISystemService = new Mock<ISystemService>();
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            var objIFilterMetadata = new List<IFilterMetadata>();
            var httpContext = new DefaultHttpContext();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            // Setup Protected method on HttpMessageHandler mock.
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    response.StatusCode = System.Net.HttpStatusCode.OK;//Setting statuscode    
                    response.Content = new StringContent(JsonConvert.SerializeObject(new UserIdentity())); // configure your response here    
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); //Setting media type for the response    
                    return response;
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            httpContext.Request.Headers["TenantId"] = "6+XUkhjhXDVjBiCqODikcVJrho0PY3FwijTIdAdbywQlayt+AbCpwj9WwbVSXPpG";
            httpContext.Request.Headers["OriginHost"] = "https:localhost:9000.com";
            httpContext.Request.Headers["UserIdentity"] = "Fi/6LK7FVqDlghpMFjX/tpZI3NuVR1Z3iTO971LN2kGC1tXIF4gVobrkByQZtQaigi6zBZIjLlPCQ5+0E3Vt2EHpEAYwQGNjTOzjvBpLM2HYqLEf5eLDlVPhBLqaIBbR9+Id3JCBZ0GTJsf5XVDFg4ZVlgU1hYYHSzXnjtWaCKXPWymiTCypAAvTyS8B6mT9qWK2/PbeyAW6PMdfl6zuuNTnOt5jP/vAfDluTa/cIw8a2wd5uGI0LlSq3GcGhZ5quVsTSE8Vj0lUL+QOYAbJ97FDSLa8cioBQpFEnvyQnyb5kX8Vf295igOQNo/RqPehqX9yYDemFXquVdOn7jeHYM72fMKxR7YVc9+UaXfwTYYOVouY+7QELHY4RGv0NFv/fbVCsldJV3BGEcF+7xFW5R83PFSJa/Octn/t+FIHsg1gfTXswC99Rcnk9fH5WzQT";

            var okjAuthorizationFilterContext = new AuthorizationFilterContext(actionContext, objIFilterMetadata)
            {
                HttpContext = httpContext
            };
            mockIServicesAggregator.Setup(p => p.Configuration.GetSection("OkrUser:BaseUrl").Value).Returns("https://test/user/");
            var token = "Bearer eyJ0eXAiOiJKV1QiLCJHTD6h_C6gVpcb10FYj8ZvkEzBO73ZVhE0pplxf";

            mockISystemService.Setup(p => p.SystemHttpClient()).Returns(httpClient);
            // Act
            var objAuthorizeAction = new AuthorizeAction(new Domain.Common.Permissions(), mockIServicesAggregator.Object, mockISystemService.Object);
            var response = objAuthorizeAction.GetUserIdentity(token, okjAuthorizationFilterContext);

            // Assert
            Assert.NotNull(response);
        }
        #endregion

        #region AuthorizationFilterAttribute
        [Fact]
        public void AuthorizationFilterAttribute_CreateObject_IsSuccess()
        {
            // Act
            new AuthorizationFilterAttribute(new UnlockOKR.OKRSupoort.Domain.Common.Permissions());
        }
        #endregion
    }
}
