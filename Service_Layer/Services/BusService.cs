using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer;
using Data_Access_Layer.Factories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service_Layer.IServices;

namespace City_Bus_Management_System.Services
{
    public class BusService : IBusService
    {
        private ILogger<BusService> logger;
        private IUnitOfWork unitOfWork;
        public BusService(ILogger<BusService> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        public ResponseModel<List<BusDTO>> GetBuses()
        {
            var buses = unitOfWork.Buses
                .FindAll<BusDTO>(b => !b.IsDeleted)
                .ToList();

            if (buses.Count == 0)
                return ResponseModelFactory<List<BusDTO>>.CreateResponse($"No Buses Found", null!,false);

            return ResponseModelFactory<List<BusDTO>>.CreateResponse("AllBuses",buses);
        }
        public ResponseModel<BusDTO> GetBusByCode(string Code)
        {
            var bus = unitOfWork.Buses
                .Find<BusDTO>(b => !b.IsDeleted && b.BusCode == Code);

            if (bus == null)
                return ResponseModelFactory<BusDTO>.CreateResponse($"No Bus Found By Code {Code}", null!,false);

            return ResponseModelFactory<BusDTO>.CreateResponse($"Bus By Code {Code}", bus);
        }
        public ResponseModel<List<BusDTO>> GetBusByType(string Type)
        {
            var bus = unitOfWork.Buses
                .FindAll<BusDTO>(b => !b.IsDeleted && b.BusType == Type).ToList();

            if (bus.Count == 0)
                return ResponseModelFactory<List<BusDTO>>.CreateResponse($"No Buses Found By Type {Type}", bus,false);

            return ResponseModelFactory<List<BusDTO>>.CreateResponse($"All Buses By Type {Type}", bus);
        }
        public async Task<ResponseModel<BusDTO>> AddBus(BusDTO Newbus)
        {
            var bus = Newbus.Adapt<Bus>();
            string msg = "";
            bool IsSuccess = true;
            try
            {
                await unitOfWork.Buses.AddAsync(bus);
                await unitOfWork.SaveAsync();
                msg = "Bus added successfully";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding bus");
                msg = ex.Message;
                IsSuccess = false;
            }

            return ResponseModelFactory<BusDTO>.CreateResponse(msg,IsSuccess ? Newbus : null!,IsSuccess);
        }
        public async Task<ResponseModel<BusDTO>> UpdateBus(int id,BusDTO Editedbus)
        {
            string msg = "";
            var bus = unitOfWork.Buses.Find(b => !b.IsDeleted && b.BusId == id);
            bus.BusCode = Editedbus.BusCode;
            bus.BusType = Editedbus.BusType;
            bus.TotalSeats = Editedbus.TotalSeats;
            bool IsSuccess = true;

            try
            {
                await unitOfWork.Buses.UpdateAsync(bus);
                await unitOfWork.SaveAsync();
                msg = "Bus updated successfully";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating bus");
                msg = ex.Message;
                IsSuccess = false;
            }

            return ResponseModelFactory<BusDTO>.CreateResponse(msg,IsSuccess ? Editedbus : null!,IsSuccess);
        }
        public async Task<ResponseModel<BusDTO>> DeleteBus(int id)
        {
            var deletedbus = unitOfWork.Buses.Find(b => !b.IsDeleted && b.BusId == id);
            string msg = "";
            bool IsSuccess = true;

            try
            {
                deletedbus.IsDeleted = true;
                await unitOfWork.Buses.UpdateAsync(deletedbus);
                await unitOfWork.SaveAsync();
                msg = "Bus deleted successfully";

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Deleteing bus");
                msg = ex.Message;
                IsSuccess = false;
            }
            return ResponseModelFactory<BusDTO>.CreateResponse(msg,null);
        }
    }
}
