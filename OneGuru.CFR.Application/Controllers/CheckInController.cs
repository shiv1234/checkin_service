using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneGuru.CFR.Application.Authorization;
using OneGuru.CFR.Domain.Commands;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.ResponseModels;

namespace OneGuru.CFR.Application.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class CheckInController : ApiControllerBase
    {
        public CheckInController(
            ILoggerFactory loggerFactory,
            IMediator mediator,
            ICommonBase commonBase)
            : base(loggerFactory, mediator, commonBase)
        {
        }

        [HttpPost("submit")]
        [Authorize(Policy = AuthorizationPolicies.CanSubmitCheckins)]
        public async Task<IActionResult> Submit([FromBody] SubmitCheckinCommand command)
        {
            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Status, result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicies.CanDeleteCheckins)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await Mediator.Send(new DeleteCheckinCommand { CheckinId = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.Status, result);
            }

            return Ok(result);
        }
    }
}
