using System.ComponentModel.DataAnnotations;

namespace CourseApi.ViewModels
{
    public class CreateTeacherVM
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
