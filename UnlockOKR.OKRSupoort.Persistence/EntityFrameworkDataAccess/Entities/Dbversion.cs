using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess.Entities
{
    [Table("DBVersion")]
    public partial class Dbversion
    {
        [Key]
        [Column("DBVersionId")]
        public int DbversionId { get; set; }
        public Guid? TenantId { get; set; }
        [Required]
        [Column("DBName")]
        [StringLength(250)]
        public string Dbname { get; set; }
        [Required]
        [StringLength(250)]
        public string KeyVault { get; set; }
        public int? ScriptSeq { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
    }
}
