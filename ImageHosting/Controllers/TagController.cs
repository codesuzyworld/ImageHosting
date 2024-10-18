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
using ImageHosting.Services;

namespace ImageHosting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagsService;

        // dependency injection of service interfaces
        public TagController(ITagService TagService)
        {
            _tagsService = TagService;
        }

        /// <summary>
        /// Returns a list of all Tags
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{TagDto},{TagDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Tag/List -> [{TagDto},{TagDto},..]
        /// </example>
        /// 
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<TagDto>>> ListTags()
        {
            // empty list of data transfer object TagDto
            IEnumerable<TagDto> TagDtos = await _tagsService.ListTags();
            // return 200 OK with TagDtos
            return Ok(TagDtos);
        }

        /// <summary>
        /// Returns a specific tag by its {id}.
        /// </summary>
        /// <param name="id">The ID of the tag to retrieve</param>
        /// <returns>
        /// 200 OK
        /// {TagDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Tag/Find/1 -> {TagDto}
        /// </example>

        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<TagDto>> FindTag(int id)
        {

            var Tag = await _tagsService.FindTag(id);

            // if the category could not be located, return 404 Not Found
            if (Tag == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Tag);
            }
        }


        /// <summary>
        /// Adds a new tag to the database.
        /// </summary>
        /// <param name="tagDto">The DTO containing the necessary information to create the tag (e.g., TagName, TagColor)</param>
        /// <returns>
        /// 201 Created
        /// {TagDto}
        /// or
        /// 404 Not Found
        /// or
        /// 500 Internal Server Error
        /// </returns>
        /// <example>
        /// POST: api/Tag/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {TagDto}
        /// ->
        /// Response Code: 201 Created
        /// </example>

        [HttpPost(template: "Add")]
        public async Task<ActionResult<TagDto>> AddTag(TagDto tagDto)
        {
            ServiceResponse response = await _tagsService.AddTag(tagDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Tag/FindTag/{response.CreatedId}", tagDto);
        }

        /// <summary>
        /// Updates an existing tag specified by its {id}.
        /// </summary>
        /// <param name="id">The ID of the tag to update</param>
        /// <param name="tagDto">The DTO containing updated tag information</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 500 Internal Server Error
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Tag/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {TagDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        /// 
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] TagDto tagDto)
        {
            // {id} in URL must match TagId in POST Body
            if (id != tagDto.TagID)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _tagsService.UpdateTag(tagDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            //Status = Updated
            return NoContent();
        }
        /// <summary>
        /// Deletes the tag specified by its {id}.
        /// </summary>
        /// <param name="id">The ID of the tag to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// or
        /// 500 Internal Server Error
        /// </returns>
        /// <example>
        /// DELETE: api/Tag/Delete/3
        /// -> 
        /// Response Code: 204 No Content
        /// </example>
       

        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            ServiceResponse response = await _tagsService.DeleteTag(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Returns a list of all projects associated with a specific tag identified by its {id}.
        /// </summary>
        /// <param name="id">The ID of the tag for which to list associated projects</param>
        /// <returns>
        /// 200 OK
        /// [{ProjectDto},{ProjectDto},..]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Tag/ListProjectsForTag/4 -> [{ProjectDto},{ProjectDto},..]
        /// </example>
        /// 
        [HttpGet(template: "ListProjectsForTag/{id}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> ListProjectsForTag(int id)
        {
            var projects = await _tagsService.ListProjectsForTag(id);

            if (projects == null || !projects.Any())
            {
                return NotFound($"No projects found.");
            }

            return Ok(projects);
        }
    }
}
