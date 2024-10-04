using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Models
{
    public class Images
    {
        [Key]
        public int ImageID { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }
        public string FilePath { get; set; }

        //An image belongs to one product
        public required virtual Project Project { get; set; }
        public int ProjectID { get; set; }

    }
    public class ImagesDto
    {
        public int ImageID { get; set; }
        public DateTime UploadedAt { get; set; }
        public required string FileName { get; set; }
        public int ProjectID { get; set; }

        //Optional Properties to show URL and Project it belongs to
        public string? FileUrl { get; set; }
        public string FilePath { get; set; }

        public string? ProjectName { get; set; }

    }

    public class ImageCreateDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int ProjectID { get; set; }
    }
}
