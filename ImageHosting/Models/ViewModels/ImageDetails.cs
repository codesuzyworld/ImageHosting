namespace ImageHosting.Models.ViewModels
{
    public class ImageDetails
    {
        //An Image Page must have an image
        //FindImage(imageID)
        public required ImagesDto Image { get; set; }

        // An image may have a project associated to it
        // ListImagesForProject(projectId)
        public ProjectDto? Project { get; set; }

    }
}
