namespace HRManagement.Models
{
    public class DepartmentSalarySummary
    {
        public string Department { get; set; }
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal TotalSalary { get; set; }
    }
}
