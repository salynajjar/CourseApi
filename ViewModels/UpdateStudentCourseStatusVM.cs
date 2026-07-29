using CourseApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseApi.ViewModels
{
    public class UpdateStudentCourseStatusVM
    {
        [Required]
        public EnrollmentStatus EnrollmentStatus { get; set; }

        [Required]
        public PassStatus PassStatus { get; set; }
    }
}