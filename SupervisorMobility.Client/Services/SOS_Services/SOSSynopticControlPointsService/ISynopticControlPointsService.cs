namespace SupervisorMobility.Client.Services.SOS_Services.SOSSynopticControlPointsService
{
    public interface ISynopticControlPointsService
    {
        Task<SOSSynopticTableofControlPoints> GetSOSSynopticTableofControlPoints(int SOSSynopticTableofControlPointsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);

        Task<List<SOSSynopticTableofControlPoints>> GetAllSOSSynopticTableofControlPoints(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);

        Task<SOSSynopticTableofControlPoints> UpdateSOSSynopticTableofControlPoints(SOSSynopticTableofControlPoints SosEntity);

        Task<bool> DeleteSOSSynopticTableofControlPoints(int SosEntity_id);

        Task<FileUpload> AddIllustrationToSOSSynopticTableofControlPoints(MultipartFormDataContent? contentfiles, int SOS_SOSSynopticTableofControlPoints_id);

        Task<string> ShowIlustrationSOSSynopticTableofControlPoints(int idfile);

        Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSSynopticTableofControlPoints_id, int ImageFile_id);
    }
}
