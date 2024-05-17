using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class Journals
    {
        public Journals()
        {

        }
        [Key]
        public int JournalId { get; set; }
        public Nullable<int> UserId { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Entry { get; set; }

    }
}
