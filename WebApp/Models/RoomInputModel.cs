using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace WebApp.Models;

public class RoomInputModel
{
    [Required(ErrorMessage = "Room name is required")]
    [StringLength(50, ErrorMessage = "Room name cannot exceed 50 characters")]
    [Display(Name = "Room Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room type is required")]
    [Display(Name = "Room Type")]
    public RoomType Type { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50")]
    [Display(Name = "Capacity (people)")]
    public int Capacity { get; set; }

    [Display(Name = "Soundproof")]
    public bool IsSoundproof { get; set; }

    [Display(Name = "Equipment")]
    public List<int> SelectedEquipmentIds { get; set; } = new();
}
