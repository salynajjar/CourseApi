
using System.ComponentModel.DataAnnotations;
namespace CourseApi.Models
{
    public class Course
    {
        public int Id { get; set; }
        

        [Required (ErrorMessage="Course title is required.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

       

        [Range(1,6,ErrorMessage = "Credithours must be between 1 and 6 .")]

        public int CreditHours { get; set; }

        [Range (0,10000,ErrorMessage = "price must be greater than or equal to 0.")]
        public decimal Price { get; set; }


        // Foreign Key
        public int TeacherId { get; set; }

        // Navigation Property
        public Teacher Teacher { get; set; } = null!;

        public ICollection<StudentCourse> StudentCourses { get; set; }
            = new List<StudentCourse>();

        public ICollection<CoursePrerequisite> Prerequisites { get; set; }
            = new List<CoursePrerequisite>();

        public ICollection<CoursePrerequisite> RequiredFor { get; set; }
            = new List<CoursePrerequisite>();

    }

}