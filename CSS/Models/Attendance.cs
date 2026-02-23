namespace HRManagement.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }

        public Employee Employee { get; set; }
    }
}
