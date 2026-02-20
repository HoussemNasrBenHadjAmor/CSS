using System.ComponentModel.DataAnnotations;

namespace HRManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Department { get; set; }

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }
    }
}
