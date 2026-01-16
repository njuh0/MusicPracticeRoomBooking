using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Pages.Students;

public class CreateModel : PageModel
{
    private readonly StudentService _studentService;

    public CreateModel(StudentService studentService)
    {
        _studentService = studentService;
    }

    [BindProperty]
    public StudentInputModel Student { get; set; } = new();

    public SelectList InstructorSelectList { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadInstructorsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadInstructorsAsync();
            return Page();
        }

        var student = new Student
        {
            FirstName = Student.FirstName,
            LastName = Student.LastName,
            Email = Student.Email,
            StudentNumber = Student.StudentNumber,
            Program = Student.Program,
            PrimaryInstrument = Student.PrimaryInstrument,
            InstructorId = Student.InstructorId
        };

        await _studentService.CreateAsync(student);
        return RedirectToPage("./Index");
    }

    private async Task LoadInstructorsAsync()
    {
        var instructors = await _studentService.GetAllInstructorsAsync();
        InstructorSelectList = new SelectList(instructors, "Id", "LastName");
    }
}
