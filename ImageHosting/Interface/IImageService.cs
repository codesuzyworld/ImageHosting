using ImageHosting.Models;

namespace ImageHosting.Interface
{
    public interface IImageService
    {
            // Base CRUD
            Task<IEnumerable<ImagesDto>> ListImages();
            Task<ImagesDto?> FindImage(int id);
            Task<ServiceResponse> AddImage(ImagesDto imagesDto);
            Task<ServiceResponse> UpdateImage(ImagesDto imagesDto);
            Task<ServiceResponse> DeleteImage(int id);

            // Related Methods
            Task<IEnumerable<ImagesDto>> ListImagesForProject(int projectId);
            Task<ServiceResponse> UpdateImageFile(int id, IFormFile ImageFile);

    }
}
