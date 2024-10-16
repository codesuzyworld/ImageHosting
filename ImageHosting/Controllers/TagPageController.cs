using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Data;
using ImageHosting.Models;
using ImageHosting.Interface;
using ImageHosting.Models.ViewModels;

namespace ImageHosting.Controllers
{
    public class TagPageController : Controller
    {
        private readonly ITagService _tagService;
        private readonly IProjectService _projectService;

        // Dependency injection of the tag service
        public TagPageController(ITagService tagService, IProjectService projectService)
        {
            _tagService = tagService;
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: TagPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<TagDto> tagDtos = await _tagService.ListTags();
            return View(tagDtos);
        }

        // GET: TagPage/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            TagDto? tagDto = await _tagService.FindTag(id);
            IEnumerable<ProjectDto> associatedProjects = await _tagService.ListProjectsForTag(id);

            if (tagDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Tag not found" } });
            }

            var tagDetails = new TagDetails
            {
                Tag = tagDto,
                AssociatedProjects = associatedProjects
            };

            return View(tagDetails);
        }


        // GET: TagPage/New
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        // POST: TagPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(TagDto tagDto)
        {
            if (!ModelState.IsValid)
            {
                return View("New", tagDto);
            }

            var response = await _tagService.AddTag(tagDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        // GET TagPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            TagDto? tagDto = await _tagService.FindTag(id);
            if (tagDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Tag not found" } });
            }
            else
            {
                return View(tagDto);
            }
        }

        // POST TagPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, TagDto tagDto)
        {
            ServiceResponse response = await _tagService.UpdateTag(tagDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        // GET: TagPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var tagDto = await _tagService.FindTag(id);
            if (tagDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Tag not found" } });
            }

            return View(tagDto);
        }

        // POST: TagPage/Delete/{id}
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _tagService.DeleteTag(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

    }
}