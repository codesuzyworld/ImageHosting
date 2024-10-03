using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Models;
using System.Reflection;

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // It is a method call that ensures default config from IdentityDbContext are applied to the model
            // I had an error where it said primary key IdentityUserLogin is not defined 
            // Therefore this method ensuring that identity-related entities are applied
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
