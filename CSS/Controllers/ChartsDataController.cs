using Microsoft.AspNetCore.Mvc;
using HRManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChartsDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("employees-by-department")]
        public async Task<IActionResult> EmployeesByDepartment()
        {
            var data = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .GroupBy(e => e.Department.DepartmentName)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .OrderBy(x => x.Department)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("avg-salary-by-department")]
        public async Task<IActionResult> AvgSalaryByDepartment()
        {
            var data = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .GroupBy(e => e.Department.DepartmentName)
                .Select(g => new { Department = g.Key, AvgSalary = g.Average(e => e.BaseSalary) })
                .OrderBy(x => x.Department)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("total-payroll-by-department")]
        public async Task<IActionResult> TotalPayrollByDepartment()
        {
            var data = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .GroupBy(e => e.Department.DepartmentName)
                .Select(g => new { Department = g.Key, TotalPayroll = g.Sum(e => e.BaseSalary) })
                .OrderBy(x => x.Department)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("hiring-trend")]
        public async Task<IActionResult> HiringTrend()
        {
            var rawData = await _context.Employees
                .AsNoTracking()
                .GroupBy(e => new { Year = e.HireDate.Year, Month = e.HireDate.Month })
                .Select(g => new 
                { 
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count() 
                })
                .ToListAsync();

            var data = rawData
                .Select(x => new { YearMonth = x.Year + "-" + x.Month.ToString("D2"), x.Count })
                .OrderBy(x => x.YearMonth)
                .ToList();

            return Ok(data);
        }

        [HttpGet("salary-distribution")]
        public async Task<IActionResult> SalaryDistribution()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .ToListAsync();

            var salaryBuckets = new Dictionary<string, int>
            {
                { "< 3000", 0 },
                { "3000 - 4999", 0 },
                { "5000 - 6999", 0 },
                { "7000 - 8999", 0 },
                { "9000+", 0 }
            };

            foreach (var emp in employees)
            {
                if (emp.BaseSalary < 3000) salaryBuckets["< 3000"]++;
                else if (emp.BaseSalary < 5000) salaryBuckets["3000 - 4999"]++;
                else if (emp.BaseSalary < 7000) salaryBuckets["5000 - 6999"]++;
                else if (emp.BaseSalary < 9000) salaryBuckets["7000 - 8999"]++;
                else salaryBuckets["9000+"]++;
            }

            return Ok(salaryBuckets);
        }

        [HttpGet("attendance-by-status")]
        public async Task<IActionResult> AttendanceByStatus()
        {
            var data = await _context.Attendance
                .AsNoTracking()
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .GroupBy(a => new { Department = a.Employee.Department.DepartmentName, Status = a.Status })
                .Select(g => new 
                { 
                    Department = g.Key.Department,
                    Status = g.Key.Status,
                    Count = g.Count() 
                })
                .OrderBy(x => x.Department)
                .ThenBy(x => x.Status)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("salary-vs-performance")]
        public async Task<IActionResult> SalaryVsPerformance()
        {
            var data = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .GroupBy(e => e.Department.DepartmentName)
                .Select(g => new 
                { 
                    Department = g.Key,
                    AvgPerformanceRating = g.Average(e => e.Id) > 0 ? 
                        _context.PerformanceReviews
                            .Where(pr => g.Select(x => x.Id).Contains(pr.EmployeeId))
                            .Average(pr => pr.Rating) : 0,
                    AvgSalary = g.Average(e => e.BaseSalary)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("salary-change-history")]
        public async Task<IActionResult> SalaryChangeHistory()
        {
            var rawData = await _context.SalaryHistory
                .AsNoTracking()
                .GroupBy(sh => new { Year = sh.ChangeDate.Year, Month = sh.ChangeDate.Month })
                .Select(g => new 
                { 
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    AvgNewSalary = g.Average(sh => sh.NewSalary),
                    AvgOldSalary = g.Average(sh => sh.OldSalary)
                })
                .ToListAsync();

            var data = rawData
                .Select(x => new { YearMonth = x.Year + "-" + x.Month.ToString("D2"), x.AvgNewSalary, x.AvgOldSalary })
                .OrderBy(x => x.YearMonth)
                .ToList();

            return Ok(data);
        }
    }
}
