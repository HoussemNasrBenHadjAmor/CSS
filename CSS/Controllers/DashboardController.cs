using Microsoft.AspNetCore.Mvc;
using HRManagement.Data;
using System.Linq;

namespace HRManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalEmployees = _context.Employees.Count();
            ViewBag.ActiveEmployees = _context.Employees.Count(e => e.IsActive);
            ViewBag.TotalPayroll = _context.Employees.Sum(e => e.BaseSalary);
            ViewBag.AvgSalary = _context.Employees.Average(e => e.BaseSalary);

            return View();
        }
    }
}