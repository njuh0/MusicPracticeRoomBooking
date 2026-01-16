using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<RoomEquipment> RoomEquipments => Set<RoomEquipment>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<RoomEquipment>(entity =>
        {
            entity.HasKey(re => new { re.RoomId, re.EquipmentId });
            
            entity.HasOne(re => re.Room)
                .WithMany(r => r.RoomEquipments)
                .HasForeignKey(re => re.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(re => re.Equipment)
                .WithMany(e => e.RoomEquipments)
                .HasForeignKey(re => re.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(200);
            entity.Property(s => s.StudentNumber).IsRequired().HasMaxLength(20);
            
            entity.HasIndex(s => s.Email).IsUnique();
            entity.HasIndex(s => s.StudentNumber).IsUnique();
            
            entity.HasOne(s => s.Instructor)
                .WithMany(i => i.Students)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Ignore calculated properties
            entity.Ignore(s => s.WeeklyQuotaHours);
            entity.Ignore(s => s.EffectiveWeeklyQuota);
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(i => i.LastName).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(i => i.Email).IsUnique();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            
            entity.HasOne(b => b.Student)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(b => b.ApprovedByInstructor)
                .WithMany(i => i.ApprovedBookings)
                .HasForeignKey(b => b.ApprovedByInstructorId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(b => b.StartTime);
            entity.HasIndex(b => new { b.RoomId, b.StartTime });
            entity.HasIndex(b => new { b.StudentId, b.StartTime });
            
            // Ignore calculated property
            entity.Ignore(b => b.DurationHours);
        });
    }
}