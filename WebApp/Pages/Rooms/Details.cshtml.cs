using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Rooms;

public class DetailsModel : PageModel
{
    private readonly RoomService _roomService;

    public DetailsModel(RoomService roomService)
    {
        _roomService = roomService;
    }

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
}
