using Service_Layer.Services;


namespace Service_Layer.IServices
{
    public interface IRouteService
    {
        ResponseModel<List<RouteDTO>> GetRouteForTrip(int tripId);
        ResponseModel<List<RouteDTO>> GetRoutes();
        ResponseModel<RouteDTO> AddRoute(RouteDTO route);
        ResponseModel<RouteDTO> UpdateRoute(int id,RouteDTO route);
        ResponseModel<RouteDTO> DeleteRoute(int id);
        ResponseModel<StationRouteDTO> GetTheNearestStationAtRoute(int Tripid, double userlng, double userlat);
        Task<Coordinates> CalcDistanceToDistnation(double userlng, double userlat, double stationlong, double stationlat);
    }
}
