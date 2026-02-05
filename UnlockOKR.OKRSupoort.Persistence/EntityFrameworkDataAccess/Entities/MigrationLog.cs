using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess.Entities
{
    [Keyless]
    [Table("MigrationLog")]
    public partial class MigrationLog
    {
        [StringLength(200)]
        public string TenantId { get; set; }
        [Column("SourceDBName")]
        [StringLength(200)]
        public string SourceDbname { get; set; }
        [Column("TargetDBName")]
        [StringLength(200)]
        public string TargetDbname { get; set; }
        [StringLength(200)]
        public string TableName { get; set; }
        public int? SourceRowCount { get; set; }
        public int? TargetRowCount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? MigratedOn { get; set; }
    }
}
