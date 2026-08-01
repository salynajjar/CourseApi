using System.ComponentModel.DataAnnotations;

namespace CourseApi.ViewModels
{
    public class AddCourseToStudentVM
    {
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }
    }
}
