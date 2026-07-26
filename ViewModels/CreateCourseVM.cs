using System.ComponentModel.DataAnnotations;

namespace CourseApi.ViewModels
{
    public class CreateCourseVM
    {

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }



        [Range(1, 6)]
        public int CreditHours { get; set; }



        [Range(0, 10000)]
        public decimal Price { get; set; }



        [Required]
        public int TeacherId { get; set; }

    }
}