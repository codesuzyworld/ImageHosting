using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Data;
using ImageHosting.Models;

namespace ImageHosting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UploadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Uploaders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Uploader>>> GetUploader()
        {
            return await _context.Uploader.ToListAsync();
        }

        // GET: api/Uploaders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Uploader>> GetUploader(int id)
        {
            var uploader = await _context.Uploader.FindAsync(id);

            if (uploader == null)
            {
                return NotFound();
            }

            return uploader;
        }

        // PUT: api/Uploaders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUploader(int id, Uploader uploader)
        {
            if (id != uploader.UploaderID)
            {
                return BadRequest();
            }

            _context.Entry(uploader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UploaderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Uploaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // I have modified this template so that it uses the Uploader DTO 
        [HttpPost]
        public async Task<ActionResult<Uploader>> PostUploader(UploaderDto uploaderDto)
        {
            var uploader = new Uploader
            {
                UploaderName = uploaderDto.UploaderName,
                UploaderEmail = uploaderDto.UploaderEmail
            };

            _context.Uploader.Add(uploader);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUploader", new { id = uploader.UploaderID }, uploader);
        }

        // DELETE: api/Uploaders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUploader(int id)
        {
            var uploader = await _context.Uploader.FindAsync(id);
            if (uploader == null)
            {
                return NotFound();
            }

            _context.Uploader.Remove(uploader);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UploaderExists(int id)
        {
            return _context.Uploader.Any(e => e.UploaderID == id);
        }
    }
}
