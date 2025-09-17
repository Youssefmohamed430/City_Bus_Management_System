using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Mapster;

namespace City_Bus_Management_System.Services
{
    public class BusService : IBusService
    {
        private AppDbContext context;
        public BusService(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<ResponseModel<BusDTO>> AddBus(BusDTO Newbus)
        {
            var bus = Newbus.Adapt<Bus>();

            try
            {
                await context.Buses.AddAsync(bus);
                await context.SaveChangesAsync();
                return new ResponseModel<BusDTO>
                {
                    Message = "Bus added successfully",
                    Result = Newbus,
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<BusDTO>
                {
                    Message = ex.Message,
                    Result = null
                };
            }
        }
        public async Task<ResponseModel<BusDTO>> DeleteBus(int busid)
        {
            var bus = context.Buses.FirstOrDefault(b => b.BusId == busid);

            try
            {
                context.Buses.Remove(bus);

                await context.SaveChangesAsync();

                return new ResponseModel<BusDTO>
                {
                    Message = "Bus deleted successfully",
                    Result = null!
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel<BusDTO>
                {
                    Message = ex.Message,
                    Result = null!
                };
            }
        }
        public ResponseModel<List<BusDTO>> GetBuses()
        {
            var buses = context.Buses
                .ProjectToType<BusDTO>()
                .ToList();

            if (buses.Count == 0)
            {
                return new ResponseModel<List<BusDTO>>
                {
                    Message = "No Buses Found",
                    Result = null!
                };
            }

            return new ResponseModel<List<BusDTO>>
            {
                Message = "All Buses",
                Result = buses
            };
        }
        public ResponseModel<BusDTO> GetBusByCode(string Code)
        {
            var bus = context.Buses
            .Where(b => b.BusCode == Code)
            .ProjectToType<BusDTO>()
            .FirstOrDefault();

            if (bus == null)
            {
                return new ResponseModel<BusDTO>
                {
                    Message = $"No Bus Found By Code {Code}",
                    Result = null!
                };
            }

            return new ResponseModel<BusDTO>
            {
                Message = $"Bus By Code {Code}",
                Result = bus
            };
        }
        public ResponseModel<List<BusDTO>> GetBusByType(string Type)
        {
            var bus = context.Buses
            .ProjectToType<BusDTO>()
            .Where(b => b.BusType == Type)
            .ToList();

            if (bus.Count == 0)
            {
                return new ResponseModel<List<BusDTO>>
                {
                    Message = $"No Buses Found By Type {Type}",
                    Result = null!
                };
            }

            return new ResponseModel<List<BusDTO>>
            {
                Message = $"All Buses By Type {Type}",
                Result = bus
            };
        }
        public async Task<ResponseModel<BusDTO>> UpdateBus(BusDTO Editedbus, int busId)
        {
            var bus = context.Buses.FirstOrDefault(b => b.BusId == busId);

            bus.BusCode = Editedbus.BusCode;
            bus.BusType = Editedbus.BusType;
            bus.TotalSeats = Editedbus.TotalSeats;

            try
            {
                context.Buses.Update(bus);

                await context.SaveChangesAsync();

                return new ResponseModel<BusDTO>
                {
                    Message = "Bus updated successfully",
                    Result = Editedbus
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<BusDTO>
                {
                    Message = ex.Message,
                    Result = null!
                };
            }
        }
    }
}
