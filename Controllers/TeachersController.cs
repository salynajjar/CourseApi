using CourseApi.Data;
using CourseApi.DTOs;
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
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;


        public TeachersController(AppDbContext context)
        {
            _context = context;
        }


        // GET 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
        {
            var teachers = await _context.Teachers
                .Select(t => new TeacherDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email,

                    Courses = t.Courses
                        .Select(c => c.Title)
                        .ToList()
                })
                .ToListAsync();


            return Ok(teachers);
        }

        // GET 
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Where(t => t.Id == id)
                .Select(t => new TeacherDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email
                })
                .FirstOrDefaultAsync();


            if (teacher == null)
            {
                return NotFound();
            }


            return Ok(teacher);
        }



        // POST
        [HttpPost]
        public async Task<ActionResult<TeacherDto>> CreateTeacher(CreateTeacherVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacher = new Teacher
            {
                Name = model.Name,
                Email = model.Email
            };


            _context.Teachers.Add(teacher);

            await _context.SaveChangesAsync();


            var teacherDto = new TeacherDto
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Email = teacher.Email
            };


            return CreatedAtAction(
                nameof(GetTeacher),
                new { id = teacher.Id },
                teacherDto
            );
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(
            int id,
            CreateTeacherVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacher = await _context.Teachers
                .FindAsync(id);



            if (teacher == null)
            {
                return NotFound();
            }



            teacher.Name = model.Name;

            teacher.Email = model.Email;



            await _context.SaveChangesAsync();


            return NoContent();
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {

            var teacher = await _context.Teachers
                .FindAsync(id);



            if (teacher == null)
            {
                return NotFound();
            }



            _context.Teachers.Remove(teacher);


            await _context.SaveChangesAsync();



            return NoContent();
        }




    }
}