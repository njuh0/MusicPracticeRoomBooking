using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Students;

public class DeleteModel : PageModel
{
    private readonly StudentService _studentService;

    public DeleteModel(StudentService studentService)
    {
        _studentService = studentService;
    }

    [BindProperty]
    public Student Student { get; set; } = null!;

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

        Student = student;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var success = await _studentService.DeleteAsync(id.Value);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToPage("./Index");
    }
}
