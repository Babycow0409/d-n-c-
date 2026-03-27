using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Services;

/// <summary>
/// Interface cho dịch vụ Language
/// </summary>
public interface ILanguageService
{
    Task<List<Language>> GetAllAsync();
    Task<Language?> GetByIdAsync(int id);
    Task<Language?> GetByCodeAsync(string code);
    Task<Language> CreateAsync(Language language);
    Task<Language> UpdateAsync(int id, Language language);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Dịch vụ quản lý Language (Ngôn ngữ)
/// </summary>
public class LanguageService : ILanguageService
{
    private readonly ApplicationDbContext _context;

    public LanguageService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy tất cả ngôn ngữ
    /// </summary>
    public async Task<List<Language>> GetAllAsync()
    {
        return await _context.Languages
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy ngôn ngữ theo ID
    /// </summary>
    public async Task<Language?> GetByIdAsync(int id)
    {
        return await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    /// <summary>
    /// Lấy ngôn ngữ theo mã code (ví dụ: vi-VN)
    /// </summary>
    public async Task<Language?> GetByCodeAsync(string code)
    {
        return await _context.Languages
            .FirstOrDefaultAsync(l => l.Code == code);
    }

    /// <summary>
    /// Tạo ngôn ngữ mới
    /// </summary>
    public async Task<Language> CreateAsync(Language language)
    {
        // Kiểm tra code có bị trùng không
        var exists = await _context.Languages
            .AsNoTracking()
            .AnyAsync(l => l.Code == language.Code);

        if (exists)
            throw new InvalidOperationException($"Mã ngôn ngữ '{language.Code}' đã tồn tại");

        _context.Languages.Add(language);
        await _context.SaveChangesAsync();
        return language;
    }

    /// <summary>
    /// Cập nhật ngôn ngữ
    /// </summary>
    public async Task<Language> UpdateAsync(int id, Language language)
    {
        var existing = await _context.Languages.FindAsync(id)
            ?? throw new KeyNotFoundException($"Ngôn ngữ với ID {id} không tồn tại");

        // Kiểm tra code không trùng với ngôn ngữ khác
        var codeExists = await _context.Languages
            .AsNoTracking()
            .AnyAsync(l => l.Code == language.Code && l.Id != id);

        if (codeExists)
            throw new InvalidOperationException($"Mã ngôn ngữ '{language.Code}' đã được sử dụng");

        existing.Name = language.Name;
        existing.Code = language.Code;
        existing.Description = language.Description;

        _context.Languages.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Xóa ngôn ngữ
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var language = await _context.Languages.FindAsync(id);
        if (language == null)
            return false;

        // Kiểm tra nếu ngôn ngữ đang được sử dụng
        var isInUse = await _context.ExplanationContents
            .AsNoTracking()
            .AnyAsync(c => c.LanguageId == id);

        if (isInUse)
            throw new InvalidOperationException("Không thể xóa ngôn ngữ đang được sử dụng");

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync();
        return true;
    }
}
