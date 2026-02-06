using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities
{
    [Keyless]
    [Table("tmp_ErrLog")]
    public partial class TmpErrLog
    {
        [Column("moment", TypeName = "datetime")]
        public DateTime? Moment { get; set; }
        [Column("pid")]
        [StringLength(20)]
        public string Pid { get; set; }
        [Column("root_pid")]
        [StringLength(20)]
        public string RootPid { get; set; }
        [Column("father_pid")]
        [StringLength(20)]
        public string FatherPid { get; set; }
        [Column("project")]
        [StringLength(50)]
        public string Project { get; set; }
        [Column("job")]
        [StringLength(255)]
        public string Job { get; set; }
        [Column("context")]
        [StringLength(50)]
        public string Context { get; set; }
        [Column("priority")]
        public int? Priority { get; set; }
        [Column("type")]
        [StringLength(255)]
        public string Type { get; set; }
        [Column("origin")]
        [StringLength(255)]
        public string Origin { get; set; }
        [Column("message")]
        [StringLength(255)]
        public string Message { get; set; }
        [Column("code")]
        public int? Code { get; set; }
    }
}
