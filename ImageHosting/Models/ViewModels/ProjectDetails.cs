namespace ImageHosting.Models.ViewModels
{
    public class ProjectDetails
    {
        //A project page must have a project
        public required ProjectDto Project { get; set; }

        //A Project page can have many Images
        public IEnumerable<ImagesDto>? ProjectImages { get; set; }

        //A Project page can have many tags
        public IEnumerable<TagDto>? ProjectTags { get; set; }

        // For a list of tags to choose from
        public IEnumerable<TagDto>? AllTags { get; set; }


    }
}
