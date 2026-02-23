namespace HRManagement.Models
{
    public class DashboardKpiViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public decimal TotalPayroll { get; set; }
        public decimal AverageSalary { get; set; }
        public int DepartmentsCount { get; set; }
    }
}
