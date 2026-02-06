using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities
{
    [Table("TenantMaster")]
    public partial class TenantMaster
    {
        public TenantMaster()
        {
            TenantUserDetails = new HashSet<TenantUserDetail>();
        }

        [Key]
        public Guid TenantId { get; set; }
        [Required]
        [StringLength(250)]
        public string SubDomain { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        public bool? IsLicensed { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DemoExpiryDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LicenseCreatedOn { get; set; }
        public int? PurchaseLicense { get; set; }
        public int? BufferLicense { get; set; }
        public bool IsFederated { get; set; }
        [Required]
        public bool? IsBufferCreated { get; set; }

        [InverseProperty(nameof(TenantUserDetail.Tenant))]
        public virtual ICollection<TenantUserDetail> TenantUserDetails { get; set; }
    }
}
