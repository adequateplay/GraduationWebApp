using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class Sessions
    {
        public Sessions() { }
        [Key]
        public int SessionId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public System.DateTime SessionDate { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }

    }
}
