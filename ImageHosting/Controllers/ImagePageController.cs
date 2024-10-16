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
using Microsoft.CodeAnalysis;
using ImageHosting.Models.ViewModels;

namespace ImageHosting.Controllers
{
    public class ImagePageController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IImageService _imageService;

        // dependency injection of service interface
        public ImagePageController(IImageService ImageService, IProjectService ProjectService)
        {

            _imageService = ImageService;
            _projectService = ProjectService;

        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ImagePage/New
        // Load the form for adding a new image
        [HttpGet]
        public IActionResult New(int projectId)
        {
            ViewData["ProjectID"] = projectId;
            return View();
        }

        // POST: ImagePage/Add
        // Handle the image file upload and save the image metadata
        [HttpPost]
        public async Task<IActionResult> Add(ImagesDto imagesDto, IFormFile ImageFile)
        {
            var result = await _imageService.AddImage(imagesDto);
            if (result.Status == ServiceResponse.ServiceStatus.Created)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    //Error msg if upload fails
                    var uploadResponse = await _imageService.UpdateImageFile(result.CreatedId, ImageFile);
                    if (uploadResponse.Status == ServiceResponse.ServiceStatus.Error)
                    {
                        ModelState.AddModelError("", string.Join(", ", uploadResponse.Messages));
                        return View("New", imagesDto);
                    }
                }

                return RedirectToAction("Details", "ProjectPage", new { id = imagesDto.ProjectID });
            }

            ModelState.AddModelError("", string.Join(", ", result.Messages));
            return View("New", imagesDto);
        }

        // GET: ImagePage/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            // Fetch image by ID
            ImagesDto? imageDto = await _imageService.FindImage(id);

            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { "Image not found" }
                });
            }

            // Fetch the project details related to the image
            ProjectDto? projectDto = await _projectService.FindProject(imageDto.ProjectID);

            var imageDetails = new ImageDetails
            {
                Image = imageDto,  // Pass the image information
                Project = projectDto // Pass the project related to this image
            };

            return View(imageDetails);
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var imageDto = await _imageService.FindImage(id);
            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Image not found" } });
            }

            return View(imageDto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var imageDto = await _imageService.FindImage(id);
            if (imageDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Image not found" } });
            }

            var response = await _imageService.DeleteImage(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                // Redirect back to the project details page using the retrieved ProjectId
                return RedirectToAction("Details", "ProjectPage", new { id = imageDto.ProjectID });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }
    }
}

