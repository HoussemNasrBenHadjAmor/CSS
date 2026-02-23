using Microsoft.AspNetCore.Mvc;
using HRManagement.Data;
using HRManagement.Models;

namespace HRManagement.Controllers
{
    public class DashboardFullController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardFullController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardKpiViewModel
            {
                TotalEmployees = _context.Employees.Count(),
                ActiveEmployees = _context.Employees.Count(e => e.IsActive),
                InactiveEmployees = _context.Employees.Count(e => !e.IsActive),
                TotalPayroll = _context.Employees.Sum(e => e.BaseSalary),
                AverageSalary = _context.Employees.Average(e => e.BaseSalary),
                DepartmentsCount = _context.Departments.Count()
            };

            return View(model);
        }
    }
}
