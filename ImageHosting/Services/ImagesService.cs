using ImageHosting.Interface;
using ImageHosting.Migrations;
using ImageHosting.Models;
using Microsoft.EntityFrameworkCore;

using ImageHosting.Data;

namespace ImageHosting.Services
{
    public class ImagesService : IImagesService
    {
        private readonly ApplicationDbContext _context;

        public ImagesService(ApplicationDbContext context)
        {
            _context = context;
        }

        //This service lists out all images according to the project
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

    }
}
