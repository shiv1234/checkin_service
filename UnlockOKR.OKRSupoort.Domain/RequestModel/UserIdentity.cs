using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using System;
using System.Collections.Generic;

namespace UnlockOKR.OKRSupoort.Domain.RequestModel
{
    public class UserIdentity
    {
        public long EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long RoleId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
        public string ReportingTo { get; set; }
        public string ImageDetail { get; set; }
        public long OrganisationId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public List<EmployeePermissionResponse> RolePermissions { get; set; }
        public bool IsImpersonatedUser { get; set; } = false;
        public string ImpersonatedBy { get; set; } = string.Empty;
        public long ImpersonatedById { get; set; } = 0;
    }
}
