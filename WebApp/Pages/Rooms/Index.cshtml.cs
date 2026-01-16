using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Rooms;

public class IndexModel : PageModel
{
    private readonly RoomService _roomService;

    public IndexModel(RoomService roomService)
    {
        _roomService = roomService;
    }

    public List<Room> Rooms { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public RoomType? RoomTypeFilter { get; set; }

    public List<SelectListItem> RoomTypeOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Populate room type filter dropdown
        RoomTypeOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "All Types" }
        };
        RoomTypeOptions.AddRange(Enum.GetValues<RoomType>()
            .Select(rt => new SelectListItem { Value = rt.ToString(), Text = rt.ToString() }));

        var allRooms = await _roomService.GetAllAsync();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var searchLower = SearchTerm.ToLower();
            allRooms = allRooms.Where(r => r.Name.ToLower().Contains(searchLower)).ToList();
        }

        if (RoomTypeFilter.HasValue)
        {
            allRooms = allRooms.Where(r => r.Type == RoomTypeFilter.Value).ToList();
        }

        Rooms = allRooms;
    }
}
