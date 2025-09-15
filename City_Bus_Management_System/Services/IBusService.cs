using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;

namespace City_Bus_Management_System.Services
{
    public interface IBusService
    {
        ResponseModel<List<BusDTO>> GetBuses();
        ResponseModel<BusDTO> GetBusByCode(string Code);
        ResponseModel<List<BusDTO>> GetBusByType(string Type);
        Task<ResponseModel<BusDTO>> AddBus(BusDTO Newbus);
        Task<ResponseModel<BusDTO>> UpdateBus(BusDTO Editedbus,int busId);
        Task<ResponseModel<BusDTO>> DeleteBus(int busid);
    }
}
