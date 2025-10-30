namespace StudentEnrollmentAPI.Models
{
    public class AssignmentSubmission
    {
        public int SubmissionId { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } = "Pending";
        public int? Grade { get; set; }
        public string? Comments { get; set; }
    }
}