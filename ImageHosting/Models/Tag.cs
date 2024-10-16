using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Models
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }

        public required string TagName { get; set; }

        public required string TagColor { get; set; }

        // Many-to-many relationship with Project through TagProject
        public ICollection<Project>? Projects { get; set; }
    }

    public class TagDto
    {
        public int TagID { get; set; }

        public required string TagName { get; set; }

        public required string TagColor { get; set; }


    }
}
