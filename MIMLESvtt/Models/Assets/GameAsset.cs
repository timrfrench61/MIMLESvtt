namespace VttMvuModel.Assets;

public sealed record GameAsset(string AssetId, string Name, AssetKind Kind, string Uri, string? ContentType);
