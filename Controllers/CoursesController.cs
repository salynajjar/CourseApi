using CourseApi.Data;
using CourseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CourseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;


        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        //GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        //POST
        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Courses.Add(course);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse),
                new { id = course.Id },
                course);
        }

        //GET by ID 
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, Course updatedCourse)
        {
            if (id != updatedCourse.Id)
            {
                return BadRequest("Course ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            course.Title = updatedCourse.Title;
            course.Instructor = updatedCourse.Instructor;
            course.CreditHours = updatedCourse.CreditHours;
            course.Price = updatedCourse.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
     public async Task<IActionResult> DeleteCourse(int id )
        {
            var course = await _context.Courses.FindAsync(id);

            if (course==null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        //SEARCH
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Course>>> SearchCourses(string title)
        {
            var courses = await _context.Courses
                .Where(c => c.Title.Contains(title))
                .ToListAsync();

            return courses;
        }

        //FILTERING
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Course>>> FilterCourses(
    decimal minPrice,
    decimal maxPrice)
        {
            var courses = await _context.Courses
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .ToListAsync();

            return courses;
        }


    }
}