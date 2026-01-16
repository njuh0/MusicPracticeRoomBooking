namespace DAL.Entities;

public class Student
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string StudentNumber { get; set; }
    public StudentProgram Program { get; set; }
    public InstrumentType PrimaryInstrument { get; set; }
    
    // Quota management
    public int WeeklyQuotaHours => Program switch
    {
        StudentProgram.PerformanceMajor => 20,
        StudentProgram.EducationMajor => 10,
        StudentProgram.Minor => 5,
        _ => 0
    };
    
    public int NoShowCount { get; set; } = 0;
    public int QuotaPenaltyHours { get; set; } = 0; // Reduced quota due to no-shows
    
    public int EffectiveWeeklyQuota => Math.Max(0, WeeklyQuotaHours - QuotaPenaltyHours);
    
    // For graduation tracking
    public int TotalLoggedHours { get; set; } = 0;
    
    // Mandate hours required for graduation
    public int MandateLoggedHours => Program switch
    {
        StudentProgram.PerformanceMajor => 200,
        StudentProgram.EducationMajor => 150,
        StudentProgram.Minor => 100,
        _ => 0
    };
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    // Navigation
    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum StudentProgram
{
    PerformanceMajor = 1,
    EducationMajor = 2,
    Minor = 3
}

public enum InstrumentType
{
    Piano = 1,
    Violin = 2,
    Cello = 3,
    Guitar = 4,
    Drums = 5,
    Trumpet = 6,
    Saxophone = 7,
    Flute = 8,
    Voice = 9,
    Other = 99
}
