using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Models
{
    public class Images
    {
        [Key]
        public int ImageID { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }
        public required string FilePath { get; set; }

        //An image belongs to one product
        public required virtual Project Project { get; set; }
        public int ProjectID { get; set; }

    }
}
