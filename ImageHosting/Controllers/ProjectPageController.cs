using Microsoft.AspNetCore.Mvc;
using ImageHosting.Interface;
using ImageHosting.Models;
using ImageHosting.Models.ViewModels;
using ImageHosting.Services;

namespace ImageHosting.Controllers
{
    public class ProjectPageController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IImageService _imageService;
        private readonly ITagService _tagService;

        // dependency injection of service interface
        public ProjectPageController(IProjectService ProjectService, IImageService ImageService, ITagService TagService)
        {
            _projectService = ProjectService;
            _imageService = ImageService;
            _tagService = TagService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ProjectPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ProjectDto?> ProjectDtos = await _projectService.ListProjects();
            foreach (var project in ProjectDtos)
            {
                project.Tags = await _projectService.ListTagsForProject(project.ProjectId);
            }
            return View(ProjectDtos);
        }


        // GET: ProjectPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ProjectDto? ProjectDto = await _projectService.FindProject(id);
            IEnumerable<ImagesDto> AssociatedImages = await _imageService.ListImagesForProject(id);
            IEnumerable<TagDto> AssociatedTags = await _projectService.ListTagsForProject(id);
            IEnumerable<TagDto> AllTags = await _tagService.ListTags();

            if (ProjectDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find Project"] });
            }
            else
            {
                // information which drives a project page
                ProjectDetails ProjectInfo = new ProjectDetails()
                {
                    Project = ProjectDto,
                    ProjectImages = AssociatedImages,
                    ProjectTags = AssociatedTags,
                    AllTags = AllTags,
                };
                return View(ProjectInfo);
            }
        }

        // GET ProjectPage/New
        public ActionResult New()
        {
            return View();
        }

        // POST ProjectPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(ProjectDto ProjectDto)
        {
            ServiceResponse response = await _projectService.AddProject(ProjectDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {

                return RedirectToAction("Details", "ProjectPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


        //GET ProjectPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ProjectDto? ProjectDto = await _projectService.FindProject(id);
            if (ProjectDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(ProjectDto);
            }
        }

        //POST ProjectPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, ProjectDto ProjectDto)
        {
            ServiceResponse response = await _projectService.UpdateProject(ProjectDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "ProjectPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET ProjectPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ProjectDto? ProjectDto = await _projectService.FindProject(id);
            if (ProjectDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(ProjectDto);
            }
        }

        //POST ProjectPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _projectService.DeleteProject(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "ProjectPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // POST: ProjectPage/LinkToTag
        [HttpPost]
        public async Task<IActionResult> LinkToTag(int projectId, int tagId)
        {
            ServiceResponse response = await _projectService.LinkTagToProject(tagId, projectId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return RedirectToAction("Details", new { id = projectId });
        }

        // POST: ProjectPage/UnlinkFromTag
        [HttpPost]
        public async Task<IActionResult> UnlinkFromTag(int projectId, int tagId)
        {
            ServiceResponse response = await _projectService.UnlinkTagFromProject(tagId, projectId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return RedirectToAction("Details", new { id = projectId });
        }

    }
}
