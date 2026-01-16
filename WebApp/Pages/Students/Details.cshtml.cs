using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Students;

public class DetailsModel : PageModel
{
    private readonly StudentService _studentService;

    public DetailsModel(StudentService studentService)
    {
        _studentService = studentService;
    }

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
}
