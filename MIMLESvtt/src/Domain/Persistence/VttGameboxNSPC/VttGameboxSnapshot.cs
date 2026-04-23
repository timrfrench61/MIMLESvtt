using MIMLESvtt.src.Domain.Models.VttGamebox;

namespace MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC
{
    public class VttGameboxSnapshot
    {
        public int Version { get; set; }

        public VttGameboxManifest? Manifest { get; set; }
        public List<VttGameboxDefinition>? Definitions { get; set; }

        public List<VttGameboxAsset>? Assets { get; set; }
    }
}
