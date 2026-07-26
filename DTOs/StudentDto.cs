namespace CourseApi.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<string> Courses { get; set; } = new();
    }
    }