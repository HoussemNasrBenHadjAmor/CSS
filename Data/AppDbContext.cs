using Microsoft.EntityFrameworkCore;
using HRManagement.Models;

namespace HRManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<DepartmentSalarySummary> DepartmentSalarySummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentSalarySummary>()
                .HasNoKey()
                .ToView("vw_DepartmentSalarySummary");
        }
    }
}
