using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Data;
using ImageHosting.Models;
using ImageHosting.Services;
using ImageHosting.Interface;

namespace ImageHosting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ImagesService _imagesService;


        public ImagesController(ApplicationDbContext context, ImagesService imagesService)
        {
            _context = context;
            _imagesService = imagesService;

        }

        /// <summary>
        /// Returns a list of all images
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ImagesDto},{ImagesDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Project/List -> [{ProjectDto},{ProjectDto},..]
        /// </example>
        /// 
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<Images>>> GetImages()
        {
            return await _context.Images.ToListAsync();
        }

        /// <summary>
        /// Returns a single Image specified by its {id}
        /// </summary>
        /// <param name="id">The Image id</param>
        /// <returns>
        /// 200 OK
        /// {ImagesDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Image/Find/1 -> {ImagesDto}
        /// </example>
        // GET: api/Images/5
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<Images>> GetImages(int id)
        {
            var images = await _context.Images.FindAsync(id);

            if (images == null)
            {
                return NotFound();
            }

            return images;
        }

        /// <summary>
        /// Updates an Image
        /// </summary>
        /// <param name="id">The ID of the Image to update</param>
        /// <param name="ImagesDto">The required information to update the Project (ImageName ImageDescription)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Project/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {ProductDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>

        // PUT: api/Images/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> PutImages(int id, [FromBody] Images images)
        {
            if (id != images.ImageID)
            {
                return BadRequest();
            }

            _context.Entry(images).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImagesExists(id))
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

        // POST: api/Images
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Images>> PostImages(Images images)
        {
            _context.Images.Add(images);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetImages", new { id = images.ImageID }, images);
        }


        // DELETE: api/Images/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImages(int id)
        {
            var images = await _context.Images.FindAsync(id);
            if (images == null)
            {
                return NotFound();
            }

            _context.Images.Remove(images);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ImagesExists(int id)
        {
            return _context.Images.Any(e => e.ImageID == id);
        }
    }
}
