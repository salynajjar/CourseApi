
using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;
namespace CourseApi.Models
{
    public class Course
    {
        public int Id { get; set; }
        

        [Required (ErrorMessage="Course title is required.")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required (ErrorMessage ="Instructor name is required.")]
        [StringLength(50)]
        public string Instructor { get; set; }

        [Range(1,6,ErrorMessage = "Credithours must be between 1 and 6 .")]

        public int CreditHours { get; set; }

        [Range (0,10000,ErrorMessage = "price must be greater than or equal to 0.")]
        public decimal Price { get; set; }



    }
}