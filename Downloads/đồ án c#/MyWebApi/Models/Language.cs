namespace MyWebApi.Models;

/// <summary>
/// Mô tả ngôn ngữ (Tiếng Việt, Tiếng Anh, v.v.)
/// </summary>
public class Language
{
    /// <summary>Mã định danh duy nhất</summary>
    public int Id { get; set; }

    /// <summary>Tên ngôn ngữ (ví dụ: Tiếng Việt, English)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mã ISO ngôn ngữ (ví dụ: vi-VN, en-US)</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Mô tả</summary>
    public string? Description { get; set; }

    /// <summary>Ngày tạo bản ghi</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Quan hệ một-nhiều
    public ICollection<ExplanationContent> ExplanationContents { get; set; } = [];
}
