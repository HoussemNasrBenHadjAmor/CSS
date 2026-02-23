using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HRManagement.Models;
using HRManagement.Data;

namespace HRManagement.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
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

        return RedirectToAction("Index", "DashboardFull");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
