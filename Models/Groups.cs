using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class Groups
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Groups()
        {

        }
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<int> LeaderId { get; set; }

    }
}
