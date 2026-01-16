namespace DAL.Entities;

public class RoomEquipment
{
    public int RoomId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; } = 1;
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    public Room Room { get; set; } = null!;
    public Equipment Equipment { get; set; } = null!;
}
