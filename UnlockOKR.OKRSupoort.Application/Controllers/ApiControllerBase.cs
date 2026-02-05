using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnlockOKR.OKRSupoort.Domain.Ports;

namespace UnlockOKR.OKRSupoort.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public IMediator Mediator { get; set; }
        public ICommonBase CommonBase;
        public ApiControllerBase(ILoggerFactory loggerFactory, IMediator mediator, ICommonBase commonBase)
        {
            LoggerFactory = loggerFactory;
            Mediator = mediator;
            CommonBase = commonBase;
        }
    }
}
