using DAL.Entities;

namespace DAL.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(int id);
    Task<List<Booking>> GetByStudentIdAsync(int studentId);
    Task<List<Booking>> GetByRoomIdAsync(int roomId);
    Task<Booking> CreateAsync(Booking booking);
    Task<bool> UpdateAsync(Booking booking);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> HasTimeConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    Task<List<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(int studentId);
    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int roomId);
    Task<List<Instructor>> GetAllInstructorsAsync();
    Task<List<int>> MarkExpiredBookingsAsNoShowAsync();
}
