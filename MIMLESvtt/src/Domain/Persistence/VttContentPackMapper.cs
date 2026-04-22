namespace MIMLESvtt.src
{
    public static class VttContentPackMapper
    {
        public static VttContentPackSnapshot ToSnapshot(VttContentPack model, int version)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new VttContentPackSnapshot
            {
                Version = version,
                Manifest = CloneManifest(model.Manifest),
                Definitions = [.. model.Definitions.Select(CloneDefinition)],
                Assets = [.. model.Assets.Select(CloneAsset)]
            };
        }

        public static VttContentPack FromSnapshot(VttContentPackSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            return new VttContentPack
            {
                Manifest = snapshot.Manifest is null ? new VttContentPackManifest() : CloneManifest(snapshot.Manifest),
                Definitions = snapshot.Definitions is null ? [] : [.. snapshot.Definitions.Select(CloneDefinition)],
                Assets = snapshot.Assets is null ? [] : [.. snapshot.Assets.Select(CloneAsset)]
            };
        }

        private static VttContentPackManifest CloneManifest(VttContentPackManifest source)
        {
            return new VttContentPackManifest
            {
                Name = source.Name,
                Description = source.Description
            };
        }

        private static VttContentPackDefinition CloneDefinition(VttContentPackDefinition source)
        {
            return new VttContentPackDefinition
            {
                Id = source.Id,
                Type = source.Type
            };
        }

        private static VttContentPackAsset CloneAsset(VttContentPackAsset source)
        {
            return new VttContentPackAsset
            {
                Id = source.Id,
                AssetPath = source.AssetPath
            };
        }
    }
}
