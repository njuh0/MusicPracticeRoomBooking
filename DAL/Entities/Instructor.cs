namespace DAL.Entities;

public class Instructor
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    // Navigation
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Booking> ApprovedBookings { get; set; } = new List<Booking>();
}
