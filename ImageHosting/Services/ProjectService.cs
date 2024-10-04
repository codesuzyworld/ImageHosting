using ImageHosting.Interface;
using ImageHosting.Migrations;
using ImageHosting.Models;
using Microsoft.EntityFrameworkCore;

using ImageHosting.Data;


namespace ImageHosting.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        //This service lists out all projects according to the uploader
        public async Task<IEnumerable<ProjectDto>> ListProjects()
        {
            var projects = await _context.Project.Include(p => p.Uploader).ToListAsync();
            return projects.Select(project => new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreatedAt = project.CreatedAt,
                UploaderId = project.UploaderId
            });
        }

        //This service finds a project by ID
        public async Task<ProjectDto?> FindProject(int id)
        {
            var project = await _context.Project.Include(p => p.Uploader).FirstOrDefaultAsync(p => p.ProjectId == id);
            if (project == null)
                return null;

            return new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreatedAt = project.CreatedAt,
                UploaderId = project.UploaderId
            };
        }

        //This service adds a project

        public async Task<ServiceResponse> AddProject(ProjectDto projectDto)
        {
            ServiceResponse response = new();
            var uploader = await _context.Uploader.FindAsync(projectDto.UploaderId);

            // Data must link to a valid uploader
            if (uploader == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Uploader not found.");
                return response;
            }
            var project = new Project
            {
                ProjectName = projectDto.ProjectName,
                ProjectDescription = projectDto.ProjectDescription,
                CreatedAt = DateTime.Now,
                UploaderId = projectDto.UploaderId,
                ImageTotal = 0
            };

            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = project.ProjectId;
            return response;
        }

        //This service updates a project

        public async Task<ServiceResponse> UpdateProject(ProjectDto projectDto)
        {
            ServiceResponse response = new();
            var project = await _context.Project.FindAsync(projectDto.ProjectId);
            if (project == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Project not found.");
                return response;
            }

            project.ProjectName = projectDto.ProjectName;
            project.ProjectDescription = projectDto.ProjectDescription;

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        //This service deletes a project

        public async Task<ServiceResponse> DeleteProject(int id)
        {
            ServiceResponse response = new();
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }


        //This service lists all projects specific to an uploader, by uploader ID
        public async Task<IEnumerable<ProjectDto>> ListProjectsForUploader(int uploaderId)
        {
            var projects = await _context.Project.Where(p => p.UploaderId == uploaderId).ToListAsync();
            return projects.Select(project => new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreatedAt = project.CreatedAt,
                UploaderId = project.UploaderId
            });
        }

        //This service lists all images within a project by id
        public async Task<IEnumerable<ImagesDto>> ListImagesForProject(int projectId)
        {
            // Retrieve the project and include images
            var project = await _context.Project
                .Include(p => p.Images) // Include the Images navigation property
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                return new List<ImagesDto>(); // Return an empty list if the project is not found
            }

            // Map images to ImageDto
            var images = project.Images.Select(img => new ImagesDto
            {
                ImageID = img.ImageID,
                FileName = img.FileName,
                FilePath = img.FilePath,
                UploadedAt = img.UploadedAt
            }).ToList();

            return images;
        }
    }
}
