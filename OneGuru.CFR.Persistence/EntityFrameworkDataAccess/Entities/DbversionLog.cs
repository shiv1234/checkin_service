using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities
{
    [Table("DBVersionLog")]
    public partial class DbversionLog
    {
        [Key]
        [Column("DBVersionLogId")]
        public int DbversionLogId { get; set; }
        [Column("DBVersionId")]
        public int DbversionId { get; set; }
        public int? ScriptSeq { get; set; }
        [Required]
        public string ErrorMessage { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
    }
}
