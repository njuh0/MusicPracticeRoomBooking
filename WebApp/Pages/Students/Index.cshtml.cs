using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Students;

public class IndexModel : PageModel
{
    private readonly StudentService _studentService;

    public IndexModel(StudentService studentService)
    {
        _studentService = studentService;
    }

    public List<Student> Students { get; set; } = new();

    public async Task OnGetAsync()
    {
        Students = await _studentService.GetAllAsync();
    }
}
