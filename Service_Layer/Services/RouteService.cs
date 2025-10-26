using Newtonsoft.Json.Linq;

namespace Service_Layer.Services
{
    public class Coordinates
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
    }
    public class RouteService(IUnitOfWork unitOfWork) : IRouteService
    {
        public ResponseModel<RouteDTO> AddRoute(RouteDTO route)
        {
            var newRoute = route.Adapt<Route>();
            bool isSuccess = false;
            string msg = "";

            try
            {
                unitOfWork.GetRepository<Route>().AddAsync(newRoute);
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
            var route = unitOfWork.GetRepository<Route>().Find(r => r.Id == id && !r.IsDeleted);
            bool isSuccess = true;
            string msg = "";

            try
            {
                route.IsDeleted = true;
                unitOfWork.GetRepository<Route>().UpdateAsync(route);
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
            var routes = unitOfWork.GetRepository<Route>()
                .FindAll<RouteDTO>(r => r.TripId == tripId && !r.IsDeleted, new string[] { "station", "trip" })
                .OrderBy(r => r.Order);

            return ResponseModelFactory<List<RouteDTO>>.CreateResponse("Route For Trip Fetched Successfully!", routes.ToList());
        }

        public ResponseModel<List<RouteDTO>> GetRoutes()
        {
            var routes = unitOfWork.GetRepository<Route>().FindAll<RouteDTO>(r => !r.IsDeleted, new string[] { "station", "trip" });

            return ResponseModelFactory<List<RouteDTO>>.CreateResponse("All Routes Fetched Successfully!", routes.ToList());
        }

        public ResponseModel<RouteDTO> UpdateRoute(int id, RouteDTO Editedroute)
        {
            var route = unitOfWork.GetRepository<Route>().Find(r => r.Id == id && !r.IsDeleted);
            bool isSuccess = true;
            string msg = "";

            try
            {
                route.TripId = Convert.ToInt32(Editedroute.TripId);
                route.StationId = Convert.ToInt32(Editedroute.StationId);
                route.Order = Convert.ToInt32(Editedroute.Order);

                unitOfWork.GetRepository<Route>().UpdateAsync(route);
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
            var routes = unitOfWork.GetRepository<Route>()
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

            var coordinates = CalcDistanceToDistnation(userlng, userlat, nearest.Longitude, nearest.Latitude);

            nearest.Distance = coordinates.Result.Distance;
            nearest.Duration = coordinates.Result.Duration;

            return new ResponseModel<StationRouteDTO> { Message = "The Nearest Station fetched successfully", Result = nearest };
        }
        public async Task<Coordinates> CalcDistanceToDistnation(double userlng, double userlat,double stationlong,double stationlat)
        {
            var apiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6IjNjYzFiNmYxZmIzMjRmMWFiNGNiY2E5NjUwZDJkN2ViIiwiaCI6Im11cm11cjY0In0=";
            var start = $"{userlng},{userlat}";
            var end = $"{stationlong},{stationlat}";

            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={apiKey}&start={start}&end={end}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(content);
            var summary = json["features"][0]["properties"]["summary"];

            Coordinates result = new Coordinates
            {
                Distance = Math.Round((summary["distance"]?.Value<double>() ?? 0) / 1000), // km
                Duration = Math.Round(TimeSpan.FromSeconds(summary["duration"]?.Value<double>() ?? 0).TotalMinutes) // minutes
            };

            return result;
        }
    }
}
