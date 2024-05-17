using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class GroupParticipants
    {
        public GroupParticipants() { }
        [Key]
        public int ParticipantId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public System.DateTime JoinDate { get; set; }

    }
}
