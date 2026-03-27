using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using MyWebApi.Services;

namespace MyWebApi.Controllers;

/// <summary>
/// API Controller cho quản lý Thuyết minh (Explanations)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExplanationsController : ControllerBase
{
    private readonly IExplanationService _explanationService;
    private readonly ILogger<ExplanationsController> _logger;

    public ExplanationsController(IExplanationService explanationService, ILogger<ExplanationsController> logger)
    {
        _explanationService = explanationService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả thuyết minh
    /// </summary>
    /// <returns>Danh sách các thuyết minh</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Explanation>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Lấy danh sách thuyết minh");
            var explanations = await _explanationService.GetAllAsync();
            return Ok(explanations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách thuyết minh");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy dữ liệu" });
        }
    }

    /// <summary>
    /// Lấy chi tiết một thuyết minh theo ID
    /// </summary>
    /// <param name="id">ID của thuyết minh</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Explanation>> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Lấy thuyết minh ID: {id}", id);
            var explanation = await _explanationService.GetByIdAsync(id);
            
            if (explanation == null)
                return NotFound(new { message = $"Thuyết minh với ID {id} không tồn tại" });

            return Ok(explanation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thuyết minh ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Tạo thuyết minh mới
    /// </summary>
    /// <param name="explanation">Thông tin thuyết minh cần tạo</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Explanation>> Create([FromBody] Explanation explanation)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Tạo thuyết minh mới: {title}", explanation.Title);
            var created = await _explanationService.CreateAsync(explanation);
            
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Lỗi xác thực khi tạo thuyết minh");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo thuyết minh");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo dữ liệu" });
        }
    }

    /// <summary>
    /// Cập nhật thuyết minh
    /// </summary>
    /// <param name="id">ID của thuyết minh cần cập nhật</param>
    /// <param name="explanation">Thông tin cập nhật</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Explanation>> Update(int id, [FromBody] Explanation explanation)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Cập nhật thuyết minh ID: {id}", id);
            var updated = await _explanationService.UpdateAsync(id, explanation);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Thuyết minh ID {id} không tồn tại", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật thuyết minh ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Xóa thuyết minh
    /// </summary>
    /// <param name="id">ID của thuyết minh cần xóa</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Xóa thuyết minh ID: {id}", id);
            var success = await _explanationService.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = $"Thuyết minh với ID {id} không tồn tại" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa thuyết minh ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    /// <summary>
    /// Lấy tất cả bản dịch của một thuyết minh
    /// </summary>
    /// <param name="id">ID của thuyết minh</param>
    [HttpGet("{id}/translations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ExplanationContent>>> GetTranslations(int id)
    {
        try
        {
            _logger.LogInformation("Lấy bản dịch cho thuyết minh ID: {id}", id);
            
            // Kiểm tra thuyết minh có tồn tại không
            var explanation = await _explanationService.GetByIdAsync(id);
            if (explanation == null)
                return NotFound(new { message = $"Thuyết minh với ID {id} không tồn tại" });

            var translations = await _explanationService.GetTranslationsAsync(id);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy bản dịch cho thuyết minh ID: {id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }
}
