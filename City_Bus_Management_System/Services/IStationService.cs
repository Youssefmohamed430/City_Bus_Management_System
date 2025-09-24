using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace City_Bus_Management_System.Services
{
    public interface IStationService
    {
        ResponseModel<List<StationDTO>> GetStations();
        ResponseModel<StationDTO> GetStationByName(string name);
        ResponseModel<StationDTO> GetStationByArea(string area);
        ResponseModel<List<StationDTO>> GetTheNearestStation(decimal longitude,decimal latitude);
        ResponseModel<StationDTO> AddStation(StationDTO station);
        ResponseModel<StationDTO> UpdateStation(int stationId,StationDTO station);
        ResponseModel<StationDTO> DeleteStation(int stationid);
    }
}
