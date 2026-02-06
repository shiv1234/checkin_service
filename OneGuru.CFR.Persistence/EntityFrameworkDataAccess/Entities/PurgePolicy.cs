using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities
{
    [Table("PurgePolicy")]
    public partial class PurgePolicy
    {
        [Key]
        public int PolicyId { get; set; }
        [Required]
        [Column("DBName")]
        [StringLength(100)]
        public string Dbname { get; set; }
        [Required]
        [StringLength(250)]
        public string RefTable { get; set; }
        [Required]
        [StringLength(250)]
        public string RefColumn { get; set; }
        public int RefValue { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastPurgedOn { get; set; }
        public long? TotalPurgeCount { get; set; }
    }
}
