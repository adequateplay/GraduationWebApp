using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GraduationWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GraduationWebApp.Models.Users> Users { get; set; } = default!;
        public DbSet<GraduationWebApp.Models.Groups> Groups { get; set; } = default!;
        public DbSet<GraduationWebApp.Models.Journals> Journals { get; set; } = default!;
        public DbSet<GraduationWebApp.Models.Resources> Resources { get; set; } = default!;
        public DbSet<GraduationWebApp.Models.GroupParticipants> GroupParticipants { get; set; } = default!;
        public DbSet<GraduationWebApp.Models.Sessions> Sessions { get; set; } = default!;
        public DbSet<GraduationWebApp.Models.ResourceCategories> ResourceCategories { get; set; } = default!;
    }
}
