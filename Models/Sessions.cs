using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationWebApp.Models
{
    public partial class Sessions
    {
        public Sessions() { }

        [Key]
        public int SessionId { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }

        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public virtual Groups Group { get; set; }
    }
}
