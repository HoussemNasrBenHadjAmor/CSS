namespace HRManagement.Models
{
    public class PerformanceReview
    {
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }

        public Employee Employee { get; set; }
    }
}
