using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Services
{
    public class TripService : ITripService
    {
        public AppDbContext context { get; set; }
        public IMemoryCache cache { get; set; }
        public TripService(AppDbContext _context, IMemoryCache _cache)
        {
            context = _context;
            cache = _cache;
        }
        public ResponseModel<List<TripDTO>> GetAllTrips()
        {
            if(!cache.TryGetValue("trips", out List<TripDTO> trips))
            {
                trips = context.Trips
                    .AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .ProjectToType<TripDTO>()
                    .ToList();

                cache.Set("trips", trips, TimeSpan.FromMinutes(20));
            }

            return new ResponseModel<List<TripDTO>>
            {
                IsSuccess = true,
                Message = "Trips fetched successfully",
                Result = trips
            };
        }

        public ResponseModel<TripDTO> AddTrip(TripDTO tripDto)
        {
            var trip = tripDto.Adapt<Trip>();

            try
            {
                context.Trips.Add(trip);

                context.SaveChanges();

                cache.Remove("trips");

                return new ResponseModel<TripDTO>
                {
                    Message = "Trip added successfully",
                    Result = trip.Adapt<TripDTO>()
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<TripDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public ResponseModel<TripDTO> DeleteTrip(int tripid)
        {
            var trip = context.Trips.FirstOrDefault(t => t.Id == tripid && !t.IsDeleted);

            try
            {
                trip.IsDeleted = true;
                context.Update(trip);
                context.SaveChanges();

                cache.Remove("trips");

                return new ResponseModel<TripDTO>
                {
                    Message = "Trip deleted successfully",
                    Result = null!
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<TripDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public ResponseModel<TripDTO> UpdateTrip(int tripid, TripDTO tripDto)
        {
            var trip = context.Trips.FirstOrDefault(t => t.Id == tripid);

            trip.Duration = tripDto.Duration;
            trip.From = tripDto.From;
            trip.To = tripDto.To;

            try
            {
                context.Update(trip);
                context.SaveChanges();

                cache.Remove("trips");
                return new ResponseModel<TripDTO>
                {
                    Message = "Trip updated successfully",
                    Result = trip.Adapt<TripDTO>()
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<TripDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
