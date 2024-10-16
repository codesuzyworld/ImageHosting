using ImageHosting.Interface;
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
            var projects = await _context.Project.Include(p => p.Uploader).Include(p => p.Tags).ToListAsync();
            return projects.Select(project => new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                CreatedAt = project.CreatedAt,
                UploaderId = project.UploaderId,
                UploaderName = project.Uploader.UploaderName,
                Tags = project.Tags.Select(t => new TagDto 
                { 
                    TagID = t.TagID, 
                    TagName = t.TagName, 
                    TagColor = t.TagColor 
                })

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
                UploaderId = project.UploaderId,
                UploaderName = project.Uploader.UploaderName
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

            var project = await _context.Project
                .Include(p => p.Images) 
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                return response;
            }

            //When deleting a project, we gotta delete the folder too
            string projectImageDirectory = Path.Combine("wwwroot/images/projects/", $"{project.ProjectId}");

            //Delete all the pictures in the folder
            foreach (var image in project.Images)
            {
                string imageFilePath = Path.Combine(projectImageDirectory, $"{image.ImageID}{image.PicExtension}");
                if (File.Exists(imageFilePath))
                {
                    try
                    {
                        File.Delete(imageFilePath);
                    }
                    catch (Exception ex)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add($"Error deleting image file {image.FileName}: {ex.Message}");
                        return response;
                    }
                }
            }

            // Delete the project folder too
            if (Directory.Exists(projectImageDirectory))
            {
                try
                {
                    Directory.Delete(projectImageDirectory);
                }
                catch (Exception ex)
                {
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add($"Error deleting project folder: {ex.Message}");
                    return response;
                }
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

        //This service lists all images for a project by project ID
        public async Task<IEnumerable<ImagesDto>> ListImagesForProject(int projectId)
        {

            var project = await _context.Project
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                return new List<ImagesDto>();
            }

            // Map images to ImageDto
            var images = project.Images.Select(img => new ImagesDto
            {
                FileName = img.FileName,
                ImageID = img.ImageID,
                UploadedAt = img.UploadedAt
            }).ToList();

            return images;
        }

        public async Task<IEnumerable<TagDto>> ListTagsForProject(int projectId)
        {

            var project = await _context.Project
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                return new List<TagDto>(); 
            }

            // Map tags to TagDto
            var tags = project.Tags.Select(tag => new TagDto
            {
                TagID = tag.TagID,
                TagName = tag.TagName,
                TagColor = tag.TagColor
            }).ToList();

            return tags;
        }

        public async Task<ServiceResponse> LinkTagToProject(int tagId, int projectId)
        {
            ServiceResponse serviceResponse = new();

            Tag? tag = await _context.Tag.Include(t => t.Projects).FirstOrDefaultAsync(t => t.TagID == tagId);
            Project? project = await _context.Project.FindAsync(projectId);

            // Validate entities
            if (project == null || tag == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (project == null) serviceResponse.Messages.Add("Project not found.");
                if (tag == null) serviceResponse.Messages.Add("Tag not found.");
                return serviceResponse;
            }

            try
            {
                tag.Projects.Add(project); // Add project to tag
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error linking tag to project.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkTagFromProject(int tagId, int projectId)
        {
            ServiceResponse serviceResponse = new();

            Tag? tag = await _context.Tag.Include(t => t.Projects).FirstOrDefaultAsync(t => t.TagID == tagId);
            Project? project = await _context.Project.FindAsync(projectId);

            // Validate entities
            if (project == null || tag == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (project == null) serviceResponse.Messages.Add("Project not found.");
                if (tag == null) serviceResponse.Messages.Add("Tag not found.");
                return serviceResponse;
            }

            try
            {
                tag.Projects.Remove(project); // Remove project from tag
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error unlinking tag from project.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

    }
}
