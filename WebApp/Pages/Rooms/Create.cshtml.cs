using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages.Rooms;

public class CreateModel : PageModel
{
    private readonly RoomService _roomService;

    public CreateModel(RoomService roomService)
    {
        _roomService = roomService;
    }

    [BindProperty]
    public RoomInputModel Input { get; set; } = new();

    public List<Equipment> AvailableEquipment { get; set; } = new();

    public async Task OnGetAsync()
    {
        AvailableEquipment = await _roomService.GetAllEquipmentAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            AvailableEquipment = await _roomService.GetAllEquipmentAsync();
            return Page();
        }

        var room = new Room
        {
            Name = Input.Name,
            Type = Input.Type,
            Capacity = Input.Capacity,
            IsSoundproof = Input.IsSoundproof
        };

        try
        {
            await _roomService.CreateAsync(room, Input.SelectedEquipmentIds);
            return RedirectToPage("./Index");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            AvailableEquipment = await _roomService.GetAllEquipmentAsync();
            return Page();
        }
    }
}
