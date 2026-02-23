namespace HRManagement.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}