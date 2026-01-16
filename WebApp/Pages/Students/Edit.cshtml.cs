using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Pages.Students;

public class EditModel : PageModel
{
    private readonly StudentService _studentService;

    public EditModel(StudentService studentService)
    {
        _studentService = studentService;
    }

    [BindProperty]
    public StudentEditModel Student { get; set; } = new();

    public SelectList InstructorSelectList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _studentService.GetByIdAsync(id.Value);
        if (student == null)
        {
            return NotFound();
        }

        Student = new StudentEditModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            StudentNumber = student.StudentNumber,
            Program = student.Program,
            PrimaryInstrument = student.PrimaryInstrument,
            InstructorId = student.InstructorId,
            NoShowCount = student.NoShowCount,
            QuotaPenaltyHours = student.QuotaPenaltyHours,
            TotalLoggedHours = student.TotalLoggedHours
        };

        await LoadInstructorsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadInstructorsAsync();
            return Page();
        }

        var student = await _studentService.GetByIdAsync(Student.Id);
        if (student == null)
        {
            return NotFound();
        }

        student.FirstName = Student.FirstName;
        student.LastName = Student.LastName;
        student.Email = Student.Email;
        student.StudentNumber = Student.StudentNumber;
        student.Program = Student.Program;
        student.PrimaryInstrument = Student.PrimaryInstrument;
        student.InstructorId = Student.InstructorId;
        student.NoShowCount = Student.NoShowCount;
        student.QuotaPenaltyHours = Student.QuotaPenaltyHours;
        student.TotalLoggedHours = Student.TotalLoggedHours;

        var success = await _studentService.UpdateAsync(student);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToPage("./Index");
    }

    private async Task LoadInstructorsAsync()
    {
        var instructors = await _studentService.GetAllInstructorsAsync();
        InstructorSelectList = new SelectList(
            instructors.Select(i => new { i.Id, DisplayName = $"{i.FirstName} {i.LastName}" }),
            "Id",
            "DisplayName"
        );
    }
}
