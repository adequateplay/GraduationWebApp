using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationWebApp.Models
{
    public partial class Groups
    {
        public Groups()
        {
        }

        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? LeaderId { get; set; }

        [ForeignKey("LeaderId")]
        public virtual Users Leader { get; set; }
    }
}