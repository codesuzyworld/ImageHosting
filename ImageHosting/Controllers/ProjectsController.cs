using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageHosting;
using ImageHosting.Data;
using ImageHosting.Models;
using ImageHosting.Interface;

namespace ImageHosting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }


        /// <summary>
        /// Returns a list of Project
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ProjectDto},{ProjectDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Project/List -> [{ProjectDto},{ProjectDto},..]
        /// </example>
        /// 
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> ListProjects()
        {
            // empty list of data transfer object ProjectDto
            IEnumerable<ProjectDto> ProjectDtos = await _projectService.ListProjects();
            
            // return 200 OK with ProjectDtos
            return Ok(ProjectDtos);
        }

        /// <summary>
        /// Returns a single Project specified by its {id}
        /// </summary>
        /// <param name="id">The Project id</param>
        /// <returns>
        /// 200 OK
        /// {ProjecttDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Project/Find/1 -> {ProjectDto}
        /// </example>
        
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ProjectDto>> FindProject(int id)
        {
            var Project = await _projectService.FindProject(id);

            // if the Project could not be located, return 404 Not Found
            if (Project == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Project);
            }

        }

        [HttpPost(template: "Add")]
        public async Task<ActionResult<ProjectDto>> AddProject([FromBody] ProjectDto projectDto)
        {
            var response = await _projectService.AddProject(projectDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return Created($"api/Project/Find/{response.CreatedId}", projectDto);
        }

        /// <summary>
        /// Updates a Project
        /// </summary>
        /// <param name="id">The ID of the Project to update</param>
        /// <param name="ProductDto">The required information to update the Project (ProjectName ProjectDescription)</param>
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

        [HttpPut(template: "Update/{id}")]
        public async Task<ActionResult> UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            if (id != projectDto.ProjectId)
            {
                return BadRequest();
            }

            ServiceResponse response = await _projectService.UpdateProject(projectDto);

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
        /// Deletes the Project
        /// </summary>
        /// <param name="id">The id of the Project to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Project/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        /// 
        [HttpDelete(template: "Delete/{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            ServiceResponse response = await _projectService.DeleteProject(id);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            } else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }
            return NoContent();
        }


        /// <summary>
        /// Returns a list of products for a specific category by its {id}
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ProductDto},{ProductDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Product/ListForCategory/3 -> [{ProductDto},{ProductDto},..]
        /// </example>
        /// 
        [HttpGet(template: "ListForUploader/{uploaderId}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> ListProjectsForUploader(int uploaderId)
        {
            IEnumerable<ProjectDto> ProjectDtos = await _projectService.ListProjectsForUploader(uploaderId);
            return Ok(ProjectDtos);
        }
    }
}