using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(userRole =>
            {
                userRole.Ignore(s => s.NormalizedEmail);
                userRole.Ignore(s => s.NormalizedUserName);
                userRole.Ignore(s => s.ConcurrencyStamp);
                userRole.Ignore(s => s.LockoutEnabled);
                userRole.Ignore(s => s.LockoutEnd);
            });
        }

    }
}