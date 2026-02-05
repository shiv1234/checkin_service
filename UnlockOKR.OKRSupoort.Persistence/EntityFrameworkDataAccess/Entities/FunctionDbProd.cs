using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess.Entities
{
    [Keyless]
    [Table("FunctionDB_Prod")]
    public partial class FunctionDbProd
    {
        [Column("FunctionDBId")]
        public short FunctionDbid { get; set; }
        [Required]
        [Column("DBName")]
        [StringLength(100)]
        public string Dbname { get; set; }
        [Required]
        [StringLength(100)]
        public string ScriptName { get; set; }
        [Required]
        [StringLength(100)]
        public string ConnectionServiceName { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
    }
}
