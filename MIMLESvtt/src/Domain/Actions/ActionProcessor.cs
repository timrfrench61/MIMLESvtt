namespace MIMLESvtt.src
{
    public class ActionProcessor
    {
        public ActionRecord Process(TableSession tableSession, ActionRequest actionRequest)
        {
            ArgumentNullException.ThrowIfNull(tableSession);
            ArgumentNullException.ThrowIfNull(actionRequest);

            var actionRecord = new ActionRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                ActionType = actionRequest.ActionType,
                ActorParticipantId = actionRequest.ActorParticipantId,
                TimestampUtc = DateTime.UtcNow,
                Payload = actionRequest.Payload
            };

            tableSession.ActionLog.Add(actionRecord);

            return actionRecord;
        }
    }
}
