using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess.Entities
{
    [Keyless]
    [Table("TenantMaster_Prod")]
    public partial class TenantMasterProd
    {
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
    }
}
