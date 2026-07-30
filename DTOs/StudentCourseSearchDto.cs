using CourseApi.Enums;

namespace CourseApi.DTOs
{
    public class StudentCourseSearchDto
    {
        public string StudentName { get; set; } = string.Empty;

        public string CourseTitle { get; set; } = string.Empty;

        public EnrollmentStatus EnrollmentStatus { get; set; }

        public PassStatus PassStatus { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }
    }
}