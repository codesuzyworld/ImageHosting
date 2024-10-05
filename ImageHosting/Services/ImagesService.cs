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
    public class ImagesService : IImagesService
    {
        private readonly ApplicationDbContext _context;

        public ImagesService(ApplicationDbContext context)
        {
            _context = context;
        }

        // This service lists out all images according to the project
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

        // This service finds image by ID
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
            };
        }

        // This service adds a record of image, it doesn't really upload a file per say
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

            var image = new Images
            {
                FileName = imagesDto.FileName,
                FilePath = imagesDto.FilePath,
                UploadedAt = DateTime.Now,
                ProjectID = imagesDto.ProjectID,
                Project = project
            };


            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = image.ImageID;
            response.Messages.Add("Image metadata added successfully.");
            return response;
        }

        // This service updates ImageName and also FilePath
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
            image.FilePath = imagesDto.FilePath;

            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        // This service deletes image by ID
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

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        // This service lists Images for Project
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
    }
}