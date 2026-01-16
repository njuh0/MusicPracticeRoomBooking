using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Pages.Bookings;

public class CreateModel : PageModel
{
    private readonly BookingService _bookingService;

    public CreateModel(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [BindProperty]
    public BookingInputModel Input { get; set; } = new();

    public SelectList StudentSelectList { get; set; } = null!;
    public SelectList RoomSelectList { get; set; } = null!;
    public SelectList InstructorSelectList { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadDropdownsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        var booking = new Booking
        {
            StudentId = Input.StudentId,
            RoomId = Input.RoomId,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            Purpose = Input.Purpose,
            ApprovedByInstructorId = Input.ApprovedByInstructorId
        };

        try
        {
            await _bookingService.CreateAsync(booking);
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
        RoomSelectList = new SelectList(rooms, nameof(Room.Id), nameof(Room.Name));

        var instructors = await _bookingService.GetAllInstructorsAsync();
        InstructorSelectList = new SelectList(
            instructors.Select(i => new { i.Id, DisplayName = $"{i.FirstName} {i.LastName}" }),
            "Id",
            "DisplayName"
        );
    }
}
