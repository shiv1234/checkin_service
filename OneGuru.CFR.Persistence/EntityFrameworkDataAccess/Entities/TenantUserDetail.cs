using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities
{
    public partial class TenantUserDetail
    {
        [Key]
        public long TenantUserId { get; set; }
        [Required]
        [StringLength(250)]
        public string EmailId { get; set; }
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(TenantId))]
        [InverseProperty(nameof(TenantMaster.TenantUserDetails))]
        public virtual TenantMaster Tenant { get; set; }
    }
}
