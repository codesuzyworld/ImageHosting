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
            // Validate file name and also uploaded file, makes sure no empty null stuff are uploaded
            if (string.IsNullOrWhiteSpace(imagesDto.FileName))
            {
                ModelState.AddModelError("FileName", "Image name cannot be empty.");
                return View("New", imagesDto);
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                return View("New", imagesDto);
            }

            // if validation is passed, then proceed with the adding of image
            var result = await _imageService.AddImage(imagesDto);
            if (result.Status == ServiceResponse.ServiceStatus.Created)
            {
                var uploadResponse = await _imageService.UpdateImageFile(result.CreatedId, ImageFile);
                if (uploadResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    ModelState.AddModelError("", string.Join(", ", uploadResponse.Messages));
                    return View("New", imagesDto);
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


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var imageDto = await _imageService.FindImage(id);
            if (imageDto == null)
            {
                return NotFound();
            }
            return View(imageDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ImagesDto imagesDto, IFormFile ImageFile)
        {
            // Validate the file name and also image upload!
            if (string.IsNullOrWhiteSpace(imagesDto.FileName))
            {
                ModelState.AddModelError("FileName", "Image name cannot be empty.");
                return View("Edit", imagesDto);
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                return View("Edit", imagesDto);
            }

            if (ModelState.IsValid)
            {
                //Proceed with image updating if validation passes 
                var response = await _imageService.UpdateImage(imagesDto);
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileResponse = await _imageService.UpdateImageFile(imagesDto.ImageID, ImageFile);
                    if (fileResponse.Status != ServiceResponse.ServiceStatus.Updated)
                    {
                        ModelState.AddModelError(string.Empty, "Image file update failed.");
                    }
                }

                if (response.Status == ServiceResponse.ServiceStatus.Updated)
                {
                    return RedirectToAction("Details", new { id = imagesDto.ImageID });
                }

                ModelState.AddModelError(string.Empty, "Image update failed.");
            }

            return View(imagesDto);
        }

    }
}

