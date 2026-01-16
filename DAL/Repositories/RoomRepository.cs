using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.RoomEquipments)
                .ThenInclude(re => re.Equipment)
            .Include(r => r.Bookings)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.RoomEquipments)
                .ThenInclude(re => re.Equipment)
            .Include(r => r.Bookings)
                .ThenInclude(b => b.Student)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room> CreateAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<bool> UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);
        
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ExistsAsync(room.Id))
            {
                return false;
            }
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null || room.IsDeleted)
        {
            return false;
        }

        // Soft delete
        room.IsDeleted = true;
        room.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Rooms.Where(r => r.Name.ToLower() == name.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync()
    {
        return await _context.Equipment
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}
