namespace DAL.Entities;

public class Booking
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int RoomId { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public BookingPurpose Purpose { get; set; }
    
    // For recital prep blocks
    public bool RequiresApproval { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public int? ApprovedByInstructorId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    // Track no-shows
    public bool IsNoShow { get; set; } = false;
    public DateTime? CheckedInAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Calculated property
    public int DurationHours => (int)Math.Ceiling((EndTime - StartTime).TotalHours);
    
    // Navigation
    public Student Student { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public Instructor? ApprovedByInstructor { get; set; }
}

public enum BookingStatus
{
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3,
    NoShow = 4
}

public enum BookingPurpose
{
    RegularPractice = 1,
    RecitalPrep = 2,
    EnsembleRehearsal = 3
}
