using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagement.Data;

namespace HRManagement.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("salary-by-department")]
        public async Task<IActionResult> SalaryByDepartment()
        {
            var data = await _context.Employees
                .GroupBy(e => e.Department.DepartmentName)
                .Select(g => new
                {
                    Department = g.Key,
                    AverageSalary = g.Average(e => e.BaseSalary)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("hiring-trend")]
        public async Task<IActionResult> HiringTrend()
        {
            var data = await _context.Employees
                .GroupBy(e => new { e.HireDate.Year, e.HireDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Year + "-" + g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return Ok(data);
        }
    }
}