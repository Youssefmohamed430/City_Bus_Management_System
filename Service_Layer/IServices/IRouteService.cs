using City_Bus_Management_System.DataLayer;
using Data_Access_Layer.DataLayer.DTOs;
using Service_Layer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<ResponseModel<Coordinates>> CalcDistanceToStation(int Tripid, double userlng, double userlat);
    }
}
