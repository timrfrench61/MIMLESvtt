using MIMLESvtt.src.Domain.Models.VttGamebox;

namespace MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC
{
    public static class VttGameboxMapper
    {
        public static VttGameboxSnapshot ToSnapshot(VttGamebox model, int version)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new VttGameboxSnapshot { 
                Version = version,
                Manifest = CloneManifest(model.Manifest),
                Definitions = [.. model.Definitions.Select(CloneDefinition)],
                Assets = [.. model.Assets.Select(CloneAsset)]
            };
        }

        public static VttGamebox FromSnapshot(VttGameboxSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            return new VttGamebox
            {
                Manifest = snapshot.Manifest is null ? new VttGameboxManifest() : CloneManifest(snapshot.Manifest),
                Definitions = snapshot.Definitions is null ? [] : [.. snapshot.Definitions.Select(CloneDefinition)],
                Assets = snapshot.Assets is null ? [] : [.. snapshot.Assets.Select(CloneAsset)]
            };
        }

        private static VttGameboxManifest CloneManifest(VttGameboxManifest source)
        {
            return new VttGameboxManifest
            {
                Name = source.Name,
                Description = source.Description
            };
        }

        private static VttGameboxDefinition CloneDefinition(VttGameboxDefinition source)
        {
            return new VttGameboxDefinition
            {
                Id = source.Id,
                Type = source.Type
            };
        }

        private static VttGameboxAsset CloneAsset(VttGameboxAsset source)
        {
            return new VttGameboxAsset
            {
                Id = source.Id,
                AssetPath = source.AssetPath
            };
        }
    }
}

namespace MIMLESvtt.src.Domain.Persistence.VttGameboxNSPC
{
}
