namespace OneGuru.CFR.Domain.ResponseModels
{
    public class EmployeePermissionResponse
    {
        public long PermissionId { get; set; }

        public string PermissionName { get; set; }

        public long ModuleId { get; set; }

        public string ModuleName { get; set; }

        public bool IsActive { get; set; }

        public bool? IsEditable { get; set; }
    }
}
