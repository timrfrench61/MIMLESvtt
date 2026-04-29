namespace MIMLESvtt.src.Domain.Models.Content;

public class ContentValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; } = [];
}
