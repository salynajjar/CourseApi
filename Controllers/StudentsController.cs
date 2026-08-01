using CourseApi.Data;
using CourseApi.DTOs;
using CourseApi.Enums;
using CourseApi.Models;
using CourseApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const int MaxPageSize = 50;


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


        // GET: api/Students/courses/search?studentName=...&courseName=...
        [HttpGet("courses/search")]
        public async Task<IActionResult> SearchStudentCourses(
            [FromQuery] StudentCourseSearchVM model)
        {
            var query = _context.StudentCourses
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.StudentName))
            {
                query = query.Where(sc =>
                    sc.Student.Name.Contains(model.StudentName));
            }

            if (!string.IsNullOrWhiteSpace(model.CourseName))
            {
                query = query.Where(sc =>
                    sc.Course.Title.Contains(model.CourseName));
            }

            var totalRecords = await query.CountAsync();

            if (model.PageNumber < 1)
            {
                model.PageNumber = 1;
            }

            if (model.PageSize < 1)
            {
                model.PageSize = 10;
            }

            if (model.PageSize > MaxPageSize)
            {
                model.PageSize = MaxPageSize;
            }

            var courses = await query
                .OrderBy(sc => sc.Student.Name)
                .ThenBy(sc => sc.Course.Title)
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(sc => new StudentCourseSearchDto
                {
                    StudentName = sc.Student.Name,
                    CourseTitle = sc.Course.Title,
                    EnrollmentStatus = sc.EnrollmentStatus,
                    PassStatus = sc.PassStatus,
                    EnrollmentDate = sc.EnrollmentDate,
                    CompletionDate = sc.CompletionDate
                })
                .ToListAsync();

            var result = new PaginatedResultDto<StudentCourseSearchDto>
            {
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(
                    totalRecords / (double)model.PageSize),
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                Data = courses
            };

            return Ok(result);
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
        [Authorize(Roles = "Admin,Teacher")]
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
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> AddCourseToStudent(
            int studentId,
            AddCourseToStudentVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                    sc.EnrollmentStatus == EnrollmentStatus.Completed &&
                    sc.PassStatus == PassStatus.Passed)
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
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == studentId);

            if (!studentExists)
            {
                return NotFound(new { message = "Student not found." });
            }

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
        [Authorize(Roles = "Admin,Teacher")]
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
        [Authorize(Roles = "Admin")]
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


        // PUT: api/Students/{studentId}/courses/{courseId}/status
        [HttpPut("{studentId}/courses/{courseId}/status")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> UpdateCourseStatus(
            int studentId,
            int courseId,
            UpdateStudentCourseStatusVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentCourse = await _context.StudentCourses
                .FirstOrDefaultAsync(sc =>
                    sc.StudentId == studentId &&
                    sc.CourseId == courseId);

            //check 

            if (studentCourse == null)
            {
                return NotFound(new
                {
                    message = "Student is not enrolled in this course."
                });
            }

            if (!Enum.IsDefined(typeof(EnrollmentStatus), model.EnrollmentStatus))
            {
                return BadRequest(new
                {
                    message = "Invalid enrollment status value."
                });
            }

            if (!Enum.IsDefined(typeof(PassStatus), model.PassStatus))
            {
                return BadRequest(new
                {
                    message = "Invalid pass status value."
                });
            }

            if (model.EnrollmentStatus == EnrollmentStatus.NotStarted &&
                model.PassStatus != PassStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "NotStarted course must have Pending pass status."
                });
            }

            if (model.EnrollmentStatus == EnrollmentStatus.InProgress &&
                model.PassStatus != PassStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "InProgress course must have Pending pass status."
                });
            }

            if (model.EnrollmentStatus == EnrollmentStatus.Withdrawn &&
                model.PassStatus != PassStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "Withdrawn course must have Pending pass status."
                });
            }

            if (model.EnrollmentStatus == EnrollmentStatus.Completed &&
                model.PassStatus == PassStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "Completed course must be Passed or Failed."
                });
            }

            studentCourse.EnrollmentStatus = model.EnrollmentStatus;
            studentCourse.PassStatus = model.PassStatus;

            if (model.EnrollmentStatus == EnrollmentStatus.Completed)
            {
                studentCourse.CompletionDate = DateTime.UtcNow;
            }
            else
            {
                studentCourse.CompletionDate = null;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Course status updated successfully."
            });
        }

    }
}