using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Pages.Bookings;

public class EditModel : PageModel
{
    private readonly BookingService _bookingService;

    public EditModel(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [BindProperty]
    public BookingInputModel Input { get; set; } = new();

    public SelectList StudentSelectList { get; set; } = null!;
    public SelectList RoomSelectList { get; set; } = null!;
    public SelectList InstructorSelectList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        Input = new BookingInputModel
        {
            Id = booking.Id,
            StudentId = booking.StudentId,
            RoomId = booking.RoomId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Purpose = booking.Purpose,
            ApprovedByInstructorId = booking.ApprovedByInstructorId
        };

        await LoadDropdownsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        var booking = await _bookingService.GetByIdAsync(Input.Id);
        if (booking == null)
        {
            return NotFound();
        }

        booking.StudentId = Input.StudentId;
        booking.RoomId = Input.RoomId;
        booking.StartTime = Input.StartTime;
        booking.EndTime = Input.EndTime;
        booking.Purpose = Input.Purpose;
        booking.ApprovedByInstructorId = Input.ApprovedByInstructorId;

        // If instructor is added and booking requires approval, mark as approved
        if (booking.RequiresApproval && Input.ApprovedByInstructorId.HasValue && !booking.IsApproved)
        {
            booking.IsApproved = true;
            booking.ApprovedAt = DateTime.UtcNow;
        }

        try
        {
            await _bookingService.UpdateAsync(booking);
            return RedirectToPage("./Index");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDropdownsAsync();
            return Page();
        }
    }

    private async Task LoadDropdownsAsync()
    {
        var students = await _bookingService.GetAllStudentsAsync();
        StudentSelectList = new SelectList(
            students.Select(s => new { s.Id, DisplayName = $"{s.FirstName} {s.LastName}" }),
            "Id",
            "DisplayName"
        );

        var rooms = await _bookingService.GetAllRoomsAsync();
        RoomSelectList = new SelectList(
            rooms.Select(r => new 
            { 
                r.Id, 
                DisplayName = $"{r.Name} ({r.Type}, Cap: {r.Capacity}) - {GetRoomEquipmentDisplay(r)}" 
            }),
            "Id",
            "DisplayName"
        );

        var instructors = await _bookingService.GetAllInstructorsAsync();
        InstructorSelectList = new SelectList(
            instructors.Select(i => new { i.Id, DisplayName = $"{i.FirstName} {i.LastName}" }),
            "Id",
            "DisplayName"
        );
    }

    private static string GetRoomEquipmentDisplay(Room room)
    {
        if (!room.RoomEquipments.Any())
            return "No equipment";

        var equipmentTypes = room.RoomEquipments
            .Select(re => re.Equipment.Type.ToString())
            .Distinct()
            .OrderBy(e => e);

        return string.Join(", ", equipmentTypes);
    }
}
