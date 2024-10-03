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

        public async Task<ServiceResponse> AddProject(ProjectDto projectDto)
        {
            ServiceResponse response = new();
            var uploader = await _context.Uploader.FindAsync(projectDto.UploaderId);
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

    }
}
