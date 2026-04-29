namespace MIMLESvtt.src.Domain.Models.Pieces
{
    public class PieceDefinition
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public PieceCategory Category { get; set; } = PieceCategory.Token;

        public Dictionary<string, object> Presentation { get; set; } = [];

        public Dictionary<string, object> RulesetExtensions { get; set; } = [];
    }
}
