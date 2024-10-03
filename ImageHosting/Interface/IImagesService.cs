using ImageHosting.Models;

namespace ImageHosting.Interface
{
    public interface IImagesService
    {
            // Base CRUD
            Task<IEnumerable<ImagesDto>> ListImages();
            Task<ImagesDto?> FindImage(int id);
            Task<ServiceResponse> AddImage(ImagesDto imagesDto);
            Task<ServiceResponse> UpdateImage(ImagesDto imagesDto);
            Task<ServiceResponse> DeleteImage(int id);

            // Related Methods
            Task<IEnumerable<ImagesDto>> ListImagesForProject(int projectId);
        }
}
