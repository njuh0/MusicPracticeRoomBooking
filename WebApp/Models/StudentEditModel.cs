using DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class StudentEditModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Student Number")]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    public StudentProgram Program { get; set; }

    [Required]
    [Display(Name = "Primary Instrument")]
    public InstrumentType PrimaryInstrument { get; set; }

    [Display(Name = "Instructor")]
    public int? InstructorId { get; set; }

    [Display(Name = "No-Show Count")]
    public int NoShowCount { get; set; }

    [Display(Name = "Quota Penalty Hours")]
    public int QuotaPenaltyHours { get; set; }

    [Display(Name = "Total Logged Hours")]
    public int TotalLoggedHours { get; set; }
}
