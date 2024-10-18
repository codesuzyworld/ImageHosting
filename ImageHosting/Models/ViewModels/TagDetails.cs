namespace ImageHosting.Models.ViewModels
{
    public class TagDetails
    {
        //A Tag page must have a tag
        public required TagDto Tag { get; set; }

        //A Tag page can have many associated projects
        public IEnumerable<ProjectDto>? AssociatedProjects { get; set; }
    }
}