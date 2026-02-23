namespace HRManagement.Models
{
    public class Employee
    {
        public required int Id { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required string FullName { get; set; } // computed column

        public required string Email { get; set; }
        public required string Phone { get; set; }

        public required int DepartmentId { get; set; }
        public required Department Department { get; set; }

        public required decimal BaseSalary { get; set; }

        public required DateTime HireDate { get; set; }

        public required bool IsActive { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required byte[] RowVersion { get; set; }
    }
}

// public class Employee
// {
//     public int Id { get; set; }
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
//     public string FullName { get; set; } // computed column
//     public string Email { get; set; }
//     public string Phone { get; set; }
//     public int DepartmentId { get; set; }
//     public Department Department { get; set; } // navigation property
//     public decimal BaseSalary { get; set; }
//     public DateTime HireDate { get; set; }
//     public bool IsActive { get; set; }
//     [Timestamp]
//     public byte[] RowVersion { get; set; }
// }