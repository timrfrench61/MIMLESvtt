using MIMLESvtt.src.Domain.Models.VttContentPack;

namespace MIMLESvtt.src.Domain.Persistence.VttContentPackNSPC
{
    public class VttContentPackSnapshot
    {
        public int Version { get; set; }

        public VttContentPackManifest? Manifest { get; set; }

        public List<VttContentPackDefinition>? Definitions { get; set; }

        public List<VttContentPackAsset>? Assets { get; set; }
    }
}
