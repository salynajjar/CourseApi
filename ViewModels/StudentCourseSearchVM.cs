namespace CourseApi.ViewModels
{
    public class StudentCourseSearchVM
    {
        public string? StudentName { get; set; }

        public string? CourseName { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}