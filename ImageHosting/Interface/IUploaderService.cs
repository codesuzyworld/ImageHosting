using ImageHosting.Models;

namespace ImageHosting.Interface
{
    public interface IUploaderService
    {
        //base CRUD
        Task<IEnumerable<UploaderDto>> ListUploaders();

        Task<UploaderDto?> FindUploader(int id);

        Task<ServiceResponse> UpdateUploader(UploaderDto uploaderDto);
        Task<ServiceResponse> AddUploader(UploaderDto uploaderDto);
        Task<ServiceResponse> DeleteUploader(int id);
    }
}
