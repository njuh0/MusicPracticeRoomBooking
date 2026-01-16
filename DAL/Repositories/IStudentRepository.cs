using DAL.Entities;

namespace DAL.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student> CreateAsync(Student student);
    Task<bool> UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByEmailAsync(string email, int? excludeStudentId = null);
    Task<bool> IncrementNoShowCountAsync(int studentId);
    Task<List<Instructor>> GetAllInstructorsAsync();
}
