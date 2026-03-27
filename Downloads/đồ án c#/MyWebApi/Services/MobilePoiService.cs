using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.Models;

namespace MyWebApi.Services;

/// <summary>
/// Interface cho Mobile POI Service
/// </summary>
public interface IMobilePoiService
{
    Task<List<MobilePoiDto>> GetPoisForMobileAsync(MobilePoiQueryParams parameters);
    Task<MobilePoiDto?> GetPoiForMobileAsync(int id, string? languageCode = null);
}

/// <summary>
/// Service xử lý POI cho mobile app
/// </summary>
public class MobilePoiService : IMobilePoiService
{
    private readonly ApplicationDbContext _context;

    public MobilePoiService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách POI cho mobile app
    /// </summary>
    public async Task<List<MobilePoiDto>> GetPoisForMobileAsync(MobilePoiQueryParams parameters)
    {
        var query = _context.Pois
            .Include(p => p.Contents)
                .ThenInclude(c => c.Audio)
            .AsQueryable();

        // Lọc theo trạng thái hoạt động
        if (parameters.ActiveOnly == true)
        {
            query = query.Where(p => p.IsActive);
        }

        // Lọc theo độ ưu tiên
        if (parameters.MinPriority.HasValue)
        {
            query = query.Where(p => p.Priority >= parameters.MinPriority.Value);
        }

        // Giới hạn kết quả
        if (parameters.Limit.HasValue)
        {
            query = query.Take(parameters.Limit.Value);
        }

        var pois = await query
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.InternalName)
            .ToListAsync();

        // Map sang DTO
        var result = new List<MobilePoiDto>();
        foreach (var poi in pois)
        {
            var mobilePoi = await MapToMobileDtoAsync(poi, parameters.Lang);
            if (mobilePoi != null)
                result.Add(mobilePoi);
        }

        return result;
    }

    /// <summary>
    /// Lấy chi tiết POI cho mobile app
    /// </summary>
    public async Task<MobilePoiDto?> GetPoiForMobileAsync(int id, string? languageCode = null)
    {
        var poi = await _context.Pois
            .Include(p => p.Contents)
                .ThenInclude(c => c.Audio)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (poi == null)
            return null;

        return await MapToMobileDtoAsync(poi, languageCode);
    }

    /// <summary>
    /// Map POI entity sang Mobile DTO
    /// </summary>
    private async Task<MobilePoiDto?> MapToMobileDtoAsync(Poi poi, string? preferredLanguageCode = null)
    {
        if (poi.Contents == null || !poi.Contents.Any())
            return null;

        // Sắp xếp nội dung theo ngôn ngữ ưu tiên
        var sortedContents = poi.Contents
            .OrderByDescending(c => c.LanguageCode == preferredLanguageCode)
            .ThenBy(c => c.LanguageCode == "vi-VN") // Ưu tiên tiếng Việt
            .ThenBy(c => c.LanguageCode == "en-US") // Sau đó đến tiếng Anh
            .ToList();

        var mobileDto = new MobilePoiDto
        {
            Id = poi.Id,
            Name = sortedContents.FirstOrDefault()?.Title ?? poi.InternalName,
            Lat = poi.Latitude,
            Lng = poi.Longitude,
            Radius = poi.Radius,
            Priority = poi.Priority,
            Content = sortedContents.Select(c => new MobileContentDto
            {
                LanguageCode = c.LanguageCode,
                Title = c.Title,
                Text = c.ContentText,
                AudioUrl = c.Audio?.FileUrl
            }).ToList(),
            AudioUrl = sortedContents.FirstOrDefault()?.Audio?.FileUrl
        };

        return mobileDto;
    }
}
