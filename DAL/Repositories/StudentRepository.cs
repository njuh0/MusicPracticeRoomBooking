using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students
            .Where(s => !s.IsDeleted)
            .Include(s => s.Instructor)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Where(s => !s.IsDeleted)
            .Include(s => s.Instructor)
            .Include(s => s.Bookings)
                .ThenInclude(b => b.Room)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> UpdateAsync(Student student)
    {
        // Use Update() to properly mark entity as Modified
        _context.Students.Update(student);
        
        try
        {
            await _context.SaveChangesAsync(); // Triggers SaveChangesAsync override and sets ModifiedAt
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ExistsAsync(student.Id))
            {
                return false;
            }
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null || student.IsDeleted)
        {
            return false;
        }

        // Soft delete - mark as deleted instead of physical removal
        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(); // Triggers ModifiedAt update
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<List<Instructor>> GetAllInstructorsAsync()
    {
        return await _context.Instructors
            .OrderBy(i => i.LastName)
            .ThenBy(i => i.FirstName)
            .ToListAsync();
    }
}
