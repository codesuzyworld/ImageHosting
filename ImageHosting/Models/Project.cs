using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public required string ProjectName { get; set; }
        public required string ProjectDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ImageTotal { get; set; }

        //Each Project Belongs to one Uploader 
        public virtual Uploader Uploader { get; set; }
        public int UploaderId { get; set; }

        //A project can have many items
        public ICollection<Images>? Images { get; set; }
    }
}
