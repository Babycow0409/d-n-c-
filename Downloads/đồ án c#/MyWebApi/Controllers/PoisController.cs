using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Models;
using MyWebApi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class PoisController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 

        public PoisController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poi>>> GetPois()
        {
            var pois = await _context.Pois.ToListAsync();
            return Ok(pois); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Poi>> GetPoi(int id)
        {
            var poi = await _context.Pois
                .Include(p => p.Contents) 
                    .ThenInclude(c => c.Audio)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (poi == null)
            {
                return NotFound(new { message = $"Không tìm thấy POI nào với ID = {id}" });
            }

            return Ok(poi);
        }

        [HttpPost]
        public async Task<ActionResult<Poi>> PostPoi(Poi poi)
        {
            _context.Pois.Add(poi);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPoi), new { id = poi.Id }, poi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoi(int id, Poi poi)
        {
            if (id != poi.Id)
            {
                return BadRequest(new { message = "ID được truyền trên URL không khớp với dữ liệu JSON" });
            }

            _context.Entry(poi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PoiExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy POI để cập nhật, có thể đã bị ai đó xóa." });
                }
                else
                {
                    throw; 
                }
            }

            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoi(int id)
        {
            var poi = await _context.Pois.FindAsync(id);
            if (poi == null)
            {
                return NotFound(new { message = "Không tìm thấy POI để xóa." });
            }

            _context.Pois.Remove(poi);
            await _context.SaveChangesAsync(); 

            return Ok(new { message = "Xóa POI thành công!", deletedId = id });
        }

        private bool PoiExists(int id)
        {
            return _context.Pois.Any(e => e.Id == id);
        }
    }
}
