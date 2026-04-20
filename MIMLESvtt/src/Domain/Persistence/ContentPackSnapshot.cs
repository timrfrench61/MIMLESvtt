namespace MIMLESvtt.src
{
    public class ContentPackSnapshot
    {
        public int Version { get; set; }

        public ContentPackManifest? Manifest { get; set; }

        public List<ContentPackDefinition>? Definitions { get; set; }

        public List<ContentPackAsset>? Assets { get; set; }
    }
}
