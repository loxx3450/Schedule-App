using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Storage
{
    public class DataContext : DbContext
    {
        public DataContext()
        { }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Adding unique fields
            modelBuilder.Entity<Classroom>()
                .HasIndex(c => c.Title)
                .IsUnique();

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.Title)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.Username)
                .IsUnique();

            modelBuilder.Entity<Group>()
                .HasIndex(t => t.Title)
                .IsUnique();

            modelBuilder.Entity<LessonStatus>()
                .HasIndex(t => t.Description)
                .IsUnique();

            // Check constraint: Lesson's end time should be bigger than start one
            modelBuilder.Entity<Lesson>()
                .ToTable(t => t.HasCheckConstraint("CK_Lesson_Time_Range", "\"EndsAt\" > \"StartsAt\""));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=schedule_app_db;Username=postgres;Password=local;")
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging();
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonStatus> LessonStatuses { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<GroupTeacher> GroupsTeachers { get; set; }
    }
}
