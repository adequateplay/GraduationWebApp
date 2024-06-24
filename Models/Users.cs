using System;
using System.ComponentModel.DataAnnotations;

namespace GraduationWebApp.Models
{
    public partial class Users
    {
        public Users()
        {
        }

        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}