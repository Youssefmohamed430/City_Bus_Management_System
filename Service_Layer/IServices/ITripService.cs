
namespace Service_Layer.IServices
{
    public interface ITripService
    {
        List<TripDTO> GetAllTrips();
        ResponseModel<TripDTO> AddTrip(TripDTO tripDto);
        ResponseModel<TripDTO> UpdateTrip(int tripid , TripDTO tripDto);
        ResponseModel<TripDTO> DeleteTrip(int tripid);
    }
}
