

namespace City_Bus_Management_System.Services
{
    public class TripService(IMemoryCache cache, IUnitOfWork unitOfWork) : ITripService
    {
        public ResponseModel<List<TripDTO>> GetAllTrips()
        {
            if(!cache.TryGetValue("trips", out List<TripDTO> trips))
            {
                trips = unitOfWork.GetRepository<Trip>().FindAll<TripDTO>(t => !t.IsDeleted).ToList();

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
                unitOfWork.GetRepository<Trip>().AddAsync(trip);

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
                var trip = unitOfWork.GetRepository<Trip>().Find(t => t.Id == tripid);
                trip.IsDeleted = true;
                unitOfWork.GetRepository<Trip>().UpdateAsync(trip);
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
            var trip = unitOfWork.GetRepository<Trip>().Find(t => t.Id == tripid);

            trip.Duration = tripDto.Duration;
            trip.From = tripDto.From;
            trip.To = tripDto.To;

            try
            {
                unitOfWork.GetRepository<Trip>().UpdateAsync(trip);
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
