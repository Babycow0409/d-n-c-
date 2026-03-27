namespace MyWebApi.DTOs;

/// <summary>
/// DTO cho POI trên mobile app - tối ưu và gọn nhẹ
/// </summary>
public class MobilePoiDto
{
    /// <summary>ID của POI</summary>
    public int Id { get; set; }

    /// <summary>Tên POI (sẽ lấy theo ngôn ngữ)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Vĩ độ</summary>
    public decimal Lat { get; set; }

    /// <summary>Kinh độ</summary>
    public decimal Lng { get; set; }

    /// <summary>Bán kính hoạt động (mét)</summary>
    public int Radius { get; set; }

    /// <summary>Độ ưu tiên</summary>
    public int Priority { get; set; }

    /// <summary>Nội dung đa ngôn ngữ</summary>
    public List<MobileContentDto> Content { get; set; } = new();

    /// <summary>URL audio (nếu có)</summary>
    public string? AudioUrl { get; set; }
}

/// <summary>
/// DTO cho nội dung POI trên mobile
/// </summary>
public class MobileContentDto
{
    /// <summary>Mã ngôn ngữ (vi-VN, en-US)</summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>Tên POI theo ngôn ngữ</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Nội dung mô tả</summary>
    public string? Text { get; set; }

    /// <summary>URL audio theo ngôn ngữ</summary>
    public string? AudioUrl { get; set; }
}

/// <summary>
/// Parameters cho mobile POI API
/// </summary>
public class MobilePoiQueryParams
{
    /// <summary>Mã ngôn ngữ ưu tiên (ví dụ: vi-VN)</summary>
    public string? Lang { get; set; } = "vi-VN";

    /// <summary>Lọc theo độ ưu tiên tối thiểu</summary>
    public int? MinPriority { get; set; }

    /// <summary>Chỉ lấy POI đang hoạt động</summary>
    public bool? ActiveOnly { get; set; } = true;

    /// <summary>Giới hạn số lượng kết quả</summary>
    public int? Limit { get; set; } = 100;
}
