using CourseApi.Enums;

namespace CourseApi.Models
{
    public class StudentCourse
    {
        public int StudentId { get; set; }

        public Student Student { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public EnrollmentStatus EnrollmentStatus { get; set; }

        public PassStatus PassStatus { get; set; }
    }
}