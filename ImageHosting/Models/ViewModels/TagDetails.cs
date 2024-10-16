namespace ImageHosting.Models.ViewModels
{
    public class TagDetails
    {
        public required TagDto Tag { get; set; }
        public IEnumerable<ProjectDto>? AssociatedProjects { get; set; }
    }
}