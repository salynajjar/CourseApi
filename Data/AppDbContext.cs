using CourseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
              .Property(c => c.Price)
              .HasPrecision(10, 2);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            modelBuilder.Entity<StudentCourse>()
    .Property(sc => sc.EnrollmentStatus)
    .HasConversion<string>();


            modelBuilder.Entity<StudentCourse>()
                .Property(sc => sc.PassStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<CoursePrerequisite>()
          .HasKey(cp => new
     {
    cp.CourseId,
    cp.PrerequisiteCourseId
});

            modelBuilder.Entity<CoursePrerequisite>()
.HasOne(cp => cp.Course)
.WithMany(c => c.Prerequisites)
.HasForeignKey(cp => cp.CourseId)
.OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CoursePrerequisite>()
            .HasOne(cp => cp.PrerequisiteCourse)
            .WithMany(c => c.RequiredFor)
            .HasForeignKey(cp => cp.PrerequisiteCourseId)
            .OnDelete(DeleteBehavior.Restrict);



        }
    }
}