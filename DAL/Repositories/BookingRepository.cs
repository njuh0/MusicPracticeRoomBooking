using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Room)
            .Include(b => b.ApprovedByInstructor)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomEquipments)
                    .ThenInclude(re => re.Equipment)
            .Include(b => b.ApprovedByInstructor)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Booking>> GetByStudentIdAsync(int studentId)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.ApprovedByInstructor)
            .Where(b => b.StudentId == studentId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByRoomIdAsync(int roomId)
    {
        return await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.ApprovedByInstructor)
            .Where(b => b.RoomId == roomId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ExistsAsync(booking.Id))
            {
                return false;
            }
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null || booking.IsDeleted)
        {
            return false;
        }

        // Soft delete
        booking.IsDeleted = true;
        booking.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> HasTimeConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
    {
        var query = _context.Bookings
            .Where(b => b.RoomId == roomId)
            .Where(b => b.Status != BookingStatus.Cancelled) // Cancelled bookings don't block
            .Where(b => 
                // Check for overlapping time slots
                (b.StartTime < endTime && b.EndTime > startTime)
            );

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<(bool isAvailable, int currentBookings, int capacity)> CheckRoomAvailabilityAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
    {
        // Get room capacity
        var room = await _context.Rooms
            .Where(r => r.Id == roomId && !r.IsDeleted)
            .FirstOrDefaultAsync();

        if (room == null)
        {
            return (false, 0, 0);
        }

        // Count approved bookings in the time slot
        var query = _context.Bookings
            .Where(b => b.RoomId == roomId)
            .Where(b => b.Status != BookingStatus.Cancelled) // Exclude cancelled
            .Where(b => !b.RequiresApproval || b.IsApproved) // Only count approved bookings
            .Where(b => 
                // Check for overlapping time slots
                (b.StartTime < endTime && b.EndTime > startTime)
            );

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        var currentBookings = await query.CountAsync();
        var isAvailable = currentBookings < room.Capacity;

        return (isAvailable, currentBookings, room.Capacity);
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        return await _context.Students
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
    }

    public async Task<List<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.RoomEquipments)
                .ThenInclude(re => re.Equipment)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms
            .Include(r => r.RoomEquipments)
                .ThenInclude(re => re.Equipment)
            .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);
    }

    public async Task<List<Instructor>> GetAllInstructorsAsync()
    {
        return await _context.Instructors
            .OrderBy(i => i.LastName)
            .ThenBy(i => i.FirstName)
            .ToListAsync();
    }

    public async Task<List<int>> MarkExpiredBookingsAsNoShowAsync()
    {
        // Get bookings that need to be marked as NoShow before updating
        var expiredBookings = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed
                     && b.EndTime < DateTime.UtcNow
                     && !b.CheckedInAt.HasValue
                     && !b.IsDeleted)
            .Select(b => b.StudentId)
            .Distinct()
            .ToListAsync();

        // Update bookings to NoShow status
        await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed
                     && b.EndTime < DateTime.UtcNow
                     && !b.CheckedInAt.HasValue
                     && !b.IsDeleted)
            .ExecuteUpdateAsync(b => b.SetProperty(x => x.Status, BookingStatus.NoShow));

        return expiredBookings;
    }
}
