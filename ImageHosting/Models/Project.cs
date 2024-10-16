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

        //A project can have many images
        public ICollection<Image>? Images { get; set; }

        //A project can have many tags (Many to Many relationship)
        public ICollection<Tag>? Tags { get; set; }

    }
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public DateTime CreatedAt { get; set; }

        public int UploaderId { get; set; }
        public string UploaderName { get; set; }

        public IEnumerable<TagDto>? Tags { get; set; }


    }
}
