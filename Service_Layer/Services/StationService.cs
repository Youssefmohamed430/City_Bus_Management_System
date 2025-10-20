using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer;
using Data_Access_Layer.DataLayer.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Service_Layer.IServices;
using System.Text.Json;

namespace City_Bus_Management_System.Services
{
    public class StationService(IMemoryCache cache, IUnitOfWork unitOfWork) : IStationService
    {

        public ResponseModel<List<StationDTO>> GetStations()
        {
            if(!cache.TryGetValue("stations",out List<StationDTO> stations))
            {
                stations = unitOfWork.GetRepository<Station>()
                    .FindAll<StationDTO>(s => !s.IsDeleted)
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
                StationsByArea = unitOfWork.GetRepository<Station>()
                    .FindAll<StationDTO>(s => s.Area == area && !s.IsDeleted)
                    .ToList(); 
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
                StationsByName = unitOfWork.GetRepository<Station>()
                    .Find<StationDTO>(s => s.Name == name && !s.IsDeleted);
            }

            return new ResponseModel<StationDTO> { Message = "Station By Name fetched successfully", Result = StationsByName };
        }
        public ResponseModel<StationDTO> GetTheNearestStation(LocationDTO myLocation)
        {
            var stationsInArea = GetStationsByArea(myLocation.Area);

            if (!stationsInArea.IsSuccess || stationsInArea.Result.Count == 0)
                return new ResponseModel<StationDTO> { IsSuccess = false, Message = "No stations found in the specified area.", Result = null! };

            double minDistance = double.MaxValue;
            StationDTO nearest = null!;

            foreach (var station in stationsInArea.Result)
            {
                var distance = DistanceKm(myLocation.Latitude, myLocation.Longitude, station.Latitude, station.Longitude);
                
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
                unitOfWork.GetRepository<Station>().AddAsync(NewStation);
                unitOfWork.SaveAsync(); 
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
            var station = unitOfWork.GetRepository<Station>().Find(s => s.Id == stationId)!;

            station.Area = Updatedstation.Area!;
            station.Name = Updatedstation.Name!;
            station.Location = Updatedstation.Location;
            station.Latitude = Updatedstation.Latitude;


            try
            {
                unitOfWork.GetRepository<Station>().UpdateAsync(station);
                unitOfWork.SaveAsync();
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
            var station = unitOfWork.GetRepository<Station>().Find(s => s.Id == stationid);

            try
            {
                station.IsDeleted = true;
                unitOfWork.GetRepository<Station>().UpdateAsync(station);
                unitOfWork.SaveAsync();
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
