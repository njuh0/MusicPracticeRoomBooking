using DAL.Entities;
using DAL.Repositories;

namespace BLL.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task<List<Booking>> GetByStudentIdAsync(int studentId)
    {
        return await _bookingRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<List<Booking>> GetByRoomIdAsync(int roomId)
    {
        return await _bookingRepository.GetByRoomIdAsync(roomId);
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        // Business logic: Validate time is in future
        if (booking.StartTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Booking start time must be in the future.");
        }

        // Business logic: Validate EndTime is after StartTime
        if (booking.EndTime <= booking.StartTime)
        {
            throw new InvalidOperationException("End time must be after start time.");
        }

        // Business logic: Check for room time conflicts
        var hasConflict = await _bookingRepository.HasTimeConflictAsync(
            booking.RoomId,
            booking.StartTime,
            booking.EndTime
        );

        if (hasConflict)
        {
            throw new InvalidOperationException($"The room is already booked for the selected time slot.");
        }

        // Business logic: RecitalPrep must be booked at least 3 hours in advance
        if (booking.Purpose == BookingPurpose.RecitalPrep)
        {
            var hoursUntilBooking = (booking.StartTime - DateTime.UtcNow).TotalHours;
            if (hoursUntilBooking < 3)
            {
                throw new InvalidOperationException("Recital prep bookings must be made at least 3 hours in advance.");
            }

            booking.RequiresApproval = true;
        }

        // Set default status
        booking.Status = BookingStatus.Confirmed;

        return await _bookingRepository.CreateAsync(booking);
    }

    public async Task<bool> UpdateAsync(Booking booking)
    {
        // Business logic: Check time conflict (excluding current booking)
        if (booking.Status != BookingStatus.Cancelled)
        {
            var hasConflict = await _bookingRepository.HasTimeConflictAsync(
                booking.RoomId,
                booking.StartTime,
                booking.EndTime,
                booking.Id
            );

            if (hasConflict)
            {
                throw new InvalidOperationException("The room is already booked for the selected time slot.");
            }
        }

        return await _bookingRepository.UpdateAsync(booking);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _bookingRepository.DeleteAsync(id);
    }

    public async Task<bool> CancelBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            return false;
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        return await _bookingRepository.UpdateAsync(booking);
    }

    public async Task<bool> CheckInAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            return false;
        }

        booking.CheckedInAt = DateTime.UtcNow;
        booking.Status = BookingStatus.Completed;
        return await _bookingRepository.UpdateAsync(booking);
    }

    public async Task<bool> ApproveBookingAsync(int id, int instructorId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            return false;
        }

        if (!booking.RequiresApproval)
        {
            throw new InvalidOperationException("This booking does not require approval.");
        }

        booking.IsApproved = true;
        booking.ApprovedByInstructorId = instructorId;
        booking.ApprovedAt = DateTime.UtcNow;
        return await _bookingRepository.UpdateAsync(booking);
    }

    public async Task<bool> BookingExistsAsync(int id)
    {
        return await _bookingRepository.ExistsAsync(id);
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        return await _bookingRepository.GetAllStudentsAsync();
    }

    public async Task<List<Room>> GetAllRoomsAsync()
    {
        return await _bookingRepository.GetAllRoomsAsync();
    }

    public async Task<List<Instructor>> GetAllInstructorsAsync()
    {
        return await _bookingRepository.GetAllInstructorsAsync();
    }
}
