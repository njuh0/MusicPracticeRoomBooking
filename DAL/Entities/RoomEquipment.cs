namespace DAL.Entities;

public class RoomEquipment
{
    public int RoomId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; } = 1;
    
    public Room Room { get; set; } = null!;
    public Equipment Equipment { get; set; } = null!;
}
