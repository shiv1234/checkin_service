using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities
{
    [Table("ErrorLog")]
    public partial class ErrorLog
    {
        [Key]
        public long LogId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        [Required]
        [StringLength(100)]
        public string PageName { get; set; }
        [Required]
        [StringLength(100)]
        public string FunctionName { get; set; }
        [Required]
        public string ErrorDetail { get; set; }
    }
}
