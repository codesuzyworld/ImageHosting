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
        /// Returns a specific tag by its {id}
        /// </summary>
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
        /// Adds a new tag
        /// </summary>
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
        /// Updates a tag
        /// </summary>
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
        /// Deletes a tag by ID
        /// </summary>
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
