using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Service_Layer.IServices;

namespace City_Bus_Management_System.Services
{
    public class TripService : ITripService
    {
        public IMemoryCache cache { get; set; }
        public IUnitOfWork unitOfWork { get; set; }
        public TripService(IMemoryCache _cache,IUnitOfWork _unitOfWork)
        {
            cache = _cache;
            unitOfWork = _unitOfWork;
        }
        public ResponseModel<List<TripDTO>> GetAllTrips()
        {
            if(!cache.TryGetValue("trips", out List<TripDTO> trips))
            {
                trips = unitOfWork.Trips.FindAll<TripDTO>(t => !t.IsDeleted).ToList();

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
                unitOfWork.Trips.AddAsync(trip);

                unitOfWork.SaveAsync();

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
            try
            {
                var trip = unitOfWork.Trips.Find(t => t.Id == tripid);
                trip.IsDeleted = true;
                unitOfWork.Trips.UpdateAsync(trip);
                unitOfWork.SaveAsync();

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
            var trip = unitOfWork.Trips.Find(t => t.Id == tripid);

            trip.Duration = tripDto.Duration;
            trip.From = tripDto.From;
            trip.To = tripDto.To;

            try
            {
                unitOfWork.Trips.UpdateAsync(trip);
                unitOfWork.SaveAsync();

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
