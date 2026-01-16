using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly BookingService _bookingService;

    public IndexModel(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public List<Booking> Bookings { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public BookingStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public BookingPurpose? PurposeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public List<SelectListItem> StatusOptions { get; set; } = new();
    public List<SelectListItem> PurposeOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Populate filter dropdowns
        StatusOptions = new List<SelectListItem> { new SelectListItem { Value = "", Text = "All Statuses" } };
        StatusOptions.AddRange(Enum.GetValues<BookingStatus>()
            .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() }));

        PurposeOptions = new List<SelectListItem> { new SelectListItem { Value = "", Text = "All Purposes" } };
        PurposeOptions.AddRange(Enum.GetValues<BookingPurpose>()
            .Select(p => new SelectListItem { Value = p.ToString(), Text = p.ToString() }));

        var allBookings = await _bookingService.GetAllAsync();

        // Apply filters
        if (StatusFilter.HasValue)
        {
            allBookings = allBookings.Where(b => b.Status == StatusFilter.Value).ToList();
        }

        if (PurposeFilter.HasValue)
        {
            allBookings = allBookings.Where(b => b.Purpose == PurposeFilter.Value).ToList();
        }

        if (StartDate.HasValue)
        {
            allBookings = allBookings.Where(b => b.StartTime.Date >= StartDate.Value.Date).ToList();
        }

        if (EndDate.HasValue)
        {
            allBookings = allBookings.Where(b => b.EndTime.Date <= EndDate.Value.Date).ToList();
        }

        Bookings = allBookings;
    }
}
