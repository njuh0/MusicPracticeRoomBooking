namespace DAL.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; } // e.g., "Room 101"
    public RoomType Type { get; set; }
    public int Capacity { get; set; } // Number of people
    public bool IsSoundproof { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum RoomType
{
    Small = 1,
    Medium = 2,
    Large = 3
}
