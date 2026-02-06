#nullable enable
namespace OneGuru.CFR.Domain.ResponseModels
{
    public class EmployeePermissionResponse
    {
        public long PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        public long ModuleId { get; set; }

        public string ModuleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool? IsEditable { get; set; }
    }
}
