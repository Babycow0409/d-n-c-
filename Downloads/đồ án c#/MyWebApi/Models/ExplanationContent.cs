namespace MyWebApi.Models;

/// <summary>
/// Nội dung thuyết minh được dịch theo từng ngôn ngữ
/// </summary>
public class ExplanationContent
{
    /// <summary>Mã định danh duy nhất</summary>
    public int Id { get; set; }

    /// <summary>ID của thuyết minh gốc</summary>
    public int ExplanationId { get; set; }

    /// <summary>ID của ngôn ngữ</summary>
    public int LanguageId { get; set; }

    /// <summary>Nội dung đã dịch</summary>
    public string TranslatedContent { get; set; } = string.Empty;

    /// <summary>Ghi chú dịch</summary>
    public string? Notes { get; set; }

    /// <summary>Người dịch</summary>
    public string? TranslatedBy { get; set; }

    /// <summary>Ngày dịch</summary>
    public DateTime TranslatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Trạng thái (Pending/Approved/Rejected)</summary>
    public string Status { get; set; } = "Pending";

    // Quan hệ
    public Explanation? Explanation { get; set; }
    public Language? Language { get; set; }
}
