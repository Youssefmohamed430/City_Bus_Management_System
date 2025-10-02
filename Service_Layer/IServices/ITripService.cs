using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace Service_Layer.IServices
{
    public interface ITripService
    {
        ResponseModel<List<TripDTO>> GetAllTrips();
        ResponseModel<TripDTO> AddTrip(TripDTO tripDto);
        ResponseModel<TripDTO> UpdateTrip(int tripid , TripDTO tripDto);
        ResponseModel<TripDTO> DeleteTrip(int tripid);
    }
}
