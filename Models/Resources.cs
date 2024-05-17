using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class Resources
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Resources()
        {

        }
        [Key]
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ResourceType { get; set; }
        public string ResourceFile { get; set; }


    }
}
