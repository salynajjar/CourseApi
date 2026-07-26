using CourseApi.Data;
using CourseApi.DTOs;
using CourseApi.Models;
using CourseApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ILogger<CoursesController> _logger;


        public CoursesController(
            AppDbContext context,
            ILogger<CoursesController> logger)
        {
            _context = context;
            _logger = logger;
        }



        // GET: api/courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            _logger.LogInformation("Fetching all courses.");

            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    Price = c.Price,
                    TeacherName = c.Teacher.Name
                })
                .ToListAsync();


            return Ok(courses);
        }



        // GET: api/courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {

            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Where(c => c.Id == id)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    Price = c.Price,
                    TeacherName = c.Teacher.Name
                })
                .FirstOrDefaultAsync();



            if (course == null)
            {
                return NotFound();
            }


            return Ok(course);
        }





        // POST: api/courses
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(
         CreateCourseVM model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            // Check Teacher exists
            var teacherExists = await _context.Teachers
                .AnyAsync(t => t.Id == model.TeacherId);



            if (!teacherExists)
            {
                return BadRequest("Teacher does not exist.");
            }



            var course = new Course
            {
                Title = model.Title,
                CreditHours = model.CreditHours,
                Price = model.Price,
                TeacherId = model.TeacherId
            };



            _context.Courses.Add(course);

            await _context.SaveChangesAsync();

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                CreditHours = course.CreditHours,
                Price = course.Price,
                TeacherName = (await _context.Teachers
                    .Where(t => t.Id == course.TeacherId)
                    .Select(t => t.Name)
                    .FirstAsync())
            };


            _logger.LogInformation(
                "Course created successfully. Course ID: {Id}",
                course.Id);



            return CreatedAtAction(
          nameof(GetCourse),
          new { id = course.Id },
          courseDto);
        }





        // PUT: api/courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(
            int id,
            CreateCourseVM model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            var course = await _context.Courses
                .FindAsync(id);



            if (course == null)
            {
                return NotFound();
            }



            var teacherExists = await _context.Teachers
                .AnyAsync(t => t.Id == model.TeacherId);



            if (!teacherExists)
            {
                return BadRequest("Teacher does not exist.");
            }



            course.Title = model.Title;
            course.CreditHours = model.CreditHours;
            course.Price = model.Price;
            course.TeacherId = model.TeacherId;



            await _context.SaveChangesAsync();



            _logger.LogInformation(
                "Course updated successfully. Course ID: {Id}",
                id);



            return NoContent();
        }



        // DELETE: api/courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {

            var course = await _context.Courses
                .FindAsync(id);



            if (course == null)
            {
                return NotFound();
            }



            _context.Courses.Remove(course);


            await _context.SaveChangesAsync();



            _logger.LogWarning(
                "Course deleted. Course ID: {Id}",
                id);



            return NoContent();
        }




        // SEARCH
        // GET: api/courses/search?title=java
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> SearchCourses(
            string title)
        {

            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Where(c => c.Title.Contains(title))
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    Price = c.Price,
                    TeacherName = c.Teacher.Name
                })
                .ToListAsync();


            return Ok(courses);
        }


        // FILTER
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> FilterCourses(
            decimal minPrice,
            decimal maxPrice)
        {

            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Where(c =>
                    c.Price >= minPrice &&
                    c.Price <= maxPrice)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    Price = c.Price,
                    TeacherName = c.Teacher.Name
                })
                .ToListAsync();


            return Ok(courses);
        }



        // GE
        [HttpGet("{courseId}/prerequisites")]
        public async Task<IActionResult> GetPrerequisites(int courseId)
        {
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == courseId);


            if (!courseExists)
            {
                return NotFound("Course not found");
            }


            var prerequisites = await _context.CoursePrerequisites
                .Where(cp => cp.CourseId == courseId)
                .Select(cp => new CoursePrerequisiteDto
                {
                    Id = cp.PrerequisiteCourse.Id,
                    Title = cp.PrerequisiteCourse.Title
                })
                .ToListAsync();


            return Ok(prerequisites);
        }




        // POST

        [HttpPost("{courseId}/prerequisites")]
        public async Task<IActionResult> AddPrerequisite(
    int courseId,
    AddPrerequisiteVM model)
        {
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists)
                return NotFound("Course not found");


            var prerequisiteExists = await _context.Courses
                .AnyAsync(c => c.Id == model.PrerequisiteCourseId);

            if (!prerequisiteExists)
                return NotFound("Prerequisite course not found");


            if (courseId == model.PrerequisiteCourseId)
                return BadRequest(
                    "Course cannot be its own prerequisite");


            var exists = await _context.CoursePrerequisites
                .AnyAsync(cp =>
                    cp.CourseId == courseId &&
                    cp.PrerequisiteCourseId == model.PrerequisiteCourseId);


            if (exists)
                return BadRequest(
                    "Prerequisite already exists");


            var prerequisite = new CoursePrerequisite
            {
                CourseId = courseId,
                PrerequisiteCourseId = model.PrerequisiteCourseId
            };


            _context.CoursePrerequisites.Add(prerequisite);

            await _context.SaveChangesAsync();


            return Ok("Prerequisite added successfully");
        }


        // DELETE: api/Courses/5/prerequisites/2
        [HttpDelete("{courseId}/prerequisites/{prerequisiteId}")]
        public async Task<IActionResult> RemovePrerequisite(
            int courseId,
            int prerequisiteId)
        {
            var prerequisite = await _context.CoursePrerequisites
                .FirstOrDefaultAsync(cp =>
                    cp.CourseId == courseId &&
                    cp.PrerequisiteCourseId == prerequisiteId);


            if (prerequisite == null)
            {
                return NotFound(
                    "Prerequisite relationship not found");
            }


            _context.CoursePrerequisites.Remove(prerequisite);

            await _context.SaveChangesAsync();


            return Ok("Prerequisite removed");
        }




    }
}