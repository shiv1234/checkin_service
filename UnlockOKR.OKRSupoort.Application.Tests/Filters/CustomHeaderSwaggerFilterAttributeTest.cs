using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Moq;
using UnlockOKR.OKRSupoort.Application.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Xunit;

namespace UnlockOKR.OKRSupoort.Application.Tests.Filters
{
    public class CustomHeaderSwaggerFilterAttributeTest
    {
        [Fact]
        public void CustomHeaderSwaggerFilterAttribute_IsSuccess()
        {
            // Arrange
            var mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();
            var mockIConfiguration = new Mock<IConfiguration>();
            var operation = new OpenApiOperation();
            var mockApiDescription = new ApiDescription();
            var mockISchemaGenerator = new Mock<ISchemaGenerator>();
            var mockSchemaRepository = new SchemaRepository();
            var mockMethodInfo = new Mock<MethodInfo>();

            var context = new OperationFilterContext(mockApiDescription, mockISchemaGenerator.Object, mockSchemaRepository, mockMethodInfo.Object);

            // Act
            var objCustomHeaderSwaggerFilterAttribute = new CustomHeaderSwaggerFilterAttribute(mockIWebHostEnvironment.Object, mockIConfiguration.Object);
            objCustomHeaderSwaggerFilterAttribute.Apply(operation, context);

            // Assert
            mockISchemaGenerator.Verify();
            mockMethodInfo.Verify();
        }

    }
}
