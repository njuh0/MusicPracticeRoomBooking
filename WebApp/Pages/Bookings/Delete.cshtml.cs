using DAL.Entities;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Bookings
{
    public class DeleteModel : PageModel
    {
        private readonly BookingService _bookingService;

        public DeleteModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _bookingService.DeleteAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
