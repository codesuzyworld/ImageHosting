using ImageHosting.Interface;
using ImageHosting.Migrations;
using ImageHosting.Models;
using Microsoft.EntityFrameworkCore;

using ImageHosting.Data;

namespace ImageHosting.Services
{
    public class UploaderService : IUploaderService
    {
        private readonly ApplicationDbContext _context;

        public UploaderService(ApplicationDbContext context)
        {
            _context = context;
        }
        // List all uploaders
        public async Task<IEnumerable<UploaderDto>> ListUploaders()
        {
            var uploaders = await _context.Uploader.ToListAsync();
            return uploaders.Select(u => new UploaderDto
            {
                UploaderID = u.UploaderID,
                UploaderName = u.UploaderName,
                UploaderEmail = u.UploaderEmail
            });
        }

        // Find an uploader by ID
        public async Task<UploaderDto?> FindUploader(int id)
        {
            var uploader = await _context.Uploader.FindAsync(id);
            if (uploader == null)
                return null;

            return new UploaderDto
            {
                UploaderID = uploader.UploaderID,
                UploaderName = uploader.UploaderName,
                UploaderEmail = uploader.UploaderEmail
            };
        }

        // Add a new uploader
        public async Task<ServiceResponse> AddUploader(UploaderDto uploaderDto)
        {
            ServiceResponse response = new();
            var uploader = new Uploader
            {
                UploaderName = uploaderDto.UploaderName,
                UploaderEmail = uploaderDto.UploaderEmail
            };

            _context.Uploader.Add(uploader);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = uploader.UploaderID;
            response.Messages.Add("Uploader added successfully.");
            return response;
        }

        // Update an existing uploader
        public async Task<ServiceResponse> UpdateUploader(UploaderDto uploaderDto)
        {
            ServiceResponse response = new();
            var uploader = await _context.Uploader.FindAsync(uploaderDto.UploaderID);
            if (uploader == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Uploader not found.");
                return response;
            }

            uploader.UploaderName = uploaderDto.UploaderName;
            uploader.UploaderEmail = uploaderDto.UploaderEmail;

            _context.Entry(uploader).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            response.Messages.Add("Uploader updated successfully.");
            return response;
        }

        // Delete an uploader
        public async Task<ServiceResponse> DeleteUploader(int id)
        {
            ServiceResponse response = new();
            var uploader = await _context.Uploader.FindAsync(id);
            if (uploader == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Uploader not found.");
                return response;
            }

            _context.Uploader.Remove(uploader);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            response.Messages.Add("Uploader deleted successfully.");
            return response;
        }

    }
}