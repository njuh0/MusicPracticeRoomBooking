using DAL.Entities;
using DAL.Repositories;

namespace BLL.Services;

public class RoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<List<Room>> GetAllAsync()
    {
        return await _roomRepository.GetAllAsync();
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _roomRepository.GetByIdAsync(id);
    }

    public async Task<Room> CreateAsync(Room room)
    {
        // Business logic: Validate room name uniqueness
        var exists = await _roomRepository.ExistsByNameAsync(room.Name);
        if (exists)
        {
            throw new InvalidOperationException($"Room with name '{room.Name}' already exists.");
        }

        return await _roomRepository.CreateAsync(room);
    }

    public async Task<bool> UpdateAsync(Room room)
    {
        // Business logic: Validate room name uniqueness (excluding current room)
        var exists = await _roomRepository.ExistsByNameAsync(room.Name, room.Id);
        if (exists)
        {
            throw new InvalidOperationException($"Another room with name '{room.Name}' already exists.");
        }

        return await _roomRepository.UpdateAsync(room);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Business logic: Check if room has active bookings
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            return false;
        }

        // Allow soft delete even with bookings - they will be preserved
        return await _roomRepository.DeleteAsync(id);
    }

    public async Task<bool> RoomExistsAsync(int id)
    {
        return await _roomRepository.ExistsAsync(id);
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync()
    {
        return await _roomRepository.GetAllEquipmentAsync();
    }
}
