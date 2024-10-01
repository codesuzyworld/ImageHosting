using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Models;

namespace ImageHosting.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // uploader.cs will map to a uploaders table
        public DbSet<Uploader> Uploader { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Images> Images { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
