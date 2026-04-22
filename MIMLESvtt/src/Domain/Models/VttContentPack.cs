namespace MIMLESvtt.src
{
    public class VttContentPack
    {
        public VttContentPackManifest Manifest { get; set; } = new();

        public List<VttContentPackDefinition> Definitions { get; set; } = [];

        public List<VttContentPackAsset> Assets { get; set; } = [];
    }
}
