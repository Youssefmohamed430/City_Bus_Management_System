using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using Data_Access_Layer.DataLayer.DTOs;

namespace Service_Layer.IServices
{
    public interface IStationService
    {
        ResponseModel<List<StationDTO>> GetStations();
        ResponseModel<StationDTO> GetStationByName(string name);
        ResponseModel<List<StationDTO>> GetStationsByArea(string area);
        ResponseModel<StationDTO> GetTheNearestStation(LocationDTO myLocation);
        ResponseModel<StationDTO> AddStation(StationDTO station);
        ResponseModel<StationDTO> UpdateStation(int stationId,StationDTO station);
        ResponseModel<StationDTO> DeleteStation(int stationid);
    }
}
