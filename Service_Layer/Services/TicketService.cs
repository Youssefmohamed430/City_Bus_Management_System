using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using Service_Layer.IServices;

namespace City_Bus_Management_System.Services
{
    public class TicketService : ITicketService
    {
        public ResponseModel<TicketDTO> AddTicket(TicketDTO ticketDTO)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<TicketDTO> DeleteTicket(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<List<TicketDTO>> GetAllTickets()
        {
            throw new NotImplementedException();
        }

        public ResponseModel<List<TicketDTO>> GetTicketByNumberOfStations(int numOfStations)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<List<TicketDTO>> GetTicketByNumberOfStationsAndRangeOfPrice(int numOfStations, double MinPrice, double MaxPrice)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<List<TicketDTO>> GetTicketsByBusTypeAndNumberOfStations(int numOfStations, string BusType)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<TicketDTO> UpdateTicket(int id, TicketDTO UpdatedTicket)
        {
            throw new NotImplementedException();
        }
    }
}
