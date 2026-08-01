using System.ComponentModel.DataAnnotations;

namespace CourseApi.Models
{
    public class Student
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public ICollection<StudentCourse> StudentCourses { get; set; }
            = new List<StudentCourse>();

    }
}