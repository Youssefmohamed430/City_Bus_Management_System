using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace Service_Layer.IServices
{
    public interface ITicketService
    {
        ResponseModel<TicketDTO> AddTicket(TicketDTO ticketDTO);
        ResponseModel<List<TicketDTO>> GetAllTickets();
        ResponseModel<List<TicketDTO>> GetTicketsByBusTypeAndNumberOfStations(int numOfStations,string BusType);
        ResponseModel<List<TicketDTO>> GetTicketByNumberOfStations(int numOfStations);
        ResponseModel<List<TicketDTO>> GetTicketByNumberOfStationsAndRangeOfPrice(int numOfStations,double MinPrice,double MaxPrice);
        ResponseModel<TicketDTO> UpdateTicket(int id,TicketDTO UpdatedTicket);
        ResponseModel<TicketDTO> DeleteTicket(int id);
    }
}
