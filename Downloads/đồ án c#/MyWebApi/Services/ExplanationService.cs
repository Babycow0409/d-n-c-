using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Services;

/// <summary>
/// Interface cho dịch vụ Explanation
/// </summary>
public interface IExplanationService
{
    Task<List<Explanation>> GetAllAsync();
    Task<Explanation?> GetByIdAsync(int id);
    Task<Explanation> CreateAsync(Explanation explanation);
    Task<Explanation> UpdateAsync(int id, Explanation explanation);
    Task<bool> DeleteAsync(int id);
    Task<List<ExplanationContent>> GetTranslationsAsync(int explanationId);
}

/// <summary>
/// Dịch vụ quản lý Explanation (Thuyết minh)
/// </summary>
public class ExplanationService : IExplanationService
{
    private readonly ApplicationDbContext _context;

    public ExplanationService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tất cả thuyết minh
    /// </summary>
    public async Task<List<Explanation>> GetAllAsync()
    {
        return await _context.Explanations
            .Where(e => e.IsActive)
            .Include(e => e.DefaultLanguage)
            .Include(e => e.Contents)
                .ThenInclude(c => c.Language)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy thuyết minh theo ID
    /// </summary>
    public async Task<Explanation?> GetByIdAsync(int id)
    {
        return await _context.Explanations
            .Where(e => e.IsActive)
            .Include(e => e.DefaultLanguage)
            .Include(e => e.Contents)
                .ThenInclude(c => c.Language)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Tạo thuyết minh mới
    /// </summary>
    public async Task<Explanation> CreateAsync(Explanation explanation)
    {
        // Kiểm tra ngôn ngữ mặc định có tồn tại không
        var languageExists = await _context.Languages
            .AsNoTracking()
            .AnyAsync(l => l.Id == explanation.DefaultLanguageId);

        if (!languageExists)
            throw new ArgumentException("Ngôn ngữ mặc định không tồn tại");

        _context.Explanations.Add(explanation);
        await _context.SaveChangesAsync();
        return explanation;
    }

    /// <summary>
    /// Cập nhật thuyết minh
    /// </summary>
    public async Task<Explanation> UpdateAsync(int id, Explanation explanation)
    {
        var existing = await _context.Explanations.FindAsync(id)
            ?? throw new KeyNotFoundException($"Thuyết minh với ID {id} không tồn tại");

        existing.Title = explanation.Title;
        existing.Description = explanation.Description;
        existing.Content = explanation.Content;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.IsActive = explanation.IsActive;

        _context.Explanations.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Xóa thuyết minh (soft delete)
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var explanation = await _context.Explanations.FindAsync(id);
        if (explanation == null)
            return false;

        explanation.IsActive = false;
        explanation.UpdatedAt = DateTime.UtcNow;

        _context.Explanations.Update(explanation);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Lấy tất cả bản dịch của một thuyết minh
    /// </summary>
    public async Task<List<ExplanationContent>> GetTranslationsAsync(int explanationId)
    {
        return await _context.ExplanationContents
            .Where(c => c.ExplanationId == explanationId)
            .Include(c => c.Language)
            .OrderBy(c => c.Language!.Name)
            .ToListAsync();
    }
}
