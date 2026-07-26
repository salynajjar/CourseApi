namespace CourseApi.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int CreditHours { get; set; }

        public decimal Price { get; set; }


        public string TeacherName { get; set; }
    }
}