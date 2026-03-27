namespace MyWebApi.Models;

/// <summary>
/// Thuyết minh (nội dung chính cần được dịch)
/// </summary>
public class Explanation
{
    /// <summary>Mã định danh duy nhất</summary>
    public int Id { get; set; }

    /// <summary>Tiêu đề thuyết minh</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Mô tả ngắn</summary>
    public string? Description { get; set; }

    /// <summary>Nội dung chính (mặc định là ngôn ngữ gốc)</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Ngôn ngữ gốc (ID của Language)</summary>
    public int DefaultLanguageId { get; set; }

    /// <summary>Người tạo</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Ngày tạo</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Ngày cập nhật cuối cùng</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Trạng thái (Active/Inactive)</summary>
    public bool IsActive { get; set; } = true;

    // Quan hệ
    public Language? DefaultLanguage { get; set; }
    public ICollection<ExplanationContent> Contents { get; set; } = [];
}
