using DAL.Entities;
using DAL.Repositories;

namespace BLL.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IStudentRepository _studentRepository;

    public BookingService(IBookingRepository bookingRepository, IStudentRepository studentRepository)
    {
        _bookingRepository = bookingRepository;
        _studentRepository = studentRepository;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        // Automatically mark NoShows and apply penalties before returning bookings
        await MarkNoShowsAndApplyPenaltiesAsync();
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task<List<Booking>> GetByStudentIdAsync(int studentId)
    {
        await MarkNoShowsAndApplyPenaltiesAsync();
        return await _bookingRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<List<Booking>> GetByRoomIdAsync(int roomId)
    {
        return await _bookingRepository.GetByRoomIdAsync(roomId);
    }

    private async Task MarkNoShowsAndApplyPenaltiesAsync()
    {
        // Mark expired bookings as NoShow and get affected student IDs
        var studentIds = await _bookingRepository.MarkExpiredBookingsAsNoShowAsync();

        // Increment NoShowCount for each affected student (includes penalty logic)
        foreach (var studentId in studentIds)
        {
            await _studentRepository.IncrementNoShowCountAsync(studentId);
        }
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

        // Business logic: Validate booking duration based on purpose
        var duration = (booking.EndTime - booking.StartTime).TotalHours;
        var maxDuration = booking.Purpose switch
        {
            BookingPurpose.RegularPractice => 2,
            BookingPurpose.RecitalPrep => 3,
            BookingPurpose.EnsembleRehearsal => 4,
            _ => 2
        };

        if (duration > maxDuration)
        {
            throw new InvalidOperationException(
                $"{booking.Purpose} bookings cannot exceed {maxDuration} hours. Requested: {duration:F1} hours."
            );
        }

        // Business logic: Validate room type matches booking purpose
        var room = await _bookingRepository.GetRoomByIdAsync(booking.RoomId);
        if (room != null)
        {
            var isValidRoomType = booking.Purpose switch
            {
                BookingPurpose.RegularPractice => room.Type == RoomType.Small || room.Type == RoomType.Medium,
                BookingPurpose.RecitalPrep => room.Type == RoomType.Medium || room.Type == RoomType.Large,
                BookingPurpose.EnsembleRehearsal => room.Type == RoomType.Large,
                _ => true
            };

            if (!isValidRoomType)
            {
                var allowedTypes = booking.Purpose switch
                {
                    BookingPurpose.RegularPractice => "Small or Medium",
                    BookingPurpose.RecitalPrep => "Medium or Large",
                    BookingPurpose.EnsembleRehearsal => "Large",
                    _ => "any"
                };
                throw new InvalidOperationException(
                    $"{booking.Purpose} bookings require {allowedTypes} rooms. Selected room is {room.Type}."
                );
            }
        }

        // Business logic: Validate equipment matches student's instrument
        var student = await _bookingRepository.GetStudentByIdAsync(booking.StudentId);
        if (student != null && room != null)
        {
            // Check if student's instrument requires specific equipment
            var requiredEquipment = GetRequiredEquipmentForInstrument(student.PrimaryInstrument, booking.Purpose);
            if (requiredEquipment != null)
            {
                var roomEquipmentTypes = room.RoomEquipments
                    .Where(re => !re.Equipment.IsDeleted)
                    .Select(re => re.Equipment.Type)
                    .ToList();

                var hasRequiredEquipment = requiredEquipment.Any(req => roomEquipmentTypes.Contains(req));

                if (!hasRequiredEquipment)
                {
                    var equipmentNames = string.Join(" or ", requiredEquipment.Select(e => e.ToString()));
                    throw new InvalidOperationException(
                        $"Student plays {student.PrimaryInstrument}. Selected room must have {equipmentNames}."
                    );
                }
            }

            // Check soundproofing for loud instruments
            if (RequiresSoundproofing(student.PrimaryInstrument) && !room.IsSoundproof)
            {
                throw new InvalidOperationException(
                    $"{student.PrimaryInstrument} requires a soundproof room to avoid disturbing others."
                );
            }
        }

        // Business logic: Check weekly quota
        if (student != null)
        {
            var weekStart = GetStartOfWeek(DateTime.UtcNow);
            var weekEnd = weekStart.AddDays(7);

            var weeklyBookings = await _bookingRepository.GetByStudentIdAsync(booking.StudentId);
            var weeklyHours = weeklyBookings
                .Where(b => b.Status != BookingStatus.Cancelled 
                         && b.StartTime >= weekStart 
                         && b.StartTime < weekEnd)
                .Sum(b => (b.EndTime - b.StartTime).TotalHours);

            var newBookingHours = (booking.EndTime - booking.StartTime).TotalHours;
            var totalHours = weeklyHours + newBookingHours;

            if (totalHours > student.EffectiveWeeklyQuota)
            {
                throw new InvalidOperationException(
                    $"Weekly quota exceeded: {totalHours:F1}/{student.EffectiveWeeklyQuota} hours. " +
                    $"This booking requires {newBookingHours:F1} hours.");
            }
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

        // Business logic: RecitalPrep requires instructor approval
        if (booking.Purpose == BookingPurpose.RecitalPrep)
        {
            booking.RequiresApproval = true;
        }

        // Set default status
        booking.Status = BookingStatus.Confirmed;

        return await _bookingRepository.CreateAsync(booking);
    }

    private DateTime GetStartOfWeek(DateTime date)
    {
        // Start of week is Monday
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
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

        // Check if booking requires approval and is not yet approved
        if (booking.RequiresApproval && !booking.IsApproved)
        {
            throw new InvalidOperationException("Cannot check in: This booking requires instructor approval first.");
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

    private List<EquipmentType>? GetRequiredEquipmentForInstrument(InstrumentType instrument, BookingPurpose purpose)
    {
        return instrument switch
        {
            InstrumentType.Piano when purpose == BookingPurpose.RecitalPrep => 
                new List<EquipmentType> { EquipmentType.GrandPiano },
            InstrumentType.Piano => 
                new List<EquipmentType> { EquipmentType.GrandPiano, EquipmentType.UprightPiano },
            InstrumentType.Drums => 
                new List<EquipmentType> { EquipmentType.Drums },
            InstrumentType.Violin => 
                new List<EquipmentType> { EquipmentType.Violin },
            InstrumentType.Cello => 
                new List<EquipmentType> { EquipmentType.Cello },
            InstrumentType.Guitar => 
                new List<EquipmentType> { EquipmentType.Guitar },
            InstrumentType.Trumpet => 
                new List<EquipmentType> { EquipmentType.Trumpet },
            InstrumentType.Saxophone => 
                new List<EquipmentType> { EquipmentType.Saxophone },
            InstrumentType.Flute => 
                new List<EquipmentType> { EquipmentType.Flute },
            _ => null // Voice and Other don't require specific room equipment
        };
    }

    private bool RequiresSoundproofing(InstrumentType instrument)
    {
        return instrument switch
        {
            InstrumentType.Drums => true,
            InstrumentType.Trumpet => true,
            InstrumentType.Saxophone => true,
            _ => false
        };
    }
}
