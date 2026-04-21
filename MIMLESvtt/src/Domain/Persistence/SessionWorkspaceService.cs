namespace MIMLESvtt.src
{
    public class SessionWorkspaceService
    {
        private const string MovePieceActionType = "MovePiece";
        private const string RotatePieceActionType = "RotatePiece";
        private const string AddMarkerActionType = "AddMarker";
        private const string RemoveMarkerActionType = "RemoveMarker";
        private const string ChangePieceStateActionType = "ChangePieceState";

        private readonly SnapshotFileWorkflowService _snapshotFileWorkflowService;
        private readonly SnapshotFileImportApplyWorkflowService _snapshotFileImportApplyWorkflowService;
        private readonly ScenarioPlanApplyService _scenarioPlanApplyService;
        private readonly ScenarioCandidateActivationService _scenarioCandidateActivationService;
        private readonly SnapshotFileLibraryService _snapshotFileLibraryService;
        private readonly SessionWorkspaceStatePersistenceService _workspaceStatePersistenceService;
        private readonly ActionProcessor _actionProcessor;
        private readonly ActionValidationService _actionValidationService;

        public WorkspaceRecoveryDiagnostics? LastRestoreDiagnostics { get; private set; }

        public SessionWorkspaceService()
            : this(
                new SnapshotFileWorkflowService(),
                new SnapshotFileImportApplyWorkflowService(),
                new ScenarioPlanApplyService(),
                new ScenarioCandidateActivationService(),
                new SnapshotFileLibraryService(),
                new SessionWorkspaceStatePersistenceService(),
                new ActionProcessor(),
                new ActionValidationService())
        {
        }

        public SessionWorkspaceService(
            SnapshotFileWorkflowService snapshotFileWorkflowService,
            SnapshotFileImportApplyWorkflowService snapshotFileImportApplyWorkflowService,
            ScenarioPlanApplyService scenarioPlanApplyService,
            ScenarioCandidateActivationService scenarioCandidateActivationService,
            SnapshotFileLibraryService snapshotFileLibraryService,
            SessionWorkspaceStatePersistenceService workspaceStatePersistenceService,
            ActionProcessor actionProcessor,
            ActionValidationService actionValidationService)
        {
            _snapshotFileWorkflowService = snapshotFileWorkflowService ?? throw new ArgumentNullException(nameof(snapshotFileWorkflowService));
            _snapshotFileImportApplyWorkflowService = snapshotFileImportApplyWorkflowService ?? throw new ArgumentNullException(nameof(snapshotFileImportApplyWorkflowService));
            _scenarioPlanApplyService = scenarioPlanApplyService ?? throw new ArgumentNullException(nameof(scenarioPlanApplyService));
            _scenarioCandidateActivationService = scenarioCandidateActivationService ?? throw new ArgumentNullException(nameof(scenarioCandidateActivationService));
            _snapshotFileLibraryService = snapshotFileLibraryService ?? throw new ArgumentNullException(nameof(snapshotFileLibraryService));
            _workspaceStatePersistenceService = workspaceStatePersistenceService ?? throw new ArgumentNullException(nameof(workspaceStatePersistenceService));
            _actionProcessor = actionProcessor ?? throw new ArgumentNullException(nameof(actionProcessor));
            _actionValidationService = actionValidationService ?? throw new ArgumentNullException(nameof(actionValidationService));

            State = new SessionWorkspaceState();
        }

        public SessionWorkspaceState State { get; }

        public void CreateNewSession()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.CreateNewSession,
                filePath: null,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Created new workspace session.",
                action: () =>
                {
                    State.CurrentTableSession = new TableSession
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = "New Session"
                    };
                    State.CurrentFilePath = null;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot;
                    State.IsDirty = false;
                    State.CurrentPendingScenarioPlan = null;
                    State.PendingScenarioSourcePath = null;
                    ClearUndoRedoHistory();
                });
        }

        public void SaveCurrentLayoutAsScenario(string scenarioTitle, string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveCurrentLayoutAsScenario,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.ScenarioSnapshot,
                successMessage: "Saved current layout as scenario.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to save scenario layout.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(scenarioTitle);
                    ArgumentException.ThrowIfNullOrWhiteSpace(path);

                    var scenario = BuildScenarioExportFromCurrentSession(State.CurrentTableSession, scenarioTitle.Trim());
                    _snapshotFileWorkflowService.SaveScenario(scenario, path);

                    var fullPath = Path.GetFullPath(path);
                    _snapshotFileLibraryService.AddPath(fullPath);
                    _snapshotFileLibraryService.RefreshEntry(fullPath);
                });
        }

        public void InitializeTurnOrder(List<string> turnOrder)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.InitializeTurnOrder,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Initialized turn order.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to initialize turn order.");
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

                    State.CurrentTableSession.TurnOrder = normalizedOrder;
                    State.CurrentTableSession.CurrentTurnIndex = 0;
                    State.IsDirty = true;
                });
        }

        public void AdvanceTurn()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AdvanceTurn,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Advanced to next turn.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to advance turn.");
                    }

                    if (State.CurrentTableSession.TurnOrder.Count == 0)
                    {
                        throw new InvalidOperationException("Turn order is required before advancing turn.");
                    }

                    var nextIndex = State.CurrentTableSession.CurrentTurnIndex + 1;
                    State.CurrentTableSession.CurrentTurnIndex = nextIndex % State.CurrentTableSession.TurnOrder.Count;
                    State.IsDirty = true;
                });
        }

        public void SetPhase(string phase)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SetPhase,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Updated current phase.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to set phase.");
                    }

                    State.CurrentTableSession.CurrentPhase = phase?.Trim() ?? string.Empty;
                    State.IsDirty = true;
                });
        }

        public void AddParticipant(string id, string name)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.AddParticipant,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Added participant.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to add participant.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(id);
                    ArgumentException.ThrowIfNullOrWhiteSpace(name);

                    var normalizedId = id.Trim();
                    var normalizedName = name.Trim();

                    if (State.CurrentTableSession.Participants.Any(p => string.Equals(p.Id, normalizedId, StringComparison.Ordinal)))
                    {
                        throw new InvalidOperationException("Participant id already exists in current session.");
                    }

                    State.CurrentTableSession.Participants.Add(new Participant
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
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Removed participant.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to remove participant.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(id);
                    var normalizedId = id.Trim();

                    var participant = State.CurrentTableSession.Participants.FirstOrDefault(p => string.Equals(p.Id, normalizedId, StringComparison.Ordinal));
                    if (participant is null)
                    {
                        throw new InvalidOperationException("Participant id was not found in current session.");
                    }

                    State.CurrentTableSession.Participants.Remove(participant);

                    foreach (var piece in State.CurrentTableSession.Pieces)
                    {
                        if (string.Equals(piece.OwnerParticipantId, normalizedId, StringComparison.Ordinal))
                        {
                            piece.OwnerParticipantId = string.Empty;
                        }
                    }

                    State.CurrentTableSession.TurnOrder.RemoveAll(participantId => string.Equals(participantId, normalizedId, StringComparison.Ordinal));

                    if (State.CurrentTableSession.TurnOrder.Count == 0)
                    {
                        State.CurrentTableSession.CurrentTurnIndex = 0;
                    }
                    else if (State.CurrentTableSession.CurrentTurnIndex >= State.CurrentTableSession.TurnOrder.Count)
                    {
                        State.CurrentTableSession.CurrentTurnIndex = 0;
                    }

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
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Added surface to current session.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to add a surface.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);
                    ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);

                    if (State.CurrentTableSession.Surfaces.Any(s => s.Id == surfaceId))
                    {
                        throw new InvalidOperationException("Surface id already exists in current session.");
                    }

                    State.CurrentTableSession.Surfaces.Add(new SurfaceInstance
                    {
                        Id = surfaceId,
                        DefinitionId = definitionId,
                        Type = surfaceType,
                        CoordinateSystem = coordinateSystem
                    });

                    RegisterUndoEntry(new WorkspaceUndoEntry
                    {
                        OperationKind = WorkspaceUndoOperationKind.AddSurface,
                        SurfaceSnapshot = CloneSurface(State.CurrentTableSession.Surfaces.Last())
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
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Added piece to current session.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to add a piece.");
                    }

                    ArgumentException.ThrowIfNullOrWhiteSpace(pieceId);
                    ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);
                    ArgumentException.ThrowIfNullOrWhiteSpace(surfaceId);

                    if (!State.CurrentTableSession.Surfaces.Any(s => s.Id == surfaceId))
                    {
                        throw new InvalidOperationException("Target surface was not found in current session.");
                    }

                    if (State.CurrentTableSession.Pieces.Any(p => p.Id == pieceId))
                    {
                        throw new InvalidOperationException("Piece id already exists in current session.");
                    }

                    var normalizedOwnerParticipantId = ownerParticipantId?.Trim() ?? string.Empty;

                    State.CurrentTableSession.Pieces.Add(new PieceInstance
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
                        PieceSnapshot = ClonePiece(State.CurrentTableSession.Pieces.Last())
                    });

                    State.IsDirty = true;
                });
        }

        public void OpenTableSessionFromFile(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.OpenTableSessionFromFile,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Opened table session from file.",
                action: () =>
                {
                    var loadedSession = _snapshotFileWorkflowService.LoadTableSession(path);
                    var fullPath = Path.GetFullPath(path);

                    State.CurrentTableSession = loadedSession;
                    State.CurrentFilePath = fullPath;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot;
                    State.IsDirty = false;
                    State.CurrentPendingScenarioPlan = null;
                    State.PendingScenarioSourcePath = null;
                    ClearUndoRedoHistory();

                    _snapshotFileLibraryService.AddPath(fullPath);
                    _snapshotFileLibraryService.RefreshEntry(fullPath);
                });
        }

        public void SaveCurrentSession()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveCurrentSession,
                filePath: State.CurrentFilePath,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Saved current session to file.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to save.");
                    }

                    if (string.IsNullOrWhiteSpace(State.CurrentFilePath))
                    {
                        throw new InvalidOperationException("CurrentFilePath is required to save current session.");
                    }

                    _snapshotFileWorkflowService.SaveTableSession(State.CurrentTableSession, State.CurrentFilePath);
                    State.IsDirty = false;

                    _snapshotFileLibraryService.AddPath(State.CurrentFilePath);
                    _snapshotFileLibraryService.RefreshEntry(State.CurrentFilePath);
                });
        }

        public void SaveCurrentSessionAs(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.SaveCurrentSessionAs,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.TableSessionSnapshot,
                successMessage: "Saved current session as a new file.",
                action: () =>
                {
                    if (State.CurrentTableSession is null)
                    {
                        throw new InvalidOperationException("CurrentTableSession is required to save.");
                    }

                    _snapshotFileWorkflowService.SaveTableSession(State.CurrentTableSession, path);

                    var fullPath = Path.GetFullPath(path);
                    State.CurrentFilePath = fullPath;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot;
                    State.IsDirty = false;

                    _snapshotFileLibraryService.AddPath(fullPath);
                    _snapshotFileLibraryService.RefreshEntry(fullPath);
                });
        }

        public void ImportScenarioToPendingPlanFromFile(string path)
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.ImportScenarioToPendingPlanFromFile,
                filePath: path,
                snapshotFormat: SnapshotFormatKind.ScenarioSnapshot,
                successMessage: "Imported scenario to pending plan.",
                action: () =>
                {
                    var context = new SnapshotImportApplyContext
                    {
                        CurrentTableSession = State.CurrentTableSession
                    };

                    var response = _snapshotFileImportApplyWorkflowService.ImportAndApplyFromFile(
                        path,
                        context,
                        ScenarioCandidateActivationMode.DryRun,
                        SnapshotImportApplyPolicy.Default);

                    if (!response.IsSuccess)
                    {
                        throw new InvalidOperationException(response.ErrorMessage ?? "Scenario file import failed.");
                    }

                    if (response.DetectedFormat != SnapshotFormatKind.ScenarioSnapshot)
                    {
                        throw new InvalidOperationException("Scenario file import did not produce ScenarioSnapshot format.");
                    }

                    var pendingPlan = response.ScenarioActivationResponse?.PendingScenarioPlan;
                    if (pendingPlan is null)
                    {
                        throw new InvalidOperationException("Scenario import did not produce a pending scenario plan.");
                    }

                    State.CurrentPendingScenarioPlan = pendingPlan;
                    State.PendingScenarioSourcePath = Path.GetFullPath(path);
                });
        }

        public void ActivatePendingScenario()
        {
            ExecuteWorkspaceOperation(
                WorkspaceOperationKind.ActivatePendingScenario,
                filePath: null,
                snapshotFormat: SnapshotFormatKind.ScenarioSnapshot,
                successMessage: "Activated pending scenario into current session.",
                action: () =>
                {
                    if (State.CurrentPendingScenarioPlan is null)
                    {
                        throw new InvalidOperationException("Pending scenario plan is required for activation.");
                    }

                    var candidateResult = _scenarioPlanApplyService.Apply(new ScenarioPlanApplyRequest
                    {
                        PendingScenarioPlan = State.CurrentPendingScenarioPlan,
                        ActiveRuntimeTableSession = State.CurrentTableSession
                    });

                    if (!candidateResult.IsSuccess || candidateResult.TableSessionCandidate is null)
                    {
                        throw new InvalidOperationException(candidateResult.ErrorMessage ?? "Failed to create table session candidate from pending scenario plan.");
                    }

                    var activationContext = new SnapshotImportApplyContext
                    {
                        CurrentTableSession = State.CurrentTableSession
                    };

                    var activationResult = _scenarioCandidateActivationService.Activate(new ScenarioCandidateActivationRequest
                    {
                        TableSessionCandidate = candidateResult.TableSessionCandidate,
                        TargetContext = activationContext,
                        Mode = ScenarioCandidateActivationMode.Activate
                    }, SnapshotImportApplyPolicy.Default);

                    if (!activationResult.IsSuccess)
                    {
                        throw new InvalidOperationException(activationResult.Message ?? activationResult.ErrorMessage ?? "Failed to activate pending scenario.");
                    }

                    State.CurrentTableSession = activationContext.CurrentTableSession;
                    State.CurrentFilePath = null;
                    State.CurrentSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot;
                    State.CurrentPendingScenarioPlan = null;
                    State.PendingScenarioSourcePath = null;
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

                        var workingSession = State.CurrentTableSession;
                        var workingCurrentFilePath = State.CurrentFilePath;
                        var workingSnapshotFormat = State.CurrentSnapshotFormat;
                        var workingPendingPlan = State.CurrentPendingScenarioPlan;
                        var workingPendingSourcePath = State.PendingScenarioSourcePath;

                        if (!string.IsNullOrWhiteSpace(recoveryState.CurrentFilePath))
                        {
                            if (!File.Exists(recoveryState.CurrentFilePath))
                            {
                                diagnostics.Errors.Add("Workspace recovery failed: referenced current session file was not found.");
                                throw new InvalidOperationException("Workspace recovery failed: referenced current session file was not found.");
                            }

                            var session = _snapshotFileWorkflowService.LoadTableSession(recoveryState.CurrentFilePath);
                            var fullSessionPath = Path.GetFullPath(recoveryState.CurrentFilePath);

                            workingSession = session;
                            workingCurrentFilePath = fullSessionPath;
                            workingSnapshotFormat = SnapshotFormatKind.TableSessionSnapshot;
                            diagnostics.CurrentSessionRestored = true;

                            _snapshotFileLibraryService.AddPath(fullSessionPath);
                            _snapshotFileLibraryService.RefreshEntry(fullSessionPath);
                        }

                        if (!string.IsNullOrWhiteSpace(recoveryState.PendingScenarioSourcePath))
                        {
                            if (!File.Exists(recoveryState.PendingScenarioSourcePath))
                            {
                                if (options.MissingPendingScenarioBehavior == MissingPendingScenarioMode.WarnAndContinue)
                                {
                                    diagnostics.Warnings.Add("Pending scenario file was not found during restore.");
                                    diagnostics.PendingScenarioRestored = false;
                                    workingPendingPlan = null;
                                    workingPendingSourcePath = null;
                                }
                                else
                                {
                                    if (diagnostics.CurrentSessionRestored)
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
                                    CurrentTableSession = workingSession
                                };

                                var response = _snapshotFileImportApplyWorkflowService.ImportAndApplyFromFile(
                                    recoveryState.PendingScenarioSourcePath,
                                    importContext,
                                    ScenarioCandidateActivationMode.DryRun,
                                    SnapshotImportApplyPolicy.Default);

                                if (!response.IsSuccess)
                                {
                                    diagnostics.Errors.Add(response.ErrorMessage ?? "Workspace recovery failed while rebuilding pending scenario plan.");
                                    throw new InvalidOperationException(response.ErrorMessage ?? "Workspace recovery failed while rebuilding pending scenario plan.");
                                }

                                var pendingPlan = response.ScenarioActivationResponse?.PendingScenarioPlan;
                                if (pendingPlan is null)
                                {
                                    diagnostics.Errors.Add("Workspace recovery failed: pending scenario plan was not rebuilt.");
                                    throw new InvalidOperationException("Workspace recovery failed: pending scenario plan was not rebuilt.");
                                }

                                workingPendingPlan = pendingPlan;
                                workingPendingSourcePath = Path.GetFullPath(recoveryState.PendingScenarioSourcePath);
                                diagnostics.PendingScenarioRestored = true;
                            }
                        }

                        else
                        {
                            workingPendingPlan = null;
                            workingPendingSourcePath = null;
                        }

                        State.CurrentTableSession = workingSession;
                        State.CurrentFilePath = workingCurrentFilePath;
                        State.CurrentSnapshotFormat = workingSnapshotFormat;
                        State.CurrentPendingScenarioPlan = workingPendingPlan;
                        State.PendingScenarioSourcePath = workingPendingSourcePath;
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
            if (State.CurrentTableSession is null)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.ProcessAction,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Cannot process action without a current table session.");

                throw new InvalidOperationException("CurrentTableSession is required to process action.");
            }

            try
            {
                if (State.Mode == WorkspaceMode.Play)
                {
                    var validation = _actionValidationService.Validate(request, State.CurrentTableSession);
                    if (!validation.IsValid)
                    {
                        throw new InvalidOperationException(validation.Message);
                    }
                }

                var undoEntry = CreateUndoEntryForAction(State.CurrentTableSession, request);
                var actionRecord = _actionProcessor.Process(State.CurrentTableSession, request);

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
            if (State.CurrentTableSession is null)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.UndoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Cannot undo without a current table session.");

                throw new InvalidOperationException("CurrentTableSession is required to undo.");
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
                ApplyUndoEntry(State.CurrentTableSession, entry, undoing: true);
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
            if (State.CurrentTableSession is null)
            {
                RecordFailureOperation(
                    WorkspaceOperationKind.RedoLastOperation,
                    filePath: State.CurrentFilePath,
                    snapshotFormat: State.CurrentSnapshotFormat,
                    message: "Cannot redo without a current table session.");

                throw new InvalidOperationException("CurrentTableSession is required to redo.");
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
                ApplyUndoEntry(State.CurrentTableSession, entry, undoing: false);
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

        private WorkspaceUndoEntry? CreateUndoEntryForAction(TableSession tableSession, ActionRequest request)
        {
            switch (request.ActionType)
            {
                case MovePieceActionType:
                {
                    if (request.Payload is not MovePiecePayload payload)
                    {
                        return null;
                    }

                    var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
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

                    var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
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

                    var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
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

                    var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
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

                    var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == payload.PieceId)
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

        private static void ApplyUndoEntry(TableSession tableSession, WorkspaceUndoEntry entry, bool undoing)
        {
            switch (entry.OperationKind)
            {
                case WorkspaceUndoOperationKind.MovePiece:
                {
                    var piece = RequirePiece(tableSession, entry.PieceId, entry.OperationKind);
                    piece.Location = undoing
                        ? CloneLocation(entry.FromLocation)
                        : CloneLocation(entry.ToLocation);
                    return;
                }

                case WorkspaceUndoOperationKind.RotatePiece:
                {
                    var piece = RequirePiece(tableSession, entry.PieceId, entry.OperationKind);
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

                    var piece = RequirePiece(tableSession, entry.PieceId, entry.OperationKind);
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

                    var piece = RequirePiece(tableSession, entry.PieceId, entry.OperationKind);
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
                    var piece = RequirePiece(tableSession, entry.PieceId, entry.OperationKind);
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
                        var removed = tableSession.Pieces.RemoveAll(p => p.Id == entry.PieceSnapshot.Id);
                        if (removed == 0)
                        {
                            throw new InvalidOperationException("Undo AddPiece failed because piece was not found.");
                        }
                    }
                    else
                    {
                        if (tableSession.Pieces.Any(p => p.Id == entry.PieceSnapshot.Id))
                        {
                            throw new InvalidOperationException("Redo AddPiece failed because piece id already exists.");
                        }

                        tableSession.Pieces.Add(ClonePiece(entry.PieceSnapshot));
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
                        if (tableSession.Pieces.Any(p => p.Location.SurfaceId == entry.SurfaceSnapshot.Id))
                        {
                            throw new InvalidOperationException("Undo AddSurface failed because pieces are still on that surface.");
                        }

                        var removed = tableSession.Surfaces.RemoveAll(s => s.Id == entry.SurfaceSnapshot.Id);
                        if (removed == 0)
                        {
                            throw new InvalidOperationException("Undo AddSurface failed because surface was not found.");
                        }
                    }
                    else
                    {
                        if (tableSession.Surfaces.Any(s => s.Id == entry.SurfaceSnapshot.Id))
                        {
                            throw new InvalidOperationException("Redo AddSurface failed because surface id already exists.");
                        }

                        tableSession.Surfaces.Add(CloneSurface(entry.SurfaceSnapshot));
                    }

                    return;
                }

                default:
                    return;
            }
        }

        private static PieceInstance RequirePiece(TableSession tableSession, string pieceId, WorkspaceUndoOperationKind operationKind)
        {
            var piece = tableSession.Pieces.FirstOrDefault(p => p.Id == pieceId);
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

        private static ScenarioExport BuildScenarioExportFromCurrentSession(TableSession session, string scenarioTitle)
        {
            return new ScenarioExport
            {
                Title = scenarioTitle,
                Surfaces = [.. session.Surfaces.Select(CloneSurface)],
                Pieces = [.. session.Pieces.Select(ClonePiece)],
                Options = CloneTableOptions(session.Options)
            };
        }

        private static TableOptions CloneTableOptions(TableOptions options)
        {
            return new TableOptions
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
                RecordSuccessOperation(operationKind, filePath, snapshotFormat, successMessage);
            }
            catch (Exception ex)
            {
                State.LastOperationMessage = ex.Message;
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
