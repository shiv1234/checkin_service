using System.Collections.Generic;

namespace OneGuru.CFR.Domain.ResponseModels
{
    public class EmployeeRolePermissionResponse
    {
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public List<EmployeePermissionResponse> EmployeePermissions { get; set; }
    }
}
