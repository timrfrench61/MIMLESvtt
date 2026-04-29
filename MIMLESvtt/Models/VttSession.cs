using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.Models.Visibility;
using System.Text.Json.Serialization;

namespace MIMLESvtt.src.Domain.Models
{
    public class VttSession
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<Participant> Participants { get; set; } = [];

        public List<PlayerSeat> PlayerSeats { get; set; } = [];

        public List<SurfaceInstance> Surfaces { get; set; } = [];

        public List<PieceInstance> Pieces { get; set; } = [];

        public List<string> TurnOrder { get; set; } = [];

        public int CurrentTurnIndex { get; set; }

        public int TurnNumber { get; set; } = 1;

        public string CurrentPhase { get; set; } = string.Empty;

        public TurnState TurnState { get; set; } = new();

        public TabletopOptions Options { get; set; } = new();

        public VisibilityState Visibility { get; set; } = new();

        public List<ActionRecord> ActionLog { get; set; } = [];

        public Dictionary<string, object> ModuleState { get; set; } = [];

        public VttCampaign Campaign
        {
            get
            {
                field ??= new VttCampaign();

                if (string.IsNullOrWhiteSpace(field.SessionId))
                {
                    field.SessionId = Id;
                }

                return field;
            }
            set
            {
                if (value is null)
                {
                    field = new VttCampaign
                    {
                        SessionId = Id
                    };
                    return;
                }

                value.SessionId = Id;
                field = value;
            }
        } = new();

        [JsonPropertyName("Campaigns")]
        public List<VttCampaign> LegacyCampaigns
        {
            get => [Campaign];
            set
            {
                var campaign = value.FirstOrDefault() ?? new VttCampaign();
                campaign.SessionId = Id;
                Campaign = campaign;
            }
        }

        [JsonIgnore]
        public VttScenario? CurrentVttScenario
        {
            get => Campaign.CurrentScenarioSnapshot;
            set => Campaign.CurrentScenarioSnapshot = value;
        }
    }

}
