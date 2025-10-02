using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace Service_Layer.IServices
{
    public interface IStationService
    {
        ResponseModel<List<StationDTO>> GetStations();
        ResponseModel<StationDTO> GetStationByName(string name);
        ResponseModel<List<StationDTO>> GetStationsByArea(string area);
        Task<ResponseModel<StationDTO>> GetTheNearestStation(string are);
        ResponseModel<StationDTO> AddStation(StationDTO station);
        ResponseModel<StationDTO> UpdateStation(int stationId,StationDTO station);
        ResponseModel<StationDTO> DeleteStation(int stationid);
    }
}
