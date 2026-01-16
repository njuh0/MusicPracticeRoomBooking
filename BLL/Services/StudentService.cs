using DAL.Entities;
using DAL.Repositories;

namespace BLL.Services;

public class StudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _studentRepository.GetAllAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        // Business logic validation can be added here
        // Example: check email uniqueness, StudentNumber validation, etc.
        
        return await _studentRepository.CreateAsync(student);
    }

    public async Task<bool> UpdateAsync(Student student)
    {
        // Business logic can be added here
        // Example: recalculate quotas, validate program changes, etc.
        
        return await _studentRepository.UpdateAsync(student);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Business logic can be added here
        // Example: check for active bookings before deletion
        
        return await _studentRepository.DeleteAsync(id);
    }

    public async Task<bool> StudentExistsAsync(int id)
    {
        return await _studentRepository.ExistsAsync(id);
    }

    public async Task<List<Instructor>> GetAllInstructorsAsync()
    {
        return await _studentRepository.GetAllInstructorsAsync();
    }

    // Example of business logic that will be implemented here:
    // public async Task<bool> CanBookRoomAsync(int studentId, DateTime startTime, int durationHours)
    // {
    //     var student = await GetByIdAsync(studentId);
    //     if (student == null) return false;
    //     
    //     // Check quota availability
    //     var weeklyUsage = await CalculateWeeklyUsageAsync(studentId);
    //     return (weeklyUsage + durationHours) <= student.EffectiveWeeklyQuota;
    // }
}
