using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class ResourceCategories
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ResourceCategories()
        {
        }
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
