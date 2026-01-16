using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Rooms;

public class DeleteModel : PageModel
{
    private readonly RoomService _roomService;

    public DeleteModel(RoomService roomService)
    {
        _roomService = roomService;
    }

    [BindProperty]
    public Room Room { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await _roomService.GetByIdAsync(id.Value);
        if (room == null)
        {
            return NotFound();
        }

        Room = room;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var success = await _roomService.DeleteAsync(id.Value);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToPage("./Index");
    }
}
