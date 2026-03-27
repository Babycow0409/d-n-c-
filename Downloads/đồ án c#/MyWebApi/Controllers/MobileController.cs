using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.Services;

namespace MyWebApi.Controllers;

/// <summary>
/// API Controller chuyên biệt cho Mobile App
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MobileController : ControllerBase
{
    private readonly IMobilePoiService _mobilePoiService;
    private readonly ILogger<MobileController> _logger;

    public MobileController(IMobilePoiService mobilePoiService, ILogger<MobileController> logger)
    {
        _mobilePoiService = mobilePoiService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách POI cho mobile app
    /// </summary>
    /// <param name="lang">Mã ngôn ngữ (ví dụ: vi-VN, en-US)</param>
    /// <param name="minPriority">Độ ưu tiên tối thiểu</param>
    /// <param name="activeOnly">Chỉ lấy POI đang hoạt động (default: true)</param>
    /// <param name="limit">Giới hạn số lượng kết quả (default: 100)</param>
    /// <returns>Danh sách POI tối ưu cho mobile</returns>
    [HttpGet("pois")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<MobilePoiDto>>> GetPois(
        [FromQuery] string? lang = "vi-VN",
        [FromQuery] int? minPriority = null,
        [FromQuery] bool? activeOnly = true,
        [FromQuery] int? limit = 100)
    {
        try
        {
            _logger.LogInformation("Mobile API - Lấy danh sách POI với lang={lang}", lang);

            var parameters = new MobilePoiQueryParams
            {
                Lang = lang,
                MinPriority = minPriority,
                ActiveOnly = activeOnly,
                Limit = limit
            };

            var pois = await _mobilePoiService.GetPoisForMobileAsync(parameters);

            // Thêm headers cho mobile optimization
            Response.Headers.Add("X-Total-Count", pois.Count.ToString());
            Response.Headers.Add("X-Language", lang ?? "vi-VN");
            Response.Headers.Add("Cache-Control", "public, max-age=300"); // Cache 5 phút

            return Ok(pois);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mobile API - Lỗi khi lấy danh sách POI");
            return StatusCode(500, new { 
                error = "Internal server error",
                message = "Đã xảy ra lỗi khi lấy dữ liệu POI",
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Lấy chi tiết POI theo ID cho mobile app
    /// </summary>
    /// <param name="id">ID của POI</param>
    /// <param name="lang">Mã ngôn ngữ (ví dụ: vi-VN, en-US)</param>
    /// <returns>Chi tiết POI</returns>
    [HttpGet("pois/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MobilePoiDto>> GetPoi(int id, [FromQuery] string? lang = "vi-VN")
    {
        try
        {
            _logger.LogInformation("Mobile API - Lấy POI ID={id} với lang={lang}", id, lang);

            var poi = await _mobilePoiService.GetPoiForMobileAsync(id, lang);

            if (poi == null)
            {
                return NotFound(new { 
                    error = "Not found",
                    message = $"Không tìm thấy POI với ID {id}",
                    poiId = id,
                    timestamp = DateTime.UtcNow
                });
            }

            // Thêm headers cho mobile optimization
            Response.Headers.Add("X-Language", lang ?? "vi-VN");
            Response.Headers.Add("Cache-Control", "public, max-age=600"); // Cache 10 phút

            return Ok(poi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mobile API - Lỗi khi lấy POI ID={id}", id);
            return StatusCode(500, new { 
                error = "Internal server error",
                message = "Đã xảy ra lỗi khi lấy dữ liệu POI",
                poiId = id,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Health check cho mobile API
    /// </summary>
    /// <returns>Trạng thái mobile API</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> GetHealth()
    {
        return Ok(new
        {
            status = "Mobile API is running ✅",
            version = "1.0.0",
            timestamp = DateTime.UtcNow,
            endpoints = new
            {
                pois = "/api/mobile/pois",
                poiDetail = "/api/mobile/pois/{id}",
                health = "/api/mobile/health"
            }
        });
    }
}
