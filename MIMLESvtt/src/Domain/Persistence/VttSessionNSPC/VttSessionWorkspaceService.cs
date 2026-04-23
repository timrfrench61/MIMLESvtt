using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Models.Pieces;
using MIMLESvtt.src.Domain.Models.Placement;
using MIMLESvtt.src.Domain.Models.Surfaces;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.Services.Import;
using MIMLESvtt.src.Domain.Persistence.Services.Scenario;
using MIMLESvtt.src.Domain.Persistence.Snapshot;

using MIMLESvtt.src.Domain.Persistence.VttScenarioNSPC;
using MIMLESvtt.src.Domain.Persistence.Workspace;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionWorkspaceService : IVttSessionCommandService
    {
        private const string MovePieceActionType = "MovePiece";
        private const string RotatePieceActionType = "RotatePiece";
        private const string AddMarkerActionType = "AddMarker";
        private const string RemoveMarkerActionType = "RemoveMarker";
        private const string ChangePieceStateActionType = "ChangePieceState";

        private readonly SnapshotFileWorkflowService _snapshotFileWorkflowService;
        private readonly SnapshotFileImportApplyWorkflowService _snapshotFileImportApplyWorkflowService;
        private readonly VttScenarioPlanApplyService _vttScenarioPlanApplyService;
        private readonly VttScenarioCandidateActivationService _vttScenarioCandidateActivationService;
        private readonly SnapshotFileLibraryService _snapshotFileLibraryService;
        private readonly KnownGameSessionRegistryPersistenceService _knownGameSessionRegistryPersistenceService;
        private readonly VttSessionWorkspaceStatePersistenceService _workspaceStatePersistenceService;
        private readonly ActionProcessor _actionProcessor;
        private readonly ActionValidationService _actionValidationService;
        private readonly Dictionary<string, KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord> _persistedSessionRegistryByPath = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _knownGameSessionRegistryPath;
        private string? _knownGameSessionRegistryWarning;

        public WorkspaceRecoveryDiagnostics? LastRestoreDiagnostics { get; private set; }

        public VttSessionWorkspaceService()
            : this(
                new SnapshotFileWorkflowService(),
                new SnapshotFileImportApplyWorkflowService(),
                new VttScenarioPlanApplyService(),
                new VttScenarioCandidateActivationService(),
                new SnapshotFileLibraryService(),
                new VttSessionWorkspaceStatePersistenceService(),
                new ActionProcessor(),
                new ActionValidationService(),
                null)
        {
        }

        public VttSessionWorkspaceService(
            SnapshotFileWorkflowService snapshotFileWorkflowService,
            SnapshotFileImportApplyWorkflowService snapshotFileImportApplyWorkflowService,
            VttScenarioPlanApplyService vttScenarioPlanApplyService,
            VttScenarioCandidateActivationService vttScenarioCandidateActivationService,
            SnapshotFileLibraryService snapshotFileLibraryService,
            VttSessionWorkspaceStatePersistenceService workspaceStatePersistenceService,
            ActionProcessor actionProcessor,
            ActionValidationService actionValidationService,
            KnownGameSessionRegistryPersistenceService? knownGameSessionRegistryPersistenceService)
        {
            _snapshotFileWorkflowService = snapshotFileWorkflowService ?? throw new ArgumentNullException(nameof(snapshotFileWorkflowService));
            _snapshotFileImportApplyWorkflowService = snapshotFileImportApplyWorkflowService ?? throw new ArgumentNullException(nameof(snapshotFileImportApplyWorkflowService));
            _vttScenarioPlanApplyService = vttScenarioPlanApplyService ?? throw new ArgumentNullException(nameof(vttScenarioPlanApplyService ));
            _vttScenarioCandidateActivationService = vttScenarioCandidateActivationService ?? throw new ArgumentNullException(nameof(vttScenarioCandidateActivationService));
            _snapshotFileLibraryService = snapshotFileLibraryService ?? throw new ArgumentNullException(nameof(snapshotFileLibraryService));
            _knownGameSessionRegistryPersistenceService = knownGameSessionRegistryPersistenceService ?? new KnownGameSessionRegistryPersistenceService(_snapshotFileLibraryService);
            _workspaceStatePersistenceService = workspaceStatePersistenceService ?? throw new ArgumentNullException(nameof(workspaceStatePersistenceService));
            _actionProcessor = actionProcessor ?? throw new ArgumentNullException(nameof(actionProcessor));
            _actionValidationService = actionValidationService ?? throw new ArgumentNullException(nameof(actionValidationService));
            _knownGameSessionRegistryPath = BuildKnownGameSessionRegistryPath();

            State = new VttSessionWorkspaceState();
            LoadKnownGameSessionRegistry();
        }

        public VttSessionWorkspaceState State { get; }

        public VttSession? CurrentVttSession => State.CurrentVttSession;

        public IReadOnlyList<SnapshotFileDescriptor> ListKnownSnapshotFiles()
        {
            return _snapshotFileLibraryService.ListEntries();
        }

        public IReadOnlyList<SnapshotFileDescriptor> ListKnownVttSessionFiles()
        {
            return _snapshotFileLibraryService
                .ListEntries()
                .Where(e => e.DetectedFormatKind == SnapshotFormatKind.VttSessionSnapshot)
                .ToList();
        }

        public IReadOnlyList<KnownGameSession> ListKnownGameSessions()
        {
            var hasNewJoinCodeAssignments = false;
            var sessions = ListKnownVttSessionFiles()
                .Select(entry =>
                {
                    var joinCode = ResolvePersistedJoinCode(entry.FullPath, entry.FileName, out var joinCodeAssigned);
                    hasNewJoinCodeAssignments = hasNewJoinCodeAssignments || joinCodeAssigned;

                    return new KnownGameSession
                    {
                        FilePath = entry.FullPath,
                        FileName = entry.FileName,
                        JoinCode = joinCode,
                        Exists = entry.Exists,
                        LastWriteTimeUtc = entry.LastWriteTimeUtc,
                        LastJoinCodeUpdatedUtc = GetRegistryRecord(entry.FullPath).LastJoinCodeUpdatedUtc,
                        LastJoinedUtc = GetRegistryRecord(entry.FullPath).LastJoinedUtc
                    };
                })
                .ToList();

            if (hasNewJoinCodeAssignments)
            {
                PersistKnownGameSessionRegistry();
            }

            return sessions;
        }

        public void AddKnownSnapshotPath(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            _snapshotFileLibraryService.AddPath(fullPath);
            _snapshotFileLibraryService.RefreshEntry(fullPath);
            EnsurePersistedJoinCodeForSessionPath(fullPath);
            PersistKnownGameSessionRegistry();
        }

        public bool RemoveKnownSnapshotPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var fullPath = Path.GetFullPath(path);
            _snapshotFileLibraryService.RemovePath(fullPath);
            _persistedSessionRegistryByPath.Remove(fullPath);
            PersistKnownGameSessionRegistry();
            return true;
        }

        public void SetCanCreateSession(bool canCreateSession)
        {
            State.CanCreateSession = canCreateSession;
        }

        public void SetKnownGameSessionJoinCode(string path, string joinCode)
        {
            var knownSession = RequireKnownSessionForJoinCodeManagement(path);

            var normalizedJoinCode = NormalizeJoinCode(joinCode);

            var conflict = IsJoinCodeConflict(knownSession.FullPath, normalizedJoinCode);

            if (conflict)
            {
                throw new InvalidOperationException("Join code is already assigned to another known session.");
            }

            var registryRecord = GetRegistryRecord(knownSession.FullPath);
            registryRecord.JoinCode = normalizedJoinCode;
            registryRecord.LastJoinCodeUpdatedUtc = DateTime.UtcNow;
            PersistKnownGameSessionRegistry();
        }

        public string ResetKnownGameSessionJoinCodeToDefault(string path)
        {
            var knownSession = RequireKnownSessionForJoinCodeManagement(path);
            var defaultJoinCode = BuildJoinCode(knownSession.FileName);
            var availableJoinCode = BuildAvailableJoinCode(knownSession.FullPath, defaultJoinCode);
            var registryRecord = GetRegistryRecord(knownSession.FullPath);
            registryRecord.JoinCode = availableJoinCode;
            registryRecord.LastJoinCodeUpdatedUtc = DateTime.UtcNow;
            PersistKnownGameSessionRegistry();
            return availableJoinCode;
        }

        public string GenerateKnownGameSessionJoinCode(string path)
        {
            var knownSession = RequireKnownSessionForJoinCodeManagement(path);
            var seed = BuildJoinCode(knownSession.FileName);
            var availableJoinCode = BuildAvailableJoinCode(knownSession.FullPath, $"{seed}-ALT");
            var registryRecord = GetRegistryRecord(knownSession.FullPath);
            registryRecord.JoinCode = availableJoinCode;
            registryRecord.LastJoinCodeUpdatedUtc = DateTime.UtcNow;
            PersistKnownGameSessionRegistry();
            return availableJoinCode;
        }

        public string GetKnownGameSessionJoinCode(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            if (_persistedSessionRegistryByPath.TryGetValue(fullPath, out var registryRecord)
                && !string.IsNullOrWhiteSpace(registryRecord.JoinCode))
            {
                return registryRecord.JoinCode;
            }

            return BuildJoinCode(Path.GetFileName(fullPath));
        }

        public string GetKnownGameSessionRegistryPath()
        {
            return _knownGameSessionRegistryPath;
        }

        public bool HasKnownGameSessionRegistry()
        {
            return File.Exists(_knownGameSessionRegistryPath);
        }

        public string? GetKnownGameSessionRegistryWarning()
        {
            return _knownGameSessionRegistryWarning;
        }

        public void SaveKnownGameSessionRegistry()
        {
            PersistKnownGameSessionRegistry();
            _knownGameSessionRegistryWarning = null;
        }

        public void ReloadKnownGameSessionRegistry()
        {
            _persistedSessionRegistryByPath.Clear();
            LoadKnownGameSessionRegistry();
        }

        private SnapshotFileDescriptor RequireKnownSessionForJoinCodeManagement(string path)
        {
            if (!State.CanCreateSession)
            {
                throw new InvalidOperationException("Admin rights are required to manage join codes.");
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);
            var knownSession = ListKnownVttSessionFiles().FirstOrDefault(entry =>
                string.Equals(entry.FullPath, fullPath, StringComparison.OrdinalIgnoreCase));

            if (knownSession is null)
            {
                throw new InvalidOperationException("Known session path was not found.");
            }

            return knownSession;
        }

        private bool IsJoinCodeConflict(string sessionPath, string joinCode)
        {
            return ListKnownGameSessions().Any(entry =>
                !string.Equals(entry.FilePath, sessionPath, StringComparison.OrdinalIgnoreCase)
                && string.Equals(entry.JoinCode, joinCode, StringComparison.OrdinalIgnoreCase));
        }

        private string BuildAvailableJoinCode(string sessionPath, string seed)
        {
            var normalizedSeed = NormalizeJoinCode(seed);
            if (!IsJoinCodeConflict(sessionPath, normalizedSeed))
            {
                return normalizedSeed;
            }

            for (var suffix = 2; suffix <= 9999; suffix++)
            {
                var candidate = NormalizeJoinCode($"{normalizedSeed}-{suffix}");
                if (!IsJoinCodeConflict(sessionPath, candidate))
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException("Unable to allocate a unique join code.");
        }

        private void EnsurePersistedJoinCodeForSessionPath(string fullPath)
        {
            if (!fullPath.EndsWith(SnapshotFileExtensions.VttSession, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (_persistedSessionRegistryByPath.ContainsKey(fullPath))
            {
                return;
            }

            _persistedSessionRegistryByPath[fullPath] = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
            {
                JoinCode = BuildJoinCode(Path.GetFileName(fullPath))
            };
        }

        private string ResolvePersistedJoinCode(string fullPath, string fileName, out bool joinCodeAssigned)
        {
            joinCodeAssigned = false;

            if (_persistedSessionRegistryByPath.TryGetValue(fullPath, out var existingRecord)
                && !string.IsNullOrWhiteSpace(existingRecord.JoinCode))
            {
                return existingRecord.JoinCode;
            }

            var generatedJoinCode = BuildJoinCode(fileName);
            _persistedSessionRegistryByPath[fullPath] = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
            {
                JoinCode = generatedJoinCode
            };
            joinCodeAssigned = true;
            return generatedJoinCode;
        }

        private void PersistKnownGameSessionRegistry()
        {
            _knownGameSessionRegistryPersistenceService.SaveRegistry(_knownGameSessionRegistryPath, _persistedSessionRegistryByPath);
        }

        private void LoadKnownGameSessionRegistry()
        {
            try
            {
                var loadedRegistry = _knownGameSessionRegistryPersistenceService.LoadRegistry(_knownGameSessionRegistryPath);
                foreach (var entry in loadedRegistry)
                {
                    _persistedSessionRegistryByPath[entry.Key] = entry.Value;
                }

                _knownGameSessionRegistryWarning = null;
            }
            catch (InvalidOperationException ex)
            {
                _knownGameSessionRegistryWarning = ex.Message;
            }
        }

        private KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord GetRegistryRecord(string fullPath)
        {
            if (_persistedSessionRegistryByPath.TryGetValue(fullPath, out var existingRecord))
            {
                return existingRecord;
            }

            var createdRecord = new KnownGameSessionRegistryPersistenceService.KnownGameSessionRegistryRecord
            {
                JoinCode = BuildJoinCode(Path.GetFileName(fullPath))
            };
            _persistedSessionRegistryByPath[fullPath] = createdRecord;
            return createdRecord;
        }

        private static string BuildKnownGameSessionRegistryPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "App_Data", "known-game-sessions.json");
        }

        private static string NormalizeJoinCode(string joinCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(joinCode);
            return joinCode.Trim().ToUpperInvariant();
        }

        public bool TryJoinExistingGame(string? joinCode, out string message)
        {
            if (string.IsNullOrWhiteSpace(joinCode))
            {
                message = "Join code is required.";
                return false;
            }

            var normalizedCode = joinCode.Trim();
            var knownSessions = ListKnownGameSessions();

            var matched = knownSessions.FirstOrDefault(entry =>
                string.Equals(entry.JoinCode, normalizedCode, StringComparison.OrdinalIgnoreCase)
                || string.Equals(entry.FileName, normalizedCode, StringComparison.OrdinalIgnoreCase)
                || string.Equals(entry.FilePath, normalizedCode, StringComparison.OrdinalIgnoreCase));

            if (matched is not null)
            {
                if (!matched.Exists)
                {
                    message = "Selected game session file is not available.";
                    return false;
                }

                return TryOpenJoinSession(matched.FilePath, "Joined existing game session.", out message, markJoined: true);
            }

            var samplePath = TryResolveSessionFromDocsSamples(normalizedCode);
            if (!string.IsNullOrWhiteSpace(samplePath))
            {
                return TryOpenJoinSession(samplePath, "Joined existing game session from sample snapshots.", out message, markJoined: false);
            }

            message = "Join code did not match any known game session.";
            return false;
        }

        private bool TryOpenJoinSession(string path, string successMessage, out string message, bool markJoined)
        {
            try
            {
                OpenVttSessionFromFile(path);

                if (markJoined)
                {
                    var fullPath = Path.GetFullPath(path);
                    if (_persistedSessionRegistryByPath.TryGetValue(fullPath, out var registryRecord))
                    {
                        registryRecord.LastJoinedUtc = DateTime.UtcNow;
                        PersistKnownGameSessionRegistry();
                    }
                }

                message = successMessage;
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        private static string? TryResolveSessionFromDocsSamples(string joinCode)
        {
            var sampleDir = FindDocsPersistenceDirectory();
            if (string.IsNullOrWhiteSpace(sampleDir))
            {
                return null;
            }

            var candidate = Directory
                .EnumerateFiles(sampleDir, $"*{SnapshotFileExtensions.VttSession}", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(path =>
                    string.Equals(BuildJoinCode(Path.GetFileName(path)), joinCode, StringComparison.OrdinalIgnoreCase));

            return candidate;
        }

        private static string? FindDocsPersistenceDirectory()
        {
            var roots = new List<string>();

            if (!string.IsNullOrWhiteSpace(AppContext.BaseDirectory))
            {
                roots.Add(AppContext.BaseDirectory);
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            if (!string.IsNullOrWhiteSpace(currentDirectory))
            {
                roots.Add(currentDirectory);
            }

            foreach (var root in roots.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var directory = new DirectoryInfo(root);
                while (directory is not null)
                {
                    var direct = Path.Combine(directory.FullName, "docs", "03-persistence");
                    if (Directory.Exists(direct))
                    {
                        return direct;
                    }

                    var nested = Path.Combine(directory.FullName, "MIMLESvtt", "docs", "03-persistence");
                    if (Directory.Exists(nested))
                    {
                        return nested;
                    }

                    directory = directory.Parent;
                }
            }

            return null;
        }

        private static string BuildJoinCode(string fileName)
        {
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return string.Empty;
            }

            var withoutSnapshotSuffix = baseName.EndsWith(".vttsession", StringComparison.OrdinalIgnoreCase)
                ? baseName[..^".vttsession".Length]
                : baseName;

            if (string.IsNullOrWhiteSpace(withoutSnapshotSuffix))
            {
                return fileName;
            }

            return withoutSnapshotSuffix.ToUpperInvariant();
        }

        public void CreateNewSession()
        {
            if (!State.CanCreateSession)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.CreateNewSession,
                    filePath: null,
                    snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                    message: "Only admins can create new sessions from Game Selector.");

                throw new InvalidOperationException("Only admins can create new sessions from Game Selector.");
            }

            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.CreateNewSession,
                filePath: null,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Created new workspace session.",
                action: () =>
                {
                    State.CurrentVttSession = new VttSession
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = "New Session"
                    };
                    State.CurrentFilePath = null;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.VttSessionSnapshot;
                    State.IsDirty = false;
                    State.CurrentPendingVttScenarioPlan = null;
                    State.PendingVttScenarioSourcePath = null;
                    ClearUndoRedoHistory();
                });
        }

        public void ReportFeatureNotEnabled()
        {
            State.LastOperationMessage = "Feature not enabled (development toggle)";
            State.LastOperationSeverity = WorkspaceMessageSeverity.Info;
        }

        public void UpdateSessionTitle(string title)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.UpdateSessionTitle,
                filePath: State.CurrentFilePath,
                snapshotFormat: State.CurrentSnapshotFormat,
                successMessage: "Updated session title.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to update session title.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(title);
                    State.CurrentVttSession.Title = title.Trim();
                    State.IsDirty = true;
                });
        }

        public void SaveCurrentLayoutAsScenario(string scenarioTitle, string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveCurrentLayoutAsScenario,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.VttScenarioSnapshot,
                successMessage: "Saved current layout as scenario.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to save scenario layout.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(scenarioTitle);
                    ArgumentException.ThrowIfNullOrWhiteSpace(path);

                    var scenario = BuildScenarioExportFromCurrentSession(State.CurrentVttSession, scenarioTitle.Trim());
                    _snapshotFileWorkflowService.SaveScenario(scenario, path);

                    var fullPath = Path.GetFullPath(path);
                    _snapshotFileLibraryService.AddPath(fullPath);
                    _snapshotFileLibraryService.RefreshEntry(fullPath);
                    PersistKnownGameSessionRegistry();
                });
        }

        public void InitializeTurnOrder(List<string> turnOrder)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.InitializeTurnOrder,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Initialized turn order.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to initialize turn order.");
                    }

                    ArgumentNullException.ThrowIfNull(turnOrder);

                    var normalizedOrder = turnOrder
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .Select(id => id.Trim())
                        .ToList();

                    if (normalizedOrder.Count == 0)
                    {
                        throw new InvalidOperationException("Turn order requires at least one participant id.");
                    }

                    State.CurrentVttSession.TurnOrder = normalizedOrder;
                    State.CurrentVttSession.CurrentTurnIndex = 0;
                    State.CurrentVttSession.TurnNumber = 1;
                    State.IsDirty = true;
                });
        }

        public void MoveTurnParticipantUp(string participantId)
        {
            ReorderTurnParticipant(participantId, moveUp: true);
        }

        public void MoveTurnParticipantDown(string participantId)
        {
            ReorderTurnParticipant(participantId, moveUp: false);
        }

        private void ReorderTurnParticipant(string participantId, bool moveUp)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.ReorderTurnParticipant,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: moveUp ? "Moved turn participant up." : "Moved turn participant down.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to reorder turn participants.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(participantId);

                    var currentIndex = State.CurrentVttSession.TurnOrder.FindIndex(id => string.Equals(id, participantId.Trim(), StringComparison.Ordinal));
                    if (currentIndex < 0)
                    {
                        throw new InvalidOperationException("Participant id was not found in current turn order.");
                    }

                    if (State.CurrentVttSession.TurnOrder.Count < 2)
                    {
                        return;
                    }

                    var targetIndex = moveUp ? currentIndex - 1 : currentIndex + 1;
                    if (targetIndex < 0 || targetIndex >= State.CurrentVttSession.TurnOrder.Count)
                    {
                        return;
                    }

                    (State.CurrentVttSession.TurnOrder[currentIndex], State.CurrentVttSession.TurnOrder[targetIndex]) =
                        (State.CurrentVttSession.TurnOrder[targetIndex], State.CurrentVttSession.TurnOrder[currentIndex]);

                    if (State.CurrentVttSession.CurrentTurnIndex == currentIndex)
                    {
                        State.CurrentVttSession.CurrentTurnIndex = targetIndex;
                    }
                    else if (State.CurrentVttSession.CurrentTurnIndex == targetIndex)
                    {
                        State.CurrentVttSession.CurrentTurnIndex = currentIndex;
                    }

                    State.IsDirty = true;
                });
        }

        public void AdvanceTurn()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AdvanceTurn,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Advanced to next turn.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to advance turn.");
                    }

                    if (State.CurrentVttSession.TurnOrder.Count == 0)
                    {
                        throw new InvalidOperationException("Turn order is required before advancing turn.");
                    }

                    var isLastParticipant = State.CurrentVttSession.CurrentTurnIndex >= State.CurrentVttSession.TurnOrder.Count - 1;
                    var nextIndex = State.CurrentVttSession.CurrentTurnIndex + 1;
                    State.CurrentVttSession.CurrentTurnIndex = nextIndex % State.CurrentVttSession.TurnOrder.Count;

                    if (isLastParticipant)
                    {
                        State.CurrentVttSession.TurnNumber++;
                    }

                    State.IsDirty = true;
                });
        }

        public void PreviousTurn()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.PreviousTurn,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Moved to previous turn.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to move to previous turn.");
                    }

                    if (State.CurrentVttSession.TurnOrder.Count == 0)
                    {
                        throw new InvalidOperationException("Turn order is required before moving to previous turn.");
                    }

                    var isFirstParticipant = State.CurrentVttSession.CurrentTurnIndex == 0;
                    var previousIndex = State.CurrentVttSession.CurrentTurnIndex - 1;
                    if (previousIndex < 0)
                    {
                        previousIndex = State.CurrentVttSession.TurnOrder.Count - 1;
                    }

                    State.CurrentVttSession.CurrentTurnIndex = previousIndex;

                    if (isFirstParticipant)
                    {
                        State.CurrentVttSession.TurnNumber = Math.Max(1, State.CurrentVttSession.TurnNumber - 1);
                    }

                    State.IsDirty = true;
                });
        }

        public void SetPhase(string phase)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SetPhase,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Updated current phase.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to set phase.");
                    }

                    State.CurrentVttSession.CurrentPhase = phase?.Trim() ?? string.Empty;
                    State.IsDirty = true;
                });
        }

        public void AddParticipant(string id, string name)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AddParticipant,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Added participant.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to add participant.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(id);
                    ArgumentException.ThrowIfNullOrWhiteSpace(name);

                    var normalizedId = id.Trim();
                    var normalizedName = name.Trim();

                    if (State.CurrentVttSession.Participants.Any(p => string.Equals(p.Id, normalizedId, StringComparison.Ordinal)))
                    {
                        throw new InvalidOperationException("Participant id already exists in current session.");
                    }

                    State.CurrentVttSession.Participants.Add(new Participant
                    {
                        Id = normalizedId,
                        Name = normalizedName
                    });

                    State.IsDirty = true;
                });
        }

        public void SetWorkspaceMode(WorkspaceMode mode)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SetWorkspaceMode,
                filePath: State.CurrentFilePath,
                snapshotFormat: State.CurrentSnapshotFormat,
                successMessage: $"Switched workspace mode to {mode}.",
                action: () =>
                {
                    State.Mode = mode;
                });
        }

        public void RemoveParticipant(string id)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.RemoveParticipant,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Removed participant.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to remove participant.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(id);
                    var normalizedId = id.Trim();

                    var participant = State.CurrentVttSession.Participants.FirstOrDefault(p => string.Equals(p.Id, normalizedId, StringComparison.Ordinal));
                    if (participant is null)
                    {
                        throw new InvalidOperationException("Participant id was not found in current session.");
                    }

                    State.CurrentVttSession.Participants.Remove(participant);

                    foreach (var piece in State.CurrentVttSession.Pieces)
                    {
                        if (string.Equals(piece.OwnerParticipantId, normalizedId, StringComparison.Ordinal))
                        {
                            piece.OwnerParticipantId = string.Empty;
                        }
                    }

                    State.CurrentVttSession.TurnOrder.RemoveAll(participantId => string.Equals(participantId, normalizedId, StringComparison.Ordinal));

                    if (State.CurrentVttSession.TurnOrder.Count == 0)
                    {
                        State.CurrentVttSession.CurrentTurnIndex = 0;
                    }
                    else if (State.CurrentVttSession.CurrentTurnIndex >= State.CurrentVttSession.TurnOrder.Count)
                    {
                        State.CurrentVttSession.CurrentTurnIndex = 0;
                    }

                    State.IsDirty = true;
                });
        }

        public void RenameParticipant(string id, string newName)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.RenameParticipant,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Renamed participant.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to rename participant.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(id);
                    ArgumentException.ThrowIfNullOrWhiteSpace(newName);

                    var normalizedId = id.Trim();
                    var participant = State.CurrentVttSession.Participants.FirstOrDefault(p => string.Equals(p.Id, normalizedId, StringComparison.Ordinal));
                    if (participant is null)
                    {
                        throw new InvalidOperationException("Participant id was not found in current session.");
                    }

                    participant.Name = newName.Trim();
                    State.IsDirty = true;
                });
        }

        public void AssignPieceOwner(string pieceId, string ownerParticipantId)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AssignPieceOwner,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Updated piece owner.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to assign piece owner.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(pieceId);

                    var piece = State.CurrentVttSession.Pieces.FirstOrDefault(p => string.Equals(p.Id, pieceId.Trim(), StringComparison.Ordinal));
                    if (piece is null)
                    {
                        throw new InvalidOperationException("Piece id was not found in current session.");
                    }

                    var normalizedOwnerParticipantId = ownerParticipantId?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(normalizedOwnerParticipantId) &&
                        State.CurrentVttSession.Participants.All(p => !string.Equals(p.Id, normalizedOwnerParticipantId, StringComparison.Ordinal)))
                    {
                        throw new InvalidOperationException("Owner participant id was not found in current session.");
                    }

                    piece.OwnerParticipantId = normalizedOwnerParticipantId;
                    State.IsDirty = true;
                });
        }

        public void AddSurface(
            string surfaceId,
            string definitionId,
            SurfaceType surfaceType,
            CoordinateSystem coordinateSystem)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AddSurface,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Added surface to current session.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to add a surface.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);
                    ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);

                    if (State.CurrentVttSession.Surfaces.Any(s => s.Id == surfaceId))
                    {
                        throw new InvalidOperationException("Surface id already exists in current session.");
                    }

                    State.CurrentVttSession.Surfaces.Add(new SurfaceInstance
                    {
                        Id = surfaceId,
                        DefinitionId = definitionId,
                        Type = surfaceType,
                        CoordinateSystem = coordinateSystem
                    });

                    RegisterUndoEntry(new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.AddSurface,
                        SurfaceSnapshot = CloneSurface(State.CurrentVttSession.Surfaces.Last())
                    });

                    State.IsDirty = true;
                });
        }

        public void AddPiece(
            string pieceId,
            string definitionId,
            string surfaceId,
            float x,
            float y,
            string ownerParticipantId,
            float rotationDegrees)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AddPiece,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Added piece to current session.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to add a piece.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(pieceId);
                    ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);
                    ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);

                    if (!State.CurrentVttSession.Surfaces.Any(s => s.Id == surfaceId))
                    {
                        throw new InvalidOperationException("Target surface was not found in current session.");
                    }

                    if (State.CurrentVttSession.Pieces.Any(p => p.Id == pieceId))
                    {
                        throw new InvalidOperationException("Piece id already exists in current session.");
                    }

                    var normalizedOwnerParticipantId = ownerParticipantId?.Trim() ?? string.Empty;

                    State.CurrentVttSession.Pieces.Add(new PieceInstance
                    {
                        Id = pieceId,
                        DefinitionId = definitionId,
                        OwnerParticipantId = normalizedOwnerParticipantId,
                        Location = new Location
                        {
                            SurfaceId = surfaceId,
                            Coordinate = new Coordinate
                            {
                                X = x,
                                Y = y
                            }
                        },
                        Rotation = new Rotation
                        {
                            Degrees = rotationDegrees
                        }
                    });

                    RegisterUndoEntry(new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.AddPiece,
                        PieceId = pieceId,
                        PieceSnapshot = ClonePiece(State.CurrentVttSession.Pieces.Last())
                    });

                    State.IsDirty = true;
                });
        }

        public void OpenVttSessionFromFile(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.OpenVttSessionFromFile,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Opened table session from file.",
                action: () =>
                {
                    var loadedSession = _snapshotFileWorkflowService.LoadVttSession(path);
                    var fullPath = Path.GetFullPath(path);

                    State.CurrentVttSession = loadedSession;
                    State.CurrentFilePath = fullPath;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.VttSessionSnapshot;
                    State.IsDirty = false;
                    State.CurrentPendingVttScenarioPlan = null;
                    State.PendingVttScenarioSourcePath = null;
                    ClearUndoRedoHistory();

                    _snapshotFileLibraryService.AddPath(fullPath);
                    _snapshotFileLibraryService.RefreshEntry(fullPath);
                    EnsurePersistedJoinCodeForSessionPath(fullPath);
                    PersistKnownGameSessionRegistry();
                });
        }

        public void SaveCurrentSession()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveCurrentSession,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Saved current session to file.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to save.");
                    }

                    if (string.IsNullOrWhiteSpace(State.CurrentFilePath))
                    {
                        throw new InvalidOperationException("CurrentFilePath is required to save current session.");
                    }

                    _snapshotFileWorkflowService.SaveVttSession(State.CurrentVttSession, State.CurrentFilePath);
                    State.IsDirty = false;

                    _snapshotFileLibraryService.AddPath(State.CurrentFilePath);
                    _snapshotFileLibraryService.RefreshEntry(State.CurrentFilePath);
                    EnsurePersistedJoinCodeForSessionPath(State.CurrentFilePath);
                    PersistKnownGameSessionRegistry();
                });
        }

        public void SaveCurrentSessionAs(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveCurrentSessionAs,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.VttSessionSnapshot,
                successMessage: "Saved current session as a new file.",
                action: () =>
                {
                    if (State.CurrentVttSession is null)
                    {
                        throw new InvalidOperationException("CurrentVttSession is required to save.");
                    }

                    _snapshotFileWorkflowService.SaveVttSession(State.CurrentVttSession, path);

                    var fullPath = Path.GetFullPath(path);
                    State.CurrentFilePath = fullPath;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.VttSessionSnapshot;
                    State.IsDirty = false;

                    _snapshotFileLibraryService.AddPath(fullPath);
                    _snapshotFileLibraryService.RefreshEntry(fullPath);
                    EnsurePersistedJoinCodeForSessionPath(fullPath);
                    PersistKnownGameSessionRegistry();
                });
        }

        public void ImportScenarioToPendingPlanFromFile(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.ImportScenarioToPendingPlanFromFile,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.VttScenarioSnapshot,
                successMessage: "Imported scenario to pending plan.",
                action: () =>
                {
                    var context = new SnapshotImportApplyContext
                    {
                        CurrentVttSession = State.CurrentVttSession
                    };

                    var response = _snapshotFileImportApplyWorkflowService.ImportAndApplyFromFile(
                        path,
                        context,
                        VttScenarioCandidateActivationMode.DryRun,
                        SnapshotImportApplyPolicy.Default);

                    if (!response.IsSuccess)
                    {
                        throw new InvalidOperationException(response.ErrorMessage ?? "Scenario file import failed.");
                    }

                    if (response.DetectedFormat != SnapshotFormatKind.VttScenarioSnapshot)
                    {
                        throw new InvalidOperationException("Scenario file import did not produce VttScenarioSnapshot format.");
                    }

                    var pendingPlan = response.ScenarioActivationResponse?.PendingVttScenarioPlan;
                    if (pendingPlan is null)
                    {
                        throw new InvalidOperationException("Scenario import did not produce a pending scenario plan.");
                    }

                    State.CurrentPendingVttScenarioPlan = pendingPlan;
                    State.PendingVttScenarioSourcePath = Path.GetFullPath(path);
                });
        }

        public void ActivatePendingScenario()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.ActivatePendingScenario,
                filePath: null,
                snapshotFormat: SnapshotFormatKind.VttScenarioSnapshot,
                successMessage: "Activated pending scenario into current session.",
                action: () =>
                {
                    if (State.CurrentPendingVttScenarioPlan is null)
                    {
                        throw new InvalidOperationException("Pending VTT scenario plan is required for activation.");
                    }

                    var candidateResult = _vttScenarioPlanApplyService.Apply(new VttScenarioPlanApplyRequest
                    {
                        PendingVttScenarioPlan = State.CurrentPendingVttScenarioPlan,
                        ActiveRuntimeVttSession = State.CurrentVttSession
                    });

                    if (!candidateResult.IsSuccess || candidateResult.VttSessionCandidate is null)
                    {
                        throw new InvalidOperationException(candidateResult.ErrorMessage ?? "Failed to create table session candidate from pending scenario plan.");
                    }

                    var activationContext = new SnapshotImportApplyContext
                    {
                        CurrentVttSession = State.CurrentVttSession
                    };

                    var activationResult = _vttScenarioCandidateActivationService.Activate(new VttScenarioCandidateActivationRequest
                    {
                        VttSessionCandidate = candidateResult.VttSessionCandidate,
                        TargetContext = activationContext,
                        Mode = VttScenarioCandidateActivationMode.Activate
                    }, SnapshotImportApplyPolicy.Default);

                    if (!activationResult.IsSuccess)
                    {
                        throw new InvalidOperationException(activationResult.Message ?? activationResult.ErrorMessage ?? "Failed to activate pending scenario.");
                    }

                    State.CurrentVttSession = activationContext.CurrentVttSession;
                    State.CurrentFilePath = null;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.VttSessionSnapshot;
                    State.CurrentPendingVttScenarioPlan = null;
                    State.PendingVttScenarioSourcePath = null;
                    State.IsDirty = true;
                    ClearUndoRedoHistory();
                });
        }

        public void SaveWorkspaceState(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveWorkspaceState,
                filePath: path,
                snapshotFormat: State.CurrentSnapshotFormat,
                successMessage: "Saved workspace recovery state.",
                action: () => _workspaceStatePersistenceService.SaveWorkspaceState(State, path));
        }

        public void RestoreWorkspaceState(string path)
        {
            RestoreWorkspaceState(path, WorkspaceRestoreOptions.Default);
        }

        public void RestoreWorkspaceState(string path, WorkspaceRestoreOptions options)
        {
            RestoreWorkspaceStateWithDiagnostics(path, options);
        }

        public WorkspaceRecoveryDiagnostics RestoreWorkspaceStateWithDiagnostics(string path)
        {
            return RestoreWorkspaceStateWithDiagnostics(path, WorkspaceRestoreOptions.Default);
        }

        public WorkspaceRecoveryDiagnostics RestoreWorkspaceStateWithDiagnostics(string path, WorkspaceRestoreOptions options)
        {
            var diagnostics = new WorkspaceRecoveryDiagnostics();
            options ??= WorkspaceRestoreOptions.Default;

            try
            {
                ExecuteWorkspaceOperation(
                    WorkspaceOperationKind.RestoreWorkspaceState,
                    filePath: path,
                    snapshotFormat: null,
                    successMessage: "Restored workspace recovery state.",
                    action: () =>
                    {
                        var recoveryState = _workspaceStatePersistenceService.LoadWorkspaceState(path);
                        var persistenceDiagnostics = _workspaceStatePersistenceService.LastRecoveryDiagnostics;
                        if (persistenceDiagnostics is not null)
                        {
                            ApplyDiagnostics(diagnostics, persistenceDiagnostics);
                        }

                        var workingSession = State.CurrentVttSession;
                        var workingCurrentFilePath = State.CurrentFilePath;
                        var workingSnapshotFormat = State.CurrentSnapshotFormat;
                        var workingPendingVttScenarioPlan = State.CurrentPendingVttScenarioPlan;
                        var workingPendingVttScenarioSourcePath = State.PendingVttScenarioSourcePath;

                        if (!string.IsNullOrWhiteSpace(recoveryState.CurrentFilePath))
                        {
                            if (!File.Exists(recoveryState.CurrentFilePath))
                            {
                                diagnostics.Errors.Add("Workspace recovery failed: referenced current session file was not found.");
                                throw new InvalidOperationException("Workspace recovery failed: referenced current session file was not found.");
                            }

                            var session = _snapshotFileWorkflowService.LoadVttSession(recoveryState.CurrentFilePath);
                            var fullSessionPath = Path.GetFullPath(recoveryState.CurrentFilePath);

                            workingSession = session;
                            workingCurrentFilePath = fullSessionPath;
                            workingSnapshotFormat = SnapshotFormatKind.VttSessionSnapshot;
                            diagnostics.CurrentVttSessionRestored = true;

                            _snapshotFileLibraryService.AddPath(fullSessionPath);
                            _snapshotFileLibraryService.RefreshEntry(fullSessionPath);
                            EnsurePersistedJoinCodeForSessionPath(fullSessionPath);
                            PersistKnownGameSessionRegistry();
                        }

                        if (!string.IsNullOrWhiteSpace(recoveryState.PendingVttScenarioSourcePath))
                        {
                            if (!File.Exists(recoveryState.PendingVttScenarioSourcePath))
                            {
                                if (options.MissingPendingVttScenarioBehavior == VttScenarioModeMissingPending.WarnAndContinue)
                                {
                                    diagnostics.Warnings.Add("Pending VTT scenario file was not found during restore.");
                                    diagnostics.PendingVttScenarioRestored = false;
                                    workingPendingVttScenarioPlan = null;
                                    workingPendingVttScenarioSourcePath = null;
                                }
                                else
                                {
                                    if (diagnostics.CurrentVttSessionRestored)
                                    {
                                        diagnostics.Warnings.Add("Pending scenario file was not found during restore.");
                                    }

                                    diagnostics.Errors.Add("Workspace recovery failed: referenced pending scenario file was not found.");
                                    throw new InvalidOperationException("Workspace recovery failed: referenced pending scenario file was not found.");
                                }
                            }

                            else
                            {
                                var importContext = new SnapshotImportApplyContext
                                {
                                    CurrentVttSession = workingSession
                                };

                                var response = _snapshotFileImportApplyWorkflowService.ImportAndApplyFromFile(
                                    recoveryState.PendingVttScenarioSourcePath,
                                    importContext,
                                    VttScenarioCandidateActivationMode.DryRun,
                                    SnapshotImportApplyPolicy.Default);

                                if (!response.IsSuccess)
                                {
                                    diagnostics.Errors.Add(response.ErrorMessage ?? "Workspace recovery failed while rebuilding pending scenario plan.");
                                    throw new InvalidOperationException(response.ErrorMessage ?? "Workspace recovery failed while rebuilding pending scenario plan.");
                                }

                                var pendingPlan = response.ScenarioActivationResponse?.PendingVttScenarioPlan;
                                if (pendingPlan is null)
                                {
                                    diagnostics.Errors.Add("Workspace recovery failed: pending Vtt scenario plan was not rebuilt.");
                                    throw new InvalidOperationException("Workspace recovery failed: pending Vtt scenario plan was not rebuilt.");
                                }

                                workingPendingVttScenarioPlan = pendingPlan;
                                workingPendingVttScenarioSourcePath = Path.GetFullPath(recoveryState.PendingVttScenarioSourcePath);
                                diagnostics.PendingVttScenarioRestored = true;
                            }
                        }

                        else
                        {
                            workingPendingVttScenarioPlan = null;
                            workingPendingVttScenarioSourcePath = null;
                        }

                        State.CurrentVttSession = workingSession;
                        State.CurrentFilePath = workingCurrentFilePath;
                        State.CurrentSnapshotFormat = workingSnapshotFormat;
                        State.CurrentPendingVttScenarioPlan = workingPendingVttScenarioPlan;
                        State.PendingVttScenarioSourcePath = workingPendingVttScenarioSourcePath;
                        State.IsDirty = false;
                        ClearUndoRedoHistory();

                        if (recoveryState.RecentOperationHistory.Count > 0)
                        {
                            State.OperationHistory.Clear();
                            State.OperationHistory.AddRange(recoveryState.RecentOperationHistory);
                        }

                        diagnostics.IsSuccess = true;

                        if (diagnostics.SourceUsed == WorkspaceRecoverySource.Backup)
                        {
                            State.LastOperationMessage = "Restored workspace from backup state file.";
                        }
                    });
            }
            catch
            {
                diagnostics.IsSuccess = false;

                var persistenceDiagnostics = _workspaceStatePersistenceService.LastRecoveryDiagnostics;
                if (persistenceDiagnostics is not null)
                {
                    ApplyDiagnostics(diagnostics, persistenceDiagnostics);
                }

                throw;
            }
            finally
            {
                LastRestoreDiagnostics = diagnostics;
            }

            return diagnostics;
        }

        public ActionRecord ProcessAction(ActionRequest request)
        {
            if (State.CurrentVttSession is null)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.ProcessAction,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Cannot process action without a current table session.");

                throw new InvalidOperationException("CurrentVttSession is required to process action.");
            }

            try
            {
                if (State.Mode == WorkspaceMode.Play && State.Settings.EnableRulesValidation)
                {
                    var validation = _actionValidationService.Validate(request, State.CurrentVttSession);
                    if (!validation.IsValid)
                    {
                        throw new InvalidOperationException(validation.Message);
                    }
                }

                var undoEntry = CreateUndoEntryForAction(State.CurrentVttSession, request);
                var actionRecord = _actionProcessor.Process(State.CurrentVttSession, request);

                if (undoEntry is not null)
                {
                    RegisterUndoEntry(undoEntry);
                }

                State.IsDirty = true;
                State.LastOperationMessage = "Processed action through workspace session.";

                RecordSuccessOperation(
                    WorkspaceOperationKind.ProcessAction,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Processed action through workspace session.");

                return actionRecord;
            }
            catch (Exception ex)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.ProcessAction,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: ex.Message);

                throw;
            }
        }

        public void UndoLastOperation()
        {
            if (State.CurrentVttSession is null)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.UndoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Cannot undo without a current table session.");

                throw new InvalidOperationException("CurrentVttSession is required to undo.");
            }

            if (!State.CanUndo)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.UndoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Nothing is available to undo.");

                throw new InvalidOperationException("Nothing is available to undo.");
            }

            var entry = State.UndoStack[^1];
            State.UndoStack.RemoveAt(State.UndoStack.Count - 1);

            try
            {
                ApplyUndoEntry(State.CurrentVttSession, entry, undoing: true);
                State.RedoStack.Add(entry);
                State.IsDirty = true;
                State.LastOperationMessage = $"Undid {entry.OperationKind}.";

                RecordSuccessOperation(
                    WorkspaceOperationKind.UndoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: State.LastOperationMessage);
            }
            catch (Exception ex)
            {
                State.UndoStack.Add(entry);

                RecordFailureOperation(
                    WorkspaceOperationKind.UndoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: ex.Message);

                throw;
            }
        }

        public void RedoLastOperation()
        {
            if (State.CurrentVttSession is null)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.RedoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Cannot redo without a current table session.");

                throw new InvalidOperationException("CurrentVttSession is required to redo.");
            }

            if (!State.CanRedo)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.RedoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Nothing is available to redo.");

                throw new InvalidOperationException("Nothing is available to redo.");
            }

            var entry = State.RedoStack[^1];
            State.RedoStack.RemoveAt(State.RedoStack.Count - 1);

            try
            {
                ApplyUndoEntry(State.CurrentVttSession, entry, undoing: false);
                State.UndoStack.Add(entry);
                State.IsDirty = true;
                State.LastOperationMessage = $"Redid {entry.OperationKind}.";

                RecordSuccessOperation(
                    WorkspaceOperationKind.RedoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: State.LastOperationMessage);
            }
            catch (Exception ex)
            {
                State.RedoStack.Add(entry);

                RecordFailureOperation(
                    WorkspaceOperationKind.RedoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: ex.Message);

                throw;
            }
        }

        private WorkspaceUndoEntry? CreateUndoEntryForAction(VttSession VttSession, ActionRequest request)
        {
            switch (request.ActionType)
            {
                case MovePieceActionType:
                {
                    if (request.Payload is not MovePiecePayload payload)
                    {
                        return null;
                    }

                    var piece = VttSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
                        ?? throw new InvalidOperationException("MovePiece target piece was not found for undo capture.");

                    return new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.MovePiece,
                        PieceId = piece.Id,
                        FromLocation = CloneLocation(piece.Location),
                        ToLocation = CloneLocation(payload.NewLocation)
                    };
                }

                case RotatePieceActionType:
                {
                    if (request.Payload is not RotatePiecePayload payload)
                    {
                        return null;
                    }

                    var piece = VttSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
                        ?? throw new InvalidOperationException("RotatePiece target piece was not found for undo capture.");

                    return new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.RotatePiece,
                        PieceId = piece.Id,
                        FromRotation = CloneRotation(piece.Rotation),
                        ToRotation = CloneRotation(payload.NewRotation)
                    };
                }

                case AddMarkerActionType:
                {
                    if (request.Payload is not AddMarkerPayload payload)
                    {
                        return null;
                    }

                    var piece = VttSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
                        ?? throw new InvalidOperationException("AddMarker target piece was not found for undo capture.");

                    var markerId = payload.MarkerId?.Trim() ?? string.Empty;
                    var existed = piece.MarkerIds.Contains(markerId);

                    return new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.AddMarker,
                        PieceId = piece.Id,
                        MarkerId = markerId,
                        AddMarkerChangedState = !existed
                    };
                }

                case RemoveMarkerActionType:
                {
                    if (request.Payload is not RemoveMarkerPayload payload)
                    {
                        return null;
                    }

                    var piece = VttSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
                        ?? throw new InvalidOperationException("RemoveMarker target piece was not found for undo capture.");

                    var markerId = payload.MarkerId?.Trim() ?? string.Empty;
                    var existed = piece.MarkerIds.Contains(markerId);

                    return new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.RemoveMarker,
                        PieceId = piece.Id,
                        MarkerId = markerId,
                        RemoveMarkerChangedState = existed
                    };
                }

                case ChangePieceStateActionType:
                {
                    if (request.Payload is not ChangePieceStatePayload payload)
                    {
                        return null;
                    }

                    var piece = VttSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
                        ?? throw new InvalidOperationException("ChangePieceState target piece was not found for undo capture.");

                    var key = payload.Key?.Trim() ?? string.Empty;
                    var existed = piece.State.TryGetValue(key, out var previousValue);

                    return new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.ChangePieceState,
                        PieceId = piece.Id,
                        StateKey = key,
                        PreviousStateKeyExisted = existed,
                        PreviousStateValue = previousValue,
                        NewStateValue = payload.Value
                    };
                }

                default:
                    return null;
            }
        }

        private static void ApplyUndoEntry(VttSession VttSession, WorkspaceUndoEntry entry, bool undoing)
        {
            switch (entry.OperationKind)
            {
                case WorkspaceUndoOperationKind.MovePiece:
                {
                    var piece = RequirePiece(VttSession, entry.PieceId, entry.OperationKind);
                    piece.Location = undoing
                        ? CloneLocation(entry.FromLocation)
                        : CloneLocation(entry.ToLocation);
                    return;
                }

                case WorkspaceUndoOperationKind.RotatePiece:
                {
                    var piece = RequirePiece(VttSession, entry.PieceId, entry.OperationKind);
                    piece.Rotation = undoing
                        ? CloneRotation(entry.FromRotation)
                        : CloneRotation(entry.ToRotation);
                    return;
                }

                case WorkspaceUndoOperationKind.AddMarker:
                {
                    if (!entry.AddMarkerChangedState)
                    {
                        return;
                    }

                    var piece = RequirePiece(VttSession, entry.PieceId, entry.OperationKind);
                    if (undoing)
                    {
                        piece.MarkerIds.Remove(entry.MarkerId);
                    }
                    else if (!piece.MarkerIds.Contains(entry.MarkerId))
                    {
                        piece.MarkerIds.Add(entry.MarkerId);
                    }

                    return;
                }

                case WorkspaceUndoOperationKind.RemoveMarker:
                {
                    if (!entry.RemoveMarkerChangedState)
                    {
                        return;
                    }

                    var piece = RequirePiece(VttSession, entry.PieceId, entry.OperationKind);
                    if (undoing)
                    {
                        if (!piece.MarkerIds.Contains(entry.MarkerId))
                        {
                            piece.MarkerIds.Add(entry.MarkerId);
                        }
                    }
                    else
                    {
                        piece.MarkerIds.Remove(entry.MarkerId);
                    }

                    return;
                }

                case WorkspaceUndoOperationKind.ChangePieceState:
                {
                    var piece = RequirePiece(VttSession, entry.PieceId, entry.OperationKind);
                    if (undoing)
                    {
                        if (entry.PreviousStateKeyExisted)
                        {
                            piece.State[entry.StateKey] = entry.PreviousStateValue ?? string.Empty;
                        }
                        else
                        {
                            piece.State.Remove(entry.StateKey);
                        }
                    }
                    else
                    {
                        piece.State[entry.StateKey] = entry.NewStateValue ?? string.Empty;
                    }

                    return;
                }

                case WorkspaceUndoOperationKind.AddPiece:
                {
                    if (entry.PieceSnapshot is null)
                    {
                        throw new InvalidOperationException("AddPiece undo entry is missing piece snapshot.");
                    }

                    if (undoing)
                    {
                        var removed = VttSession.Pieces.RemoveAll(p => p.Id == entry.PieceSnapshot.Id);
                        if (removed == 0)
                        {
                            throw new InvalidOperationException("Undo AddPiece failed because piece was not found.");
                        }
                    }
                    else
                    {
                        if (VttSession.Pieces.Any(p => p.Id == entry.PieceSnapshot.Id))
                        {
                            throw new InvalidOperationException("Redo AddPiece failed because piece id already exists.");
                        }

                        VttSession.Pieces.Add(ClonePiece(entry.PieceSnapshot));
                    }

                    return;
                }

                case WorkspaceUndoOperationKind.AddSurface:
                {
                    if (entry.SurfaceSnapshot is null)
                    {
                        throw new InvalidOperationException("AddSurface undo entry is missing surface snapshot.");
                    }

                    if (undoing)
                    {
                        if (VttSession.Pieces.Any(p => p.Location.SurfaceId == entry.SurfaceSnapshot.Id))
                        {
                            throw new InvalidOperationException("Undo AddSurface failed because pieces are still on that surface.");
                        }

                        var removed = VttSession.Surfaces.RemoveAll(s => s.Id == entry.SurfaceSnapshot.Id);
                        if (removed == 0)
                        {
                            throw new InvalidOperationException("Undo AddSurface failed because surface was not found.");
                        }
                    }
                    else
                    {
                        if (VttSession.Surfaces.Any(s => s.Id == entry.SurfaceSnapshot.Id))
                        {
                            throw new InvalidOperationException("Redo AddSurface failed because surface id already exists.");
                        }

                        VttSession.Surfaces.Add(CloneSurface(entry.SurfaceSnapshot));
                    }

                    return;
                }

                default:
                    return;
            }
        }

        private static PieceInstance RequirePiece(VttSession VttSession, string pieceId, WorkspaceUndoOperationKind operationKind)
        {
            var piece = VttSession.Pieces.FirstOrDefault(p => p.Id == pieceId);
            if (piece is null)
            {
                throw new InvalidOperationException($"{operationKind} undo/redo target piece was not found.");
            }

            return piece;
        }

        private static PieceInstance ClonePiece(PieceInstance piece)
        {
            return new PieceInstance
            {
                Id = piece.Id,
                DefinitionId = piece.DefinitionId,
                OwnerParticipantId = piece.OwnerParticipantId,
                Location = CloneLocation(piece.Location),
                Rotation = CloneRotation(piece.Rotation),
                MarkerIds = [.. piece.MarkerIds],
                State = piece.State.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.Ordinal)
            };
        }

        private static SurfaceInstance CloneSurface(SurfaceInstance surface)
        {
            return new SurfaceInstance
            {
                Id = surface.Id,
                DefinitionId = surface.DefinitionId,
                Type = surface.Type,
                CoordinateSystem = surface.CoordinateSystem,
                Transform = surface.Transform is null
                    ? new SurfaceTransform()
                    : new SurfaceTransform
                    {
                        OffsetX = surface.Transform.OffsetX,
                        OffsetY = surface.Transform.OffsetY,
                        Scale = surface.Transform.Scale
                    },
                Layers = [.. surface.Layers.Select(layer => new Layer
                {
                    Id = layer.Id,
                    Name = layer.Name
                })],
                Zones = [.. surface.Zones.Select(zone => new Zone
                {
                    Id = zone.Id,
                    Name = zone.Name
                })]
            };
        }

        private static Location CloneLocation(Location? location)
        {
            if (location is null)
            {
                throw new InvalidOperationException("Location snapshot is required for undo/redo.");
            }

            return new Location
            {
                SurfaceId = location.SurfaceId,
                Coordinate = CloneCoordinate(location.Coordinate),
                ZoneId = location.ZoneId,
                LayerId = location.LayerId
            };
        }

        private static Coordinate CloneCoordinate(Coordinate? coordinate)
        {
            if (coordinate is null)
            {
                throw new InvalidOperationException("Coordinate snapshot is required for undo/redo.");
            }

            return new Coordinate
            {
                X = coordinate.X,
                Y = coordinate.Y
            };
        }

        private static Rotation CloneRotation(Rotation? rotation)
        {
            if (rotation is null)
            {
                throw new InvalidOperationException("Rotation snapshot is required for undo/redo.");
            }

            return new Rotation
            {
                Degrees = rotation.Degrees
            };
        }

        private void RegisterUndoEntry(WorkspaceUndoEntry? undoEntry)
        {
            if (undoEntry is null)
            {
                return;
            }

            State.UndoStack.Add(undoEntry);
            State.RedoStack.Clear();
        }

        private void ClearUndoRedoHistory()
        {
            State.UndoStack.Clear();
            State.RedoStack.Clear();
        }

        private static Scenario BuildScenarioExportFromCurrentSession(VttSession session, string scenarioTitle)
        {
            return new Scenario
            {
                Title = scenarioTitle,
                Surfaces = [.. session.Surfaces.Select(CloneSurface)],
                Pieces = [.. session.Pieces.Select(ClonePiece)],
                Options = CloneTabletopOptions(session.Options)
            };
        }

        private static TabletopOptions CloneTabletopOptions(TabletopOptions options)
        {
            return new TabletopOptions
            {
                EnableFog = options.EnableFog,
                EnableTurnTracker = options.EnableTurnTracker,
                Options = options.Options.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.Ordinal)
            };
        }

        private void ExecuteWorkspaceOperation(
            WorkspaceOperationKind operationKind,
            string? filePath,
            SnapshotFormatKind? snapshotFormat,
            string successMessage,
            Action action)
        {
            try
            {
                action();
                State.LastOperationMessage = successMessage;
                State.LastOperationSeverity = WorkspaceMessageSeverity.Success;
                RecordSuccessOperation(operationKind, filePath, snapshotFormat, successMessage);
            }
            catch (Exception ex)
            {
                State.LastOperationMessage = ex.Message;
                State.LastOperationSeverity = WorkspaceMessageSeverity.Error;
                RecordFailureOperation(operationKind, filePath, snapshotFormat, ex.Message);
                throw;
            }
        }

        private void RecordSuccessOperation(
            WorkspaceOperationKind operationKind,
            string? filePath,
            SnapshotFormatKind? snapshotFormat,
            string message)
        {
            State.LastOperationMessage = message;
            State.LastOperationSeverity = WorkspaceMessageSeverity.Success;

            State.OperationHistory.Add(new WorkspaceOperationEntry
            {
                OperationKind = operationKind,
                TimestampUtc = DateTime.UtcNow,
                Success = true,
                FilePath = filePath,
                Message = message,
                SnapshotFormat = snapshotFormat
            });
        }

        private void RecordFailureOperation(
            WorkspaceOperationKind operationKind,
            string? filePath,
            SnapshotFormatKind? snapshotFormat,
            string message)
        {
            State.LastOperationMessage = message;
            State.LastOperationSeverity = WorkspaceMessageSeverity.Error;

            State.OperationHistory.Add(new WorkspaceOperationEntry
            {
                OperationKind = operationKind,
                TimestampUtc = DateTime.UtcNow,
                Success = false,
                FilePath = filePath,
                Message = message,
                SnapshotFormat = snapshotFormat
            });
        }

        private static void ApplyDiagnostics(WorkspaceRecoveryDiagnostics target, WorkspaceRecoveryDiagnostics source)
        {
            target.SourceUsed = source.SourceUsed;
            target.MainFileAttempted = source.MainFileAttempted;
            target.BackupAttempted = source.BackupAttempted;
            target.MainFileValid = source.MainFileValid;
            target.BackupValid = source.BackupValid;

            foreach (var warning in source.Warnings)
            {
                target.Warnings.Add(warning);
            }

            foreach (var error in source.Errors)
            {
                target.Errors.Add(error);
            }
        }
    }
}
