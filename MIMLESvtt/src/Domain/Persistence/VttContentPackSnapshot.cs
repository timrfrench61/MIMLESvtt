namespace MIMLESvtt.src
{
    public class VttContentPackSnapshot
    {
        public int Version { get; set; }

        public VttContentPackManifest? Manifest { get; set; }

        public List<VttContentPackDefinition>? Definitions { get; set; }

        public List<VttContentPackAsset>? Assets { get; set; }
    }
}
