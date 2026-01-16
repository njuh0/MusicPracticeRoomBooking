using DAL.Entities;

namespace DAL.Repositories;

public interface IRoomRepository
{
    Task<List<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(int id);
    Task<Room> CreateAsync(Room room);
    Task<bool> UpdateAsync(Room room);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<List<Equipment>> GetAllEquipmentAsync();
    Task UpdateRoomEquipmentAsync(int roomId, List<int> equipmentIds);
}
