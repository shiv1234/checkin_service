using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using UnlockOKR.OKRSupoort.Application.Filters;
using System.Collections.Generic;
using Xunit;

namespace UnlockOKR.OKRSupoort.Application.Tests.Filters
{
    public class ResultFilterTest
    {
        [Fact]
        public void ResultFilter_IsSuccess()
        {
            // Arrange
            string headersName = "header name";
            // Create a default ActionContext (depending on our case-scenario)
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            var objIFilterMetadata = new List<IFilterMetadata>();
            var mockIActionResult = new Mock<IActionResult>();
            // Create the filter input parameters (depending on our case-scenario)
            var resultExecutingContext = new ResultExecutingContext(actionContext, new List<IFilterMetadata>(), new ObjectResult("A dummy result from the action method."), Mock.Of<Controller>());

            var objResultExecutedContext = new ResultExecutedContext(actionContext, objIFilterMetadata, mockIActionResult.Object, resultExecutingContext);
            // Act
            var objResultFilter = new ResultFilter();
            objResultFilter.OnResultExecuted(objResultExecutedContext);
            // Assert
            Assert.Equal(0, resultExecutingContext.HttpContext.Response.Headers.Count);
            Assert.False(resultExecutingContext.HttpContext.Response.Headers.ContainsKey(headersName));
        }
    }
}
