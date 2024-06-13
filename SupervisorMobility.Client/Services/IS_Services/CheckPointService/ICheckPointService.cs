using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Services.IS_Services.CheckpointService
{
    public interface ICheckPointService
    {
        
        #region Checkpoint
        Task<Checkpoint> CreateCheckpoint(Checkpoint CheckpointtoCreate);
        Task<Checkpoint> GetCheckpoint(int id_Checkpoint, bool includeStandars = false, bool includeSketches = false);
        Task<List<Checkpoint>> GetAllCheckpoints(bool includeStandars = false, bool includeSketches = false);

        Task<Checkpoint?> UpdateCheckpoint(Checkpoint CheckpointtoUpdate);
        Task<Checkpoint> DeleteCheckpoint(int id);

        Task<bool> UpdatePanelSequence(int Checkpoint_Id, Checkpoint Checkpoint);

        #endregion

        #region CheckpointNorm
        Task<CheckpointNorm> CreatecheckpointNorm(CheckpointNorm checkpointNorm);
        Task<CheckpointNorm> GetCheckpointNorm(int id_CheckpointNorm, bool includeSketches = false);
        //Task<bool> UpdatecheckpointNormSequence(int Checkpoint_Id, CheckpointNorm datacheckpointNorm);
        Task<CheckpointNorm> DeleteCheckpointNorm(int Checkpoint_Id);
        #endregion

        Task<FileUpload> UploadSketchCheckpoint(MultipartFormDataContent? contentfiles, int Checkpoint_id);
        Task<FileUpload> UploadSketchCheckpointNorm(MultipartFormDataContent? contentfiles, int CheckpointNorm_id);
        Task<string> ShowImageCheckpoint(int idfile);
        Task<string> ShowImageCheckpointNorm(int idfile);
        Task<bool> RemoveSketchCheckPoint(int CheckpointId, int fileUploadId);
        Task<bool> RemoveSketchCheckPointNorm(int Checkpoint_NormId, int fileUploadId);
    }
}
