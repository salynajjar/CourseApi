using CourseApi.Data;
using CourseApi.DTOs;
using CourseApi.Enums;
using CourseApi.Models;
using CourseApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseApi.Enums;

namespace CourseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;


        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _context.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,

                    Courses = s.StudentCourses
                        .Select(sc => sc.Course.Title)
                        .ToList()
                })
                .ToListAsync();


            return Ok(students);
        }



        // GET
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {

            var student = await _context.Students
                .Where(s => s.Id == id)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,

                    Courses = s.StudentCourses
                        .Select(sc => sc.Course.Title)
                        .ToList()
                })
                .FirstOrDefaultAsync();


            if (student == null)
            {
                return NotFound();
            }


            return Ok(student);
        }


        // POST
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(CreateStudentVM model)
        {
         
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var student = new Student
            {
                Name = model.Name,
                Email = model.Email
            };


            _context.Students.Add(student);

            await _context.SaveChangesAsync();


            return CreatedAtAction(
                nameof(GetStudent),
                new { id = student.Id },
                student
            );
        }



        // POST
        [HttpPost("{studentId}/courses")]
        public async Task<IActionResult> AddCourseToStudent(
            int studentId,
            AddCourseToStudentVM model)
        {

       
            var student = await _context.Students
                .FindAsync(studentId);


            if (student == null)
            {
                return NotFound("Student not found");
            }



            var course = await _context.Courses
                .FindAsync(model.CourseId);



            if (course == null)
            {
                return NotFound("Course not found");
            }



            var exists = await _context.StudentCourses
                .AnyAsync(sc =>
                    sc.StudentId == studentId &&
                    sc.CourseId == model.CourseId);



            if (exists)
            {
                return BadRequest(
                    "Student already enrolled in this course");
            }

            var prerequisites = await _context.CoursePrerequisites
    .Where(cp => cp.CourseId == model.CourseId)
    .Select(cp => cp.PrerequisiteCourseId)
    .ToListAsync();

            var completedCourses = await _context.StudentCourses
    .Where(sc =>
        sc.StudentId == studentId &&
        sc.EnrollmentStatus == EnrollmentStatus.Completed)
    .Select(sc => sc.CourseId)
    .ToListAsync();

            var missingPrerequisites = prerequisites
    .Except(completedCourses)
    .ToList();
           
            
            if (missingPrerequisites.Any())
            {

                var missingCoursesNames = await _context.Courses
                    .Where(c => missingPrerequisites.Contains(c.Id))
                    .Select(c => c.Title)
                    .ToListAsync();



                return BadRequest(new
                {
                    Message = "Student cannot enroll. Missing prerequisites.",
                    MissingCourses = missingCoursesNames
                });

            }

            //Create Enrollment
            var studentCourse = new StudentCourse
            {
                StudentId = studentId,

                CourseId = model.CourseId,

                EnrollmentDate = DateTime.UtcNow,

                CompletionDate = null,

                EnrollmentStatus = EnrollmentStatus.NotStarted,

                PassStatus = PassStatus.Pending
            };



            _context.StudentCourses.Add(studentCourse);


            await _context.SaveChangesAsync();



            return Ok(
                "Course added to student successfully");
        }

        // GET
        [HttpGet("{studentId}/courses")]
        public async Task<IActionResult> GetStudentCourses(int studentId)
        {

            var courses = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new StudentCourseDto
                {
                    CourseId = sc.CourseId,

                    CourseTitle = sc.Course.Title,

                    CreditHours = sc.Course.CreditHours,

                    Price = sc.Course.Price,

                    EnrollmentStatus = sc.EnrollmentStatus,

                    PassStatus = sc.PassStatus,

                    EnrollmentDate = sc.EnrollmentDate,

                    CompletionDate = sc.CompletionDate
                })
                .ToListAsync();



            return Ok(courses);
        }

        // PUT 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            CreateStudentVM model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var student = await _context.Students
                .FindAsync(id);



            if (student == null)
            {
                return NotFound();
            }



            student.Name = model.Name;

            student.Email = model.Email;



            await _context.SaveChangesAsync();



            return NoContent();
        }


        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {

            var student = await _context.Students
                .FindAsync(id);



            if (student == null)
            {
                return NotFound();
            }



            _context.Students.Remove(student);


            await _context.SaveChangesAsync();



            return NoContent();
        }

    }
}