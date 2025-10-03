using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Services;
using Core_Layer;
using Data_Access_Layer.DataLayer.DTOs;
using Data_Access_Layer.Factories;
using Mapster;
using Service_Layer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class RouteService : IRouteService
    {
        public IUnitOfWork unitOfWork { get; set; }
        public RouteService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public ResponseModel<RouteDTO> AddRoute(RouteDTO route)
        {
            var newRoute = route.Adapt<Route>();
            bool isSuccess = false;
            string msg = "";

            try
            {
                unitOfWork.Routes.AddAsync(newRoute);
                unitOfWork.SaveAsync(); 
                msg = "Route added successfully";
                isSuccess = true;
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                isSuccess = false;
            }
            return ResponseModelFactory<RouteDTO>.CreateResponse(msg, isSuccess ? newRoute.Adapt<RouteDTO>() : null!, isSuccess);
        }

        public ResponseModel<RouteDTO> DeleteRoute(int id)
        {
            var route = unitOfWork.Routes.Find(r => r.Id == id && !r.IsDeleted);
            bool isSuccess = true;
            string msg = "";

            try
            {
                route.IsDeleted = true;
                unitOfWork.Routes.UpdateAsync(route);
                unitOfWork.SaveAsync();
                msg = "Route Deleted successfully!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSuccess = false;
            }
            return ResponseModelFactory<RouteDTO>.CreateResponse(msg, null!, isSuccess);
        }

        public ResponseModel<List<RouteDTO>> GetRouteForTrip(int tripId)
        {
            var routes = unitOfWork.Routes
                .FindAll<RouteDTO>(r => r.TripId == tripId && !r.IsDeleted, new string[] { "station", "trip" })
                .OrderBy(r => r.Order);

            return ResponseModelFactory<List<RouteDTO>>.CreateResponse("Route For Trip Fetched Successfully!", routes.ToList());
        }

        public ResponseModel<List<RouteDTO>> GetRoutes()
        {
            var routes = unitOfWork.Routes.FindAll<RouteDTO>(r => !r.IsDeleted, new string[] { "station", "trip" });

            return ResponseModelFactory<List<RouteDTO>>.CreateResponse("All Routes Fetched Successfully!", routes.ToList());
        }

        public ResponseModel<RouteDTO> UpdateRoute(int id, RouteDTO Editedroute)
        {
            var route = unitOfWork.Routes.Find(r => r.Id == id && !r.IsDeleted);
            bool isSuccess = true;
            string msg = "";

            try
            {
                route.TripId = Convert.ToInt32(Editedroute.TripId);
                route.StationId = Convert.ToInt32(Editedroute.StationId);
                route.Order = Convert.ToInt32(Editedroute.Order);

                unitOfWork.Routes.UpdateAsync(route);
                unitOfWork.SaveAsync();
                msg = "Route Updated successfully!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSuccess = false;
            }
            return ResponseModelFactory<RouteDTO>.CreateResponse(msg, isSuccess ? route.Adapt<RouteDTO>() : null!, isSuccess);
        }

        public ResponseModel<StationRouteDTO> GetTheNearestStationAtRoute(int Tripid,double userlng, double userlat)
        {
            var routes = unitOfWork.Routes
                .FindAll<StationRouteDTO>(r => r.TripId == Tripid && !r.IsDeleted, new string[] { "station", "trip" });

            double minDistance = double.MaxValue;
            StationRouteDTO nearest = null!;

            foreach (var station in routes)
            {
                var distance = StationService.DistanceKm(userlat, userlng, station.Latitude, station.Longitude);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = station;
                }
            }

            return new ResponseModel<StationRouteDTO> { Message = "The Nearest Station fetched successfully", Result = nearest };
        }
    }
}
