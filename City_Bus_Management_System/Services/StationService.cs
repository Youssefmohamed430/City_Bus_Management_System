using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;

namespace City_Bus_Management_System.Services
{
    public class GraphNode
    {

    }
    public class StationService : IStationService
    {
        public AppDbContext context { get; set; }
        public IMemoryCache cache { get; set; }
        public StationService(AppDbContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
        }

        public ResponseModel<List<StationDTO>> GetStations()
        {
            if(!cache.TryGetValue("stations",out List<StationDTO> stations))
            {
                stations = context.Stations
                    .AsNoTracking()
                    .Where(s => !s.IsDeleted)
                    .ProjectToType<StationDTO>()
                    .ToList();

                cache.Set("stations", stations, TimeSpan.FromMinutes(15));
            }

            return new ResponseModel<List<StationDTO>> { Message = "Stations fetched successfully", Result = stations };
        }
        public ResponseModel<List<StationDTO>> GetStationsByArea(string area)
        {
            List<StationDTO> StationsByArea = null!;
            if (cache.TryGetValue("stations", out List<StationDTO> stations))
            {
                StationsByArea = stations
                    .Where(s => s.Area == area)
                    .ToList()!;
            }
            else
            {
                StationsByArea = context.Stations
                    .AsNoTracking()
                    .Where(s => s.Area == area && !s.IsDeleted)
                    .ProjectToType<StationDTO>()
                    .ToList()!;
            }

            if (StationsByArea.Count == 0)
                return new ResponseModel<List<StationDTO>> { IsSuccess = false, Message = "No stations found in the specified area.", Result = null! };

            return new ResponseModel<List<StationDTO>> { Message = "Station By Area fetched successfully", Result = StationsByArea };
        }
        public ResponseModel<StationDTO> GetStationByName(string name)
        {
            StationDTO StationsByName = null;
            if (cache.TryGetValue("stations", out List<StationDTO> stations))
            {
                StationsByName = stations
                    .FirstOrDefault(s => s.Name == name)!;
            }
            else
            {
                StationsByName = context.Stations
                    .AsNoTracking()
                    .Where(s => s.Name == name && !s.IsDeleted)
                    .ProjectToType<StationDTO>()
                    .FirstOrDefault()!;
            }

            return new ResponseModel<StationDTO> { Message = "Station By Name fetched successfully", Result = StationsByName };
        }
        public async Task<ResponseModel<StationDTO>> GetTheNearestStation(string area)
        {
            var stationsInArea = GetStationsByArea(area);

            if (!stationsInArea.IsSuccess || stationsInArea.Result.Count == 0)
                return new ResponseModel<StationDTO> { IsSuccess = false, Message = "No stations found in the specified area.", Result = null! };

            var http = new HttpClient();

            var json = await http.GetStringAsync("https://ipinfo.io/json");
            var obj = JsonDocument.Parse(json);
            var loc = obj.RootElement.GetProperty("loc").GetString()?.Split(','); // "lat,lng"
            var userlat = Convert.ToDouble(loc![0]);
            var userlng = Convert.ToDouble(loc[1]);

            double minDistance = double.MaxValue;
            StationDTO nearest = null!;

            foreach (var station in stationsInArea.Result)
            {
                var distance = DistanceKm(userlat, userlng, station.Latitude, station.Longitude);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = station;
                }
            }

            return new ResponseModel<StationDTO> { Message = "The Nearest Station fetched successfully", Result = nearest };
        }
        public ResponseModel<StationDTO> AddStation(StationDTO station)
        {
            var NewStation = station.Adapt<Station>();

            try
            {
                context.Stations.Add(NewStation);

                context.SaveChanges();
                cache.Remove("schedules");

                return new ResponseModel<StationDTO>
                { Message = "Station Added successfully.", Result = station };
            }
            catch (Exception ex)
            {
                return new ResponseModel<StationDTO> 
                {IsSuccess = false , Message = ex.Message ,Result = null! };
            }
        }
        public ResponseModel<StationDTO> UpdateStation(int stationId, StationDTO Updatedstation)
        {
            var station = context.Stations.FirstOrDefault(s => s.Id == stationId)!;

            station.Area = Updatedstation.Area!;
            station.Name = Updatedstation.Name!;
            station.Location = Updatedstation.Location;
            station.Latitude = Updatedstation.Latitude;


            try
            {
                context.Update(station);
                context.SaveChanges();
                cache.Remove("stations");
                return new ResponseModel<StationDTO> { Message = "Station Updated successfully", Result = Updatedstation };
            }
            catch (Exception ex)
            {
                return new ResponseModel<StationDTO>
                { IsSuccess = false, Message = ex.Message,Result = null! };
            }
        }
        public ResponseModel<StationDTO> DeleteStation(int stationid)
        {
            var station = context.Stations.FirstOrDefault(s => s.Id == stationid);

            try
            {
                station.IsDeleted = true;
                context.Update(station);
                context.SaveChanges();
                cache.Remove("schedules");
                return new ResponseModel<StationDTO> { Message = "Station removed successfully", Result = null! };
            }
            catch (Exception ex)
            {
                return new ResponseModel<StationDTO> { Message = ex.Message , Result = null! };
            }
        }
        public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) *
                Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }
    
    }
}
