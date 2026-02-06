using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Application.Controllers
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
