using ImageHosting.Models;

namespace ImageHosting.Interface
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> ListTags();
        Task<TagDto?> FindTag(int id);
        Task<ServiceResponse> AddTag(TagDto tagDto);
        Task<ServiceResponse> UpdateTag(TagDto tagDto);
        Task<ServiceResponse> DeleteTag(int id);
        Task<IEnumerable<ProjectDto>> ListProjectsForTag(int id);

    }
}