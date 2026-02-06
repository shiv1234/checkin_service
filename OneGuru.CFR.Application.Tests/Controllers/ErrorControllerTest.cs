using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using OneGuru.CFR.Application.Controllers;
using OneGuru.CFR.Domain.Ports;
using System;
using System.IO;
using Xunit;

namespace OneGuru.CFR.Application.Tests.Controllers
{
    public class ErrorControllerTest
    {
        private MockRepository _mockRepository;
        private Mock<IMediator> _mockMediator;
        private Mock<ICommonBase> _commonBase;
        private void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockMediator = _mockRepository.Create<IMediator>();
            _commonBase = new Mock<ICommonBase>();
        }
        private ErrorController CreateErrorController()
        {
            Setup();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            loggerFactory.CreateLogger<ErrorController>();
            return new ErrorController(loggerFactory, _mockMediator.Object, _commonBase.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public void ErrorController_ExpectedNullReferenceException_Success()
        {
            // Arrange
            var controller = CreateErrorController();

            var exceptionHandlerFeature = Substitute.For<IExceptionHandlerFeature>();
            var exception = Substitute.For<Exception>(new NullReferenceException().ToString());
            exception.StackTrace.Returns(new NullReferenceException().ToString());
            exceptionHandlerFeature.Error.Returns(new NullReferenceException());
            controller.HttpContext.Features.Set(exceptionHandlerFeature);

            // Act
            var result = controller.Error();

            //Assert
            Assert.NotNull(result.StackTrace.ToString());
            Assert.NotNull(result);
        }
        [Fact]
        public void ErrorController_ExpectedFileNotFoundException_Success()
        {
            // Arrange
            var controller = CreateErrorController();

            var exceptionHandlerFeature = Substitute.For<IExceptionHandlerFeature>();
            var exception = Substitute.For<Exception>(new FileNotFoundException().ToString());
            exception.StackTrace.Returns(new FileNotFoundException().ToString());
            exceptionHandlerFeature.Error.Returns(new FileNotFoundException());
            controller.HttpContext.Features.Set(exceptionHandlerFeature);

            // Act
            var result = controller.Error();

            //Assert
            Assert.NotNull(result.StackTrace.ToString());
            Assert.NotNull(result);
        }
        [Fact]
        public void ErrorController_ExpectedArgumentNullException_Success()
        {
            // Arrange
            var controller = CreateErrorController();

            var exceptionHandlerFeature = Substitute.For<IExceptionHandlerFeature>();
            var exception = Substitute.For<Exception>(new ArgumentNullException().ToString());
            exception.StackTrace.Returns(new ArgumentNullException().ToString());
            exceptionHandlerFeature.Error.Returns(new ArgumentNullException());
            controller.HttpContext.Features.Set(exceptionHandlerFeature);

            // Act
            var result = controller.Error();

            //Assert
            Assert.NotNull(result.StackTrace.ToString());
            Assert.NotNull(result);
        }
        [Fact]
        public void ErrorController_ExpectedUnauthorizedAccessException_Success()
        {
            // Arrange
            var controller = CreateErrorController();

            var exceptionHandlerFeature = Substitute.For<IExceptionHandlerFeature>();
            var exception = Substitute.For<Exception>(new UnauthorizedAccessException().ToString());
            exception.StackTrace.Returns(new UnauthorizedAccessException().ToString());
            exceptionHandlerFeature.Error.Returns(new UnauthorizedAccessException());
            controller.HttpContext.Features.Set(exceptionHandlerFeature);

            // Act
            var result = controller.Error();

            //Assert
            Assert.NotNull(result.StackTrace);
            Assert.NotNull(result);
        }
    }
}

