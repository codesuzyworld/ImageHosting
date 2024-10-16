using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Data;
using ImageHosting.Models;
using ImageHosting.Interface;

namespace ImageHosting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploaderController : ControllerBase
    {
        private readonly IUploaderService _uploaderService;

        public UploaderController(IUploaderService uploaderService)
        {
            _uploaderService = uploaderService;
        }

        /// <summary>
        /// Returns a list of all uploaders
        /// </summary>
        /// <returns>200 OK with a list of UploaderDto</returns>
        /// <example>
        /// GET: api/Uploaders/List -> [{UploaderDto},{UploaderDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<UploaderDto>>> ListUploaders()
        {
            var uploaders = await _uploaderService.ListUploaders();
            return Ok(uploaders);
        }

        /// <summary>
        /// Returns a single uploader specified by its {id}
        /// </summary>
        /// <param name="id">The uploader id</param>
        /// <returns>200 OK with UploaderDto or 404 Not Found</returns>
        /// <example>
        /// GET: api/Uploaders/Find/1 -> {UploaderDto}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<UploaderDto>> FindUploader(int id)
        {
            var uploader = await _uploaderService.FindUploader(id);
            if (uploader == null)
            {
                return NotFound();
            }

            return Ok(uploader);
        }

        /// <summary>
        /// Adds a new uploader
        /// </summary>
        /// <param name="uploaderDto">Adds Uploader name and email</param>
        /// <returns>201 Created with the added uploader</returns>
        /// <example>
        /// POST: api/Uploaders/Add
        /// Request Body: {UploaderDto}
        /// -> 201 Created
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<UploaderDto>> AddUploader([FromBody] UploaderDto uploaderDto)
        {
            var response = await _uploaderService.AddUploader(uploaderDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Created($"api/Uploaders/Find/{response.CreatedId}", uploaderDto);
        }

        /// <summary>
        /// Updates an existing uploader
        /// </summary>
        /// <param name="id">The ID of the uploader to update</param>
        /// <param name="uploaderDto">Updates Uploader name and email</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        /// <example>
        /// PUT: api/Uploaders/Update/1
        /// Request Body: {UploaderDto}
        /// -> 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> UpdateUploader(int id, [FromBody] UploaderDto uploaderDto)
        {
            if (id != uploaderDto.UploaderID)
            {
                return BadRequest("ID mismatch.");
            }

            var response = await _uploaderService.UpdateUploader(uploaderDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return NoContent();
        }

        /// <summary>
        /// Deletes an uploader by id
        /// </summary>
        /// <param name="id">UploaderId</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        /// <example>
        /// DELETE: api/Uploaders/Delete/1
        /// -> 204 No Content
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteUploader(int id)
        {
            var response = await _uploaderService.DeleteUploader(id);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

    }

}
