using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.Workspace;
using System.Text;
using System.Text.Json;

namespace MIMLESvtt.src.Domain.Persistence.VttSessionNSPC
{
    public class VttSessionWorkspaceStatePersistenceService
    {
        private const int MaxHistoryEntries = 10;
        private static readonly JsonSerializerOptions SerializerOptions = new();

        private WorkspaceRecoveryDiagnostics? _lastRecoveryDiagnostics;

        public WorkspaceRecoveryDiagnostics? LastRecoveryDiagnostics => _lastRecoveryDiagnostics;

        public void SaveWorkspaceState(VttSessionWorkspaceState state, string path)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var recoveryState = new VttSessionWorkspaceRecoveryState
            {
                CurrentFilePath = state.CurrentFilePath,
                CurrentSnapshotFormat = state.CurrentSnapshotFormat,
                PendingVttScenarioSourcePath = state.PendingVttScenarioSourcePath,
                RecentOperationHistory = state.OperationHistory
                    .TakeLast(MaxHistoryEntries)
                    .Select(e => new WorkspaceOperationEntry
                    {
                        OperationKind = e.OperationKind,
                        TimestampUtc = e.TimestampUtc,
                        Success = e.Success,
                        FilePath = e.FilePath,
                        Message = e.Message,
                        SnapshotFormat = e.SnapshotFormat
                    })
                    .ToList()
            };

            var json = JsonSerializer.Serialize(recoveryState, SerializerOptions);

            var fullPath = Path.GetFullPath(path);
            var tempPath = GetTempPath(fullPath);
            var backupPath = GetBackupPath(fullPath);

            File.WriteAllText(tempPath, json, Encoding.UTF8);

            if (File.Exists(fullPath))
            {
                File.Replace(tempPath, fullPath, backupPath, ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempPath, fullPath);
            }

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }

        public VttSessionWorkspaceRecoveryState LoadWorkspaceState(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var fullPath = Path.GetFullPath(path);

            var diagnostics = new WorkspaceRecoveryDiagnostics
            {
                MainFileAttempted = true
            };

            _lastRecoveryDiagnostics = diagnostics;

            if (!File.Exists(fullPath))
            {
                diagnostics.Errors.Add("Workspace state file was not found.");
                throw new FileNotFoundException("Workspace state file was not found.", fullPath);
            }

            try
            {
                var recoveryState = LoadWorkspaceStateCore(fullPath);
                diagnostics.MainFileValid = true;
                diagnostics.SourceUsed = WorkspaceRecoverySource.Main;
                diagnostics.IsSuccess = true;
                return recoveryState;
            }
            catch (InvalidOperationException mainException)
            {
                diagnostics.MainFileValid = false;
                diagnostics.Errors.Add($"Main workspace state file invalid: {mainException.Message}");

                var backupPath = GetBackupPath(fullPath);
                if (!File.Exists(backupPath))
                {
                    diagnostics.IsSuccess = false;
                    diagnostics.SourceUsed = WorkspaceRecoverySource.None;
                    throw;
                }

                diagnostics.BackupAttempted = true;

                try
                {
                    var backupState = LoadWorkspaceStateCore(backupPath);
                    diagnostics.BackupValid = true;
                    diagnostics.SourceUsed = WorkspaceRecoverySource.Backup;
                    diagnostics.IsSuccess = true;
                    diagnostics.Warnings.Add("Main workspace state file was invalid. Restored from backup.");
                    return backupState;
                }
                catch (Exception backupException)
                {
                    diagnostics.BackupValid = false;
                    diagnostics.SourceUsed = WorkspaceRecoverySource.None;
                    diagnostics.IsSuccess = false;
                    diagnostics.Errors.Add($"Backup workspace state file invalid: {backupException.Message}");
                    throw new InvalidOperationException("Workspace state and backup files are invalid and could not be loaded.", new AggregateException(mainException, backupException));
                }
            }
        }

        private static VttSessionWorkspaceRecoveryState LoadWorkspaceStateCore(string path)
        {
            var json = File.ReadAllText(path, Encoding.UTF8);

            try
            {
                var recoveryState = JsonSerializer.Deserialize<VttSessionWorkspaceRecoveryState>(json, SerializerOptions);
                if (recoveryState is null)
                {
                    throw new InvalidOperationException("Failed to deserialize workspace state document.");
                }

                ValidateRecoveryState(recoveryState);
                return recoveryState;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Workspace state file contains malformed JSON.", ex);
            }
        }

        private static string GetBackupPath(string fullPath)
        {
            return fullPath + ".bak";
        }

        private static string GetTempPath(string fullPath)
        {
            return fullPath + ".tmp";
        }

        private static void ValidateRecoveryState(VttSessionWorkspaceRecoveryState recoveryState)
        {
            if (recoveryState.CurrentFilePath is not null
                && !recoveryState.CurrentFilePath.EndsWith(SnapshotFileExtensions.VttSession, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Workspace state CurrentFilePath must use a VTT session snapshot extension.");
            }

            if (recoveryState.PendingVttScenarioSourcePath is not null
                && !recoveryState.PendingVttScenarioSourcePath.EndsWith(SnapshotFileExtensions.VttScenario, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Workspace state PendingVttScenarioSourcePath must use a scenario snapshot extension.");
            }

            if (recoveryState.CurrentSnapshotFormat is not null
                && recoveryState.CurrentSnapshotFormat != SnapshotFormatKind.VttSessionSnapshot
                && recoveryState.CurrentSnapshotFormat != SnapshotFormatKind.VttScenarioSnapshot)
            {
                throw new InvalidOperationException("Workspace state CurrentSnapshotFormat is unsupported.");
            }
        }
    }
}
