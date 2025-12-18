
namespace Service_Layer.IServices
{
    public interface IStationService
    {
        List<StationDTO> GetStations();
        ResponseModel<StationDTO> GetStationByName(string name);
        ResponseModel<List<StationDTO>> GetStationsByArea(string area);
        ResponseModel<StationDTO> GetTheNearestStation(LocationDTO myLocation);
        ResponseModel<StationDTO> AddStation(StationDTO station);
        ResponseModel<StationDTO> UpdateStation(int stationId,StationDTO station);
        ResponseModel<StationDTO> DeleteStation(int stationid);
    }
}
