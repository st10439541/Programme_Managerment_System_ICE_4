using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ── DbSets (table mappings) ──────────────────────────
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<ModuleAssignment> ModuleAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Indexes ─────────────────────────────────────
            modelBuilder.Entity<Module>()
                .HasIndex(m => m.ModuleCode)
                .IsUnique()
                .HasDatabaseName("IX_Module_ModuleCode");

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique()
                .HasDatabaseName("IX_Student_Email");

            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.Email)
                .IsUnique()
                .HasDatabaseName("IX_Lecturer_Email");

            // ── Table names (optional but explicit) ─────────
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Lecturer>().ToTable("Lecturers");
            modelBuilder.Entity<Module>().ToTable("Modules");
            modelBuilder.Entity<Registration>().ToTable("Registrations");
            modelBuilder.Entity<ModuleAssignment>().ToTable("ModuleAssignments");

            // ── Relationships with cascade delete ───────────
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Student)
                .WithMany(s => s.Registrations)
                .HasForeignKey(r => r.StudentID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Module)
                .WithMany(m => m.Registrations)
                .HasForeignKey(r => r.ModuleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ModuleAssignment>()
                .HasOne(a => a.Lecturer)
                .WithMany(l => l.ModuleAssignments)
                .HasForeignKey(a => a.LecturerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ModuleAssignment>()
                .HasOne(a => a.Module)
                .WithMany(m => m.ModuleAssignments)
                .HasForeignKey(a => a.ModuleID)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Composite unique constraints (prevent duplicates) ──
            modelBuilder.Entity<Registration>()
                .HasIndex(r => new { r.StudentID, r.ModuleID })
                .IsUnique()
                .HasDatabaseName("IX_Registration_StudentModule");

            modelBuilder.Entity<ModuleAssignment>()
                .HasIndex(a => new { a.LecturerID, a.ModuleID })
                .IsUnique()
                .HasDatabaseName("IX_ModuleAssignment_LecturerModule");
        }
    }
}