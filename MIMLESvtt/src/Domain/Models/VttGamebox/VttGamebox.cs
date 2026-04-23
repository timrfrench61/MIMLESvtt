namespace MIMLESvtt.src.Domain.Models.VttGamebox
{
    public class VttGamebox
    {
        public VttGameboxManifest Manifest { get; set; } = new();
        public List<VttGameboxDefinition> Definitions { get; set; } = [];

        public List<VttGameboxAsset> Assets { get; set; } = [];
    }
}
