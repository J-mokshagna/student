namespace StudentEnrollmentAPI.DTOs
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int CourseId { get; set; }
    }

    public class CreateAssignmentDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int CourseId { get; set; }
    }

    public class SubmissionDTO
    {
        public int SubmissionId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int AssignmentId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? Grade { get; set; }
        public string? Comments { get; set; }
    }

    public class CreateSubmissionDTO
    {
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
    }
}