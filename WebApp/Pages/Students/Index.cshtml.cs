using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
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

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        var allStudents = await _studentService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var searchLower = SearchTerm.ToLower();
            Students = allStudents
                .Where(s => s.FirstName.ToLower().Contains(searchLower) ||
                           s.LastName.ToLower().Contains(searchLower) ||
                           s.Email.ToLower().Contains(searchLower) ||
                           s.StudentNumber.ToLower().Contains(searchLower))
                .ToList();
        }
        else
        {
            Students = allStudents;
        }
    }
}
