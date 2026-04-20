namespace MIMLESvtt.src
{
    class PieceInstance
    {
        string Id;
        string DefinitionId;

        Location Location;

        Rotation Rotation;

        string OwnerParticipantId;

        VisibilityState Visibility;

        Dictionary<string, object> State;

        List<string> MarkerIds;

        string StackId;
        string ContainerId;
    }
}
