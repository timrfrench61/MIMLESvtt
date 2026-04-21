namespace MIMLESvtt.src
{
    public class SessionWorkspaceService
    {
        private readonly SnapshotFileWorkflowService _snapshotFileWorkflowService;
        private readonly SnapshotFileImportApplyWorkflowService _snapshotFileImportApplyWorkflowService;
        private readonly ScenarioPlanApplyService _scenarioPlanApplyService;
        private readonly ScenarioCandidateActivationService _scenarioCandidateActivationService;
        private readonly SnapshotFileLibraryService _snapshotFileLibraryService;
        private readonly SessionWorkspaceStatePersistenceService _workspaceStatePersistenceService;
        private readonly ActionProcessor _actionProcessor;

        public WorkspaceRecoveryDiagnostics? LastRestoreDiagnostics { get; private set; }

        public SessionWorkspaceService()
            : this(
                new SnapshotFileWorkflowService(),
                new SnapshotFileImportApplyWorkflowService(),
                new ScenarioPlanApplyService(),
                new ScenarioCandidateActivationService(),
                new SnapshotFileLibraryService(),
                new SessionWorkspaceStatePersistenceService(),
                new ActionProcessor())
        {
        }

        public SessionWorkspaceService(
            SnapshotFileWorkflowService snapshotFileWorkflowService,
            SnapshotFileImportApplyWorkflowService snapshotFileImportApplyWorkflowService,
            ScenarioPlanApplyService scenarioPlanApplyService,
            ScenarioCandidateActivationService scenarioCandidateActivationService,
            SnapshotFileLibraryService snapshotFileLibraryService,
            SessionWorkspaceStatePersistenceService workspaceStatePersistenceService,
            ActionProcessor actionProcessor)
        {
            _snapshotFileWorkflowService = snapshotFileWorkflowService ?? throw new ArgumentNullException(nameof(snapshotFileWorkflowService));
            _snapshotFileImportApplyWorkflowService = snapshotFileImportApplyWorkflowService ?? throw new ArgumentNullException(nameof(snapshotFileImportApplyWorkflowService));
            _scenarioPlanApplyService = scenarioPlanApplyService ?? throw new ArgumentNullException(nameof(scenarioPlanApplyService));
            _scenarioCandidateActivationService = scenarioCandidateActivationService ?? throw new ArgumentNullException(nameof(scenarioCandidateActivationService));
            _snapshotFileLibraryService = snapshotFileLibraryService ?? throw new ArgumentNullException(nameof(snapshotFileLibraryService));
            _workspaceStatePersistenceService = workspaceStatePersistenceService ?? throw new ArgumentNullException(nameof(workspaceStatePersistenceService));
            _actionProcessor = actionProcessor ?? throw new ArgumentNullException(nameof(actionProcessor));

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

                    State.CurrentTableSession.Pieces.Add(new PieceInstance
                    {
                        Id = pieceId,
                        DefinitionId = definitionId,
                        OwnerParticipantId = ownerParticipantId ?? string.Empty,
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
                var actionRecord = _actionProcessor.Process(State.CurrentTableSession, request);
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
