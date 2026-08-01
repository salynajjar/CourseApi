namespace CourseApi.Models
{
    public class CoursePrerequisite
    {
        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;


        public int PrerequisiteCourseId { get; set; }

        public Course PrerequisiteCourse { get; set; } = null!;
    }
}