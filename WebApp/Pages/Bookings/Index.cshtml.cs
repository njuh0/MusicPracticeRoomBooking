using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly BookingService _bookingService;

    public IndexModel(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public List<Booking> Bookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        Bookings = await _bookingService.GetAllAsync();
    }
}
