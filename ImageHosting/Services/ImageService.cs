using ImageHosting.Interface;
using ImageHosting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ImageHosting.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageHosting.Services
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _context;

        public ImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a list of all images by project ID, including associated project details.
        public async Task<IEnumerable<ImagesDto>> ListImages()
        {
            var images = await _context.Images.Include(img => img.Project).ToListAsync();
            return images.Select(img => new ImagesDto
            {
                ImageID = img.ImageID,
                UploadedAt = img.UploadedAt,
                FileName = img.FileName,
                ProjectID = img.ProjectID,
                ProjectName = img.Project.ProjectName,
            });
        }

        // Finds a specific image by its ID, including associated project details.
        // Returns null if the image is not found.
        public async Task<ImagesDto?> FindImage(int id)
        {
            var img = await _context.Images.Include(img => img.Project).FirstOrDefaultAsync(img => img.ImageID == id);
            if (img == null)
                return null;

            return new ImagesDto
            {
                ImageID = img.ImageID,
                UploadedAt = img.UploadedAt,
                FileName = img.FileName,
                ProjectID = img.ProjectID,
                ProjectName = img.Project.ProjectName,
                HasPic = img.HasPic,
                PicExtension = img.PicExtension
            };
        }

        // Adds a new image to the database.
        // The method checks that the image's associated project exists before adding the image.
        public async Task<ServiceResponse> AddImage(ImagesDto imagesDto)
        {
            ServiceResponse response = new();

            var project = await _context.Project.FindAsync(imagesDto.ProjectID);
            if (project == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Project not found.");
                return response;
            }

            var image = new Image
            {
                FileName = imagesDto.FileName,
                UploadedAt = DateTime.Now,
                ProjectID = imagesDto.ProjectID,
                Project = project,
                HasPic = imagesDto.HasPic,
                PicExtension = imagesDto.PicExtension
            };


            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = image.ImageID;
            response.Messages.Add("Image metadata added successfully.");
            return response;
        }

        // Updates the image's FileName.
        public async Task<ServiceResponse> UpdateImage(ImagesDto imagesDto)
        {
            ServiceResponse response = new();

            var image = await _context.Images.FindAsync(imagesDto.ImageID);
            if (image == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Image not found.");
                return response;
            }

            // Only update properties that are allowed to change (e.g., FileName)
            image.FileName = imagesDto.FileName;

            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        // Deletes an image and image file by its ID.
        public async Task<ServiceResponse> DeleteImage(int id)
        {
            ServiceResponse response = new();

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Image not found.");
                return response;
            }

            //We need to delete the image file as well
            string projectImageDirectory = Path.Combine("wwwroot/images/projects/", $"{image.ProjectID}");

            if (image.HasPic && image.PicExtension != null)
            {
                string filePath = Path.Combine(projectImageDirectory, $"{image.ImageID}{image.PicExtension}");

                // Delete the file if it exists
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add($"Can't delete the file");
                        return response;
                    }
                }
            }

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Deleted;
                return response;
            }


        // Lists all images associated with a project by projectId.
        public async Task<IEnumerable<ImagesDto>> ListImagesForProject(int projectId)
        {
            var images = await _context.Images.Where(img => img.ProjectID == projectId).ToListAsync();
            return images.Select(img => new ImagesDto
            {
                ImageID = img.ImageID,
                UploadedAt = img.UploadedAt,
                FileName = img.FileName,
                ProjectID = img.ProjectID,
            });
        }

        // Updates the image file by ImageId.
        // Validates the file extension and replaces the old image file if one exists.
        // Saves the new image file and updates the image in the database.
        public async Task<ServiceResponse> UpdateImageFile(int id, IFormFile ImageFile)
        {
            ServiceResponse response = new();

            Image? Image = await _context.Images.FindAsync(id);
            if (Image == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"Image {id} not found");
                return response;
            }

            if (ImageFile.Length > 0)
            {
                // Generate directory for project-specific images
                string projectImageDirectory = Path.Combine("wwwroot/images/projects/", $"{Image.ProjectID}");
                if (!Directory.Exists(projectImageDirectory))
                {
                    Directory.CreateDirectory(projectImageDirectory);
                }

                // Remove old image if exists
                if (Image.HasPic)
                {
                    string oldFileName = $"{Image.ImageID}{Image.PicExtension}";
                    string oldFilePath = Path.Combine(projectImageDirectory, oldFileName);
                    if (File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Check if image file is provided
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    response.Messages.Add("No file uploaded. Please upload a valid image file.");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                // Establish valid file extensions
                List<string> validExtensions = new List<string> { ".jpeg", ".jpg", ".png", ".gif" };
                string imageFileExtension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                if (!validExtensions.Contains(imageFileExtension))
                {
                    response.Messages.Add($"{imageFileExtension} is not a valid file extension");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                string newFileName = $"{id}{imageFileExtension}";
                string newFilePath = Path.Combine(projectImageDirectory, newFileName);

                // Save the new image file
                using (var targetStream = System.IO.File.Create(newFilePath))
                {
                    await ImageFile.CopyToAsync(targetStream);
                }

                // Verify if file was uploaded successfully
                if (File.Exists(newFilePath))
                {
                    Image.PicExtension = imageFileExtension;
                    Image.HasPic = true;

                    _context.Entry(Image).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add("An error occurred while updating the image.");
                        return response;
                    }
                }
            }
            else
            {
                response.Messages.Add("No File Content");
                response.Status = ServiceResponse.ServiceStatus.Error;
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }
    }
}