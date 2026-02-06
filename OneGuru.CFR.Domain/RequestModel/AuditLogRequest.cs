#nullable enable
namespace OneGuru.CFR.Domain.RequestModel;

public class AuditLogRequest
{
    public string? ActionType { get; set; }
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public string? ActivityName { get; set; }
    public long TransactionId { get; set; }
    public string? ActivityDescription { get; set; }
}
