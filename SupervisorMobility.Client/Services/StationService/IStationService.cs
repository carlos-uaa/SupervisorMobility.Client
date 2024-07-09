namespace SupervisorMobility.Client.Services.StationService
{
    public interface IStationService
    {
        Task<List<Station>> GetStations();

        Task<Station> GetStationById(int id);

        Task<Station> CreateStation(Station Station);

        Task<bool> UpdateStation(Station Station);

        Task DeleteStation(int id);
    }
}
