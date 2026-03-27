using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Controllers;

/// <summary>
/// Controller tạm để tạo dữ liệu test
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tạo dữ liệu mẫu cho POI
    /// </summary>
    [HttpPost("create-sample-poi")]
    public async Task<IActionResult> CreateSamplePoi()
    {
        // Tạo POI
        var poi = new Poi
        {
            InternalName = "Ho Chi Minh Mausoleum",
            Latitude = 21.5885m,
            Longitude = 105.8344m,
            Radius = 200,
            Priority = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Pois.Add(poi);
        await _context.SaveChangesAsync();

        // Tạo nội dung tiếng Việt
        var contentVi = new PoiContent
        {
            PoiId = poi.Id,
            LanguageCode = "vi-VN",
            Title = "Lăng Bác",
            ContentText = "Lăng Chủ tịch Hồ Chí Minh là nơi an nghỉ của Chủ tịch Hồ Chí Minh, vị lãnh tụ vĩ đại của dân tộc Việt Nam."
        };

        // Tạo nội dung tiếng Anh
        var contentEn = new PoiContent
        {
            PoiId = poi.Id,
            LanguageCode = "en-US",
            Title = "Ho Chi Minh Mausoleum",
            ContentText = "Ho Chi Minh Mausoleum is the resting place of Ho Chi Minh, the great leader of the Vietnamese nation."
        };

        _context.PoiContents.Add(contentVi);
        _context.PoiContents.Add(contentEn);
        await _context.SaveChangesAsync();

        return Ok(new { 
            message = "Đã tạo dữ liệu mẫu thành công",
            poiId = poi.Id,
            contents = new[] { contentVi, contentEn }
        });
    }
}
