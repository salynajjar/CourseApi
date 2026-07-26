using CourseApi.Enums;

namespace CourseApi.DTOs
{
    public class StudentCourseDto
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        public int CreditHours { get; set; }

        public decimal Price { get; set; }

        public EnrollmentStatus EnrollmentStatus { get; set; }

        public PassStatus PassStatus { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }
    }
}