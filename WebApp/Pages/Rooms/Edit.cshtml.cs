using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages.Rooms;

public class EditModel : PageModel
{
    private readonly RoomService _roomService;

    public EditModel(RoomService roomService)
    {
        _roomService = roomService;
    }

    [BindProperty]
    public RoomEditModel Input { get; set; } = new();

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

        Input = new RoomEditModel
        {
            Id = room.Id,
            Name = room.Name,
            Type = room.Type,
            Capacity = room.Capacity,
            IsSoundproof = room.IsSoundproof
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var room = await _roomService.GetByIdAsync(Input.Id);
        if (room == null)
        {
            return NotFound();
        }

        room.Name = Input.Name;
        room.Type = Input.Type;
        room.Capacity = Input.Capacity;
        room.IsSoundproof = Input.IsSoundproof;

        try
        {
            await _roomService.UpdateAsync(room);
            return RedirectToPage("./Index");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
