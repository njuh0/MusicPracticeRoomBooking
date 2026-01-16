namespace DAL.Entities;

public class Equipment
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public EquipmentType Type { get; set; }
    public string? Description { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    // Navigation
    public ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
}

public enum EquipmentType
{
    GrandPiano = 1,
    UprightPiano = 2,
    Drums = 3,
    Amplifier = 4,
    Microphone = 5,
    MusicStand = 6
}
