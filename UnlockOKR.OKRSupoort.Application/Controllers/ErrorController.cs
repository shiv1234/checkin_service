using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnlockOKR.OKRSupoort.Domain.Ports;
using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using System;
using System.IO;
using System.Net;

namespace UnlockOKR.OKRSupoort.Application.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ApiControllerBase
    {

        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILoggerFactory loggerFactory, IMediator mediator, ICommonBase commonBase) : base(loggerFactory, mediator, commonBase)
        {
            _logger = LoggerFactory.CreateLogger<ErrorController>();
        }

        [Route("error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            int code = (int)HttpStatusCode.InternalServerError;

            if (exception is NullReferenceException) code = (int)HttpStatusCode.NotImplemented;
            else if (exception is FileNotFoundException) code = (int)HttpStatusCode.NotFound;
            else if (exception is ArgumentNullException) code = (int)HttpStatusCode.BadRequest;
            else if (exception is UnauthorizedAccessException) code = (int)HttpStatusCode.Unauthorized;

            _logger.LogError(" AuthService: " + " in controller: ErrorController in method:Error  message: " + context);
            Response.StatusCode = code;
            return new ErrorResponse(exception);
        }
    }
}
