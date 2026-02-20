using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagement.Data;
using HRManagement.Models;

namespace HRManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: All Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }

        // Stored Procedure - SELECT
        public async Task<IActionResult> ByDepartment(string department)
        {
            var employees = await _context.Employees
                .FromSqlRaw("EXEC sp_GetEmployeesByDepartment @DepartmentName = {0}", department)
                .ToListAsync();

            return View(employees);
        }

        // Stored Procedure - UPDATE
        [HttpPost]
        public async Task<IActionResult> IncreaseSalary(int id, decimal percentage)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_IncreaseSalary @EmployeeId = {0}, @Percentage = {1}",
                id, percentage);

            return RedirectToAction(nameof(Index));
        }

        // SQL View
        public async Task<IActionResult> DepartmentSummary()
        {
            var summary = await _context.DepartmentSalarySummaries.ToListAsync();
            return View(summary);
        }
    }
}
