using Microsoft.EntityFrameworkCore;
using CourseApi.Models;

namespace CourseApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }


        public DbSet<Course> Courses { get; set; }
    }
}