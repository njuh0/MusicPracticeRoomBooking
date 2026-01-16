using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Bookings;

public class DetailsModel : PageModel
{
    private readonly BookingService _bookingService;

    public DetailsModel(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public Booking Booking { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _bookingService.GetByIdAsync(id.Value);
        if (booking == null)
        {
            return NotFound();
        }

        Booking = booking;
        return Page();
    }

    public async Task<IActionResult> OnPostCheckInAsync(int id)
    {
        await _bookingService.CheckInAsync(id);
        return RedirectToPage("./Details", new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        await _bookingService.CancelBookingAsync(id);
        return RedirectToPage("./Index");
    }
}
