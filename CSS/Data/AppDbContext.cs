using Microsoft.EntityFrameworkCore;
using HRManagement.Models;

namespace HRManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentSalarySummary> DepartmentSalarySummaries { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<SalaryHistory> SalaryHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.FullName)
                .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");

            modelBuilder.Entity<DepartmentSalarySummary>()
                .HasNoKey()
                .ToView("vw_DepartmentSalarySummary");

            modelBuilder.Entity<DepartmentSalarySummary>()
                .Property(d => d.Department)
                .HasColumnName("DepartmentName");

            modelBuilder.Entity<DepartmentSalarySummary>()
                .Property(d => d.TotalSalary)
                .HasColumnName("TotalPayroll");

            modelBuilder.Entity<Attendance>()
                .HasKey(a => a.AttendanceId);

            modelBuilder.Entity<PerformanceReview>()
                .HasKey(pr => pr.ReviewId);

            modelBuilder.Entity<SalaryHistory>()
                .HasKey(sh => sh.SalaryHistoryId);
        }
    }
}