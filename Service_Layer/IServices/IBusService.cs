
namespace Service_Layer.IServices
{
    public interface IBusService
    {
        ResponseModel<List<BusDTO>> GetBuses();
        ResponseModel<BusDTO> GetBusByCode(string Code);
        ResponseModel<List<BusDTO>> GetBusByType(string Type);
        Task<ResponseModel<BusDTO>> AddBus(BusDTO Newbus);
        Task<ResponseModel<BusDTO>> UpdateBus(int id,BusDTO Editedbus);
        Task<ResponseModel<BusDTO>> DeleteBus(int id);
    }
}
