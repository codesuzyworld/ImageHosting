using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Models
{
    public class Image
    {
        [Key]
        public int ImageID { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }

        public bool HasPic { get; set; } = false;

        // images stored in /wwwroot/images/projects/{ImageId}.{PicExtension}
        public string? PicExtension { get; set; }

        //An image belongs to one project
        public required virtual Project Project { get; set; }
        public int ProjectID { get; set; }

    }
    public class ImagesDto
    {
        public int ImageID { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }
        public int ProjectID { get; set; }

        public string? ProjectName { get; set; }
        public bool HasPic { get; set; }
        public string? PicExtension { get; set; }
    }

}
