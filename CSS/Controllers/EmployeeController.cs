using Microsoft.AspNetCore.Mvc;
using HRManagement.Repositories; // <-- MUST add this
using HRManagement.Models;       // optional, if you reference Employee directly
using HRManagement.Data;
using Microsoft.EntityFrameworkCore;


namespace HRManagement.Controllers
{

    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _repository;
        private readonly ApplicationDbContext _context;

        public EmployeesController(IEmployeeRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = 25)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Where(e => e.IsActive);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var safePage = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var employees = query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((safePage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = safePage;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(employees);
        }

        public IActionResult IncreaseSalary(int id)
        {
            _repository.IncreaseSalary(id, 10); // +10%
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ByDepartment(int? departmentId, int page = 1, int pageSize = 25)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Where(e => e.IsActive);

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var safePage = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var employees = query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((safePage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = safePage;
            ViewBag.TotalPages = totalPages;
            ViewBag.DepartmentId = departmentId;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(employees);
        }

        public IActionResult DepartmentSummary()
        {
            var summary = _context.DepartmentSalarySummaries
                .AsNoTracking()
                .ToList();

            return View(summary);
        }
    }
}