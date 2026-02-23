using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HRManagement.Data;
using HRManagement.Models;

namespace HRManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees
                           .Include(e => e.Department)
                           .Where(e => e.IsActive)
                           .ToList();
        }

        public Employee GetById(int id)
        {
            return _context.Employees.Find(id);
        }

        public void IncreaseSalary(int id, decimal percentage)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC sp_IncreaseSalary @EmployeeId, @Percentage",
                new SqlParameter("@EmployeeId", id),
                new SqlParameter("@Percentage", percentage));
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}