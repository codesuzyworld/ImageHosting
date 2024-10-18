using ImageHosting.Models;

namespace ImageHosting.Interface
{
    public interface IProjectService
    {
            //base CRUD
            Task<IEnumerable<ProjectDto>> ListProjects();

            Task<ProjectDto?> FindProject(int id);

            Task<ServiceResponse> UpdateProject(ProjectDto projectDto);
            Task<ServiceResponse> AddProject(ProjectDto projectDto);
            Task<ServiceResponse> DeleteProject(int id);

            //Related Methods
            Task<IEnumerable<ProjectDto>> ListProjectsForUploader(int uploaderId);
            Task<IEnumerable<ImagesDto>> ListImagesForProject(int projectId);
            Task<IEnumerable<TagDto>> ListTagsForProject(int projectId);
            Task<ServiceResponse> LinkTagToProject(int tagId, int projectId);
            Task<ServiceResponse> UnlinkTagFromProject(int tagId, int projectId);

    }
}
