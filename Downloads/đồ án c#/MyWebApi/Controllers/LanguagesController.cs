using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using MyWebApi.Services;

namespace MyWebApi.Controllers;

/// <summary>
/// API Controller cho quản lý Ngôn ngữ (Languages)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LanguagesController : ControllerBase
{
    private readonly ILanguageService _languageService;
    private readonly ILogger<LanguagesController> _logger;

    public LanguagesController(ILanguageService languageService, ILogger<LanguagesController> logger)
    {
        _languageService = languageService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả ngôn ngữ
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Language>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Lấy danh sách ngôn ngữ");
            var languages = await _languageService.GetAllAsync();
            return Ok(languages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách ngôn ngữ");
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Lấy chi tiết một ngôn ngữ theo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Language>> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Lấy ngôn ngữ ID: {id}", id);
            var language = await _languageService.GetByIdAsync(id);
            
            if (language == null)
                return NotFound(new { message = $"Ngôn ngữ với ID {id} không tồn tại" });

            return Ok(language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy ngôn ngữ ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Lấy ngôn ngữ theo mã code (ví dụ: vi-VN)
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Language>> GetByCode(string code)
    {
        try
        {
            _logger.LogInformation("Lấy ngôn ngữ code: {code}", code);
            var language = await _languageService.GetByCodeAsync(code);
            
            if (language == null)
                return NotFound(new { message = $"Ngôn ngữ với code '{code}' không tồn tại" });

            return Ok(language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy ngôn ngữ code: {code}", code);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Tạo ngôn ngữ mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Language>> Create([FromBody] Language language)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Tạo ngôn ngữ mới: {name}", language.Name);
            var created = await _languageService.CreateAsync(language);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Lỗi xác thực khi tạo ngôn ngữ");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo ngôn ngữ");
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Cập nhật ngôn ngữ
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Language>> Update(int id, [FromBody] Language language)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Cập nhật ngôn ngữ ID: {id}", id);
            var updated = await _languageService.UpdateAsync(id, language);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Ngôn ngữ ID {id} không tồn tại", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Lỗi xác thực khi cập nhật ngôn ngữ");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật ngôn ngữ ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Xóa ngôn ngữ
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Xóa ngôn ngữ ID: {id}", id);
            var success = await _languageService.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = $"Ngôn ngữ với ID {id} không tồn tại" });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Không thể xóa ngôn ngữ ID: {id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa ngôn ngữ ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }
}
