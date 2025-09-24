using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Services
{
    public class StationService : IStationService
    {
        public AppDbContext context { get; set; }
        public IMemoryCache cache { get; set; }
        public StationService(AppDbContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
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

        public ResponseModel<StationDTO> DeleteStation(int stationid)
        {
            var station = context.Stations.FirstOrDefault(s => s.Id == stationid);

            try
            {
                station.IsDeleted = true;
                context.Update(station);
                context.SaveChanges();
                cache.Remove("schedules");
                return new ResponseModel<StationDTO> { Message = "Schedule removed successfully", Result = null! };
            }
            catch (Exception ex)
            {
                return new ResponseModel<StationDTO> { Message = ex.Message , Result = null! };
            }
        }

        public ResponseModel<StationDTO> GetStationByArea(string area)
        {
            StationDTO StationsByArea = null!;
            if (cache.TryGetValue("stations", out List<StationDTO> stations))
            {
                StationsByArea = stations
                    .FirstOrDefault(s => s.Area == area)!;
            }
            else
            {
                StationsByArea = context.Stations
                    .AsNoTracking()
                    .Where(s => s.Area == area)
                    .ProjectToType<StationDTO>()
                    .FirstOrDefault()!;

                cache.Set("stations", stations, TimeSpan.FromMinutes(15));
            }

            return new ResponseModel<StationDTO> { Message = "Station By Area fetched successfully", Result = StationsByArea };
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
                    .Where(s => s.Name == name)
                    .ProjectToType<StationDTO>()
                    .FirstOrDefault()!;

                cache.Set("stations", stations, TimeSpan.FromMinutes(15));
            }

            return new ResponseModel<StationDTO> { Message = "Station By Name fetched successfully", Result = StationsByName };
        }

        public ResponseModel<List<StationDTO>> GetStations()
        {
            if(!cache.TryGetValue("stations",out List<StationDTO> stations))
            {
                stations = context.Stations
                    .AsNoTracking()
                    .ProjectToType<StationDTO>()
                    .ToList();

                cache.Set("stations", stations, TimeSpan.FromMinutes(15));
            }

            return new ResponseModel<List<StationDTO>> { Message = "Stations fetched successfully", Result = stations };
        }

        public ResponseModel<List<StationDTO>> GetTheNearestStation(decimal longitude, decimal latitude)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<StationDTO> UpdateStation(int stationId, StationDTO Updatedstation)
        {
            var station = context.Stations.FirstOrDefault(s => s.Id == stationId)!;

            station.Area = Updatedstation.Area!;
            station.Name = Updatedstation.Name!;
            station.Longitude = Convert.ToDecimal(Updatedstation.Longitude!);
            station.Latitude = Convert.ToDecimal(Updatedstation.Latitude!);

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
    }
}
