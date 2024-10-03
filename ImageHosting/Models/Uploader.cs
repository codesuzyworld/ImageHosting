using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Models
{
    public class Uploader
    {
        [Key]
        public int UploaderID { get; set; }
        public required string UploaderName { get; set; }

        public required string UploaderEmail { get; set; }

        //An uploader can have many projects
        public ICollection<Project>? Project { get; set; }
    }

    public class UploaderDto
    {
        public int UploaderID { get; set; }
        public required string UploaderName { get; set; }
        public required string UploaderEmail { get; set; }
    }
}
