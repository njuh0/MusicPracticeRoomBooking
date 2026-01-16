namespace DAL.Entities;

public class Equipment
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public EquipmentType Type { get; set; }
    public string? Description { get; set; }
    
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
