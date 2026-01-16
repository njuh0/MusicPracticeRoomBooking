using System.ComponentModel.DataAnnotations;
using DAL.Entities;
using WebApp.Validation;

namespace WebApp.Models;

public class BookingInputModel
{
    [Required(ErrorMessage = "Student is required")]
    [Display(Name = "Student")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Room is required")]
    [Display(Name = "Room")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    [Display(Name = "Start Time")]
    [FutureDate(ErrorMessage = "Booking must be in the future")]
    public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);

    [Required(ErrorMessage = "End time is required")]
    [Display(Name = "End Time")]
    [GreaterThan(nameof(StartTime), ErrorMessage = "End time must be after start time")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(2);

    [Required(ErrorMessage = "Purpose is required")]
    [Display(Name = "Purpose")]
    public BookingPurpose Purpose { get; set; }

    [Display(Name = "Instructor (for Recital Prep)")]
    public int? ApprovedByInstructorId { get; set; }
}
