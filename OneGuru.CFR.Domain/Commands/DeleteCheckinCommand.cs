#nullable enable
using MediatR;
using OneGuru.CFR.Domain.ResponseModels;

namespace OneGuru.CFR.Domain.Commands
{
    public class DeleteCheckinCommand : IRequest<Payload<bool>>
    {
        public long CheckinId { get; set; }
    }
}
