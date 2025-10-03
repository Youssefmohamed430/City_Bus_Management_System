using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer;
using Data_Access_Layer.Factories;
using Mapster;
using Service_Layer.IServices;

namespace City_Bus_Management_System.Services
{
    public class TicketService : ITicketService
    {
        private IUnitOfWork unitOfWork;
        public TicketService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public ResponseModel<TicketDTO> AddTicket(TicketDTO ticketDTO)
        {
            var ticket = ticketDTO.Adapt<Ticket>();

            try
            {
                unitOfWork.Tickets.AddAsync(ticket);
                unitOfWork.SaveAsync();
                var addedTicket = ticket.Adapt<TicketDTO>();
                return ResponseModelFactory<TicketDTO>.CreateResponse("Ticket added successfully.", addedTicket);
            }
            catch (Exception ex)
            {
                return ResponseModelFactory<TicketDTO>.CreateResponse($"Error adding ticket: {ex.Message}", null!, false);
            }
        }
        public ResponseModel<TicketDTO> DeleteTicket(int id)
        {
            var ticket = unitOfWork.Tickets.Find<Ticket>(t => t.Id == id && !t.IsDeleted);

            try
            {
                ticket.IsDeleted = true;
                unitOfWork.Tickets.UpdateAsync(ticket);
                unitOfWork.SaveAsync();

                return ResponseModelFactory<TicketDTO>.CreateResponse("Ticket Deleted successfully!", null!);
            }
            catch (Exception ex)
            {
                return ResponseModelFactory<TicketDTO>.CreateResponse($"Error Deleteing Ticket {ex.Message}", null!, false);
            }
        }
        public ResponseModel<List<TicketDTO>> GetAllTickets()
        {
            var tickets = unitOfWork.Tickets.FindAll<TicketDTO>(t => !t.IsDeleted);

            if (tickets == null || tickets.Count() == 0)
                return ResponseModelFactory<List<TicketDTO>>.CreateResponse("No tickets found.", null!, false);

            return ResponseModelFactory<List<TicketDTO>>.CreateResponse("All tickets retrieved successfully.", tickets.ToList());
        }

        public ResponseModel<List<TicketDTO>> GetTicketByNumberOfStations(int numOfStations)
        {
            var tickets = unitOfWork.Tickets
                .FindAll<TicketDTO>(t => !t.IsDeleted && t.MinStations >= numOfStations);

            if (tickets == null || tickets.Count() == 0)
                return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"No tickets found By {numOfStations} stations.", null!, false);

            return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"All tickets By {numOfStations} stations retrieved successfully.", tickets.ToList());
        }

        public ResponseModel<List<TicketDTO>> GetTicketByNumberOfStationsAndRangeOfPrice(int numOfStations, double MinPrice, double MaxPrice)
        {
            var tickets = unitOfWork.Tickets
                .FindAll<TicketDTO>(t => !t.IsDeleted && t.MinStations >= numOfStations);

            if (tickets == null || tickets.Count() == 0)
                return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"No tickets found By {numOfStations} stations.", null!, false);

            tickets = tickets.Where(t => t.Price >= MinPrice && t.Price <= MaxPrice);

            if (tickets == null || tickets.Count() == 0)
                return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"No tickets found between {MinPrice} , {MaxPrice}.", null!, false);

            return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"All tickets By {numOfStations} stations between {MinPrice} , {MaxPrice} retrieved successfully.", tickets.ToList());
        }

        public ResponseModel<List<TicketDTO>> GetTicketsByBusTypeAndNumberOfStations(int numOfStations, string BusType)
        {
            var tickets = unitOfWork.Tickets
                .FindAll<TicketDTO>(t => !t.IsDeleted && t.MinStations >= numOfStations);

            if (tickets == null || tickets.Count() == 0)
                return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"No tickets found By {numOfStations} stations.", null!, false);

            tickets = tickets.Where(t => t.BusType == BusType);

            if (tickets == null || tickets.Count() == 0)
                return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"No tickets found by type {BusType}.", null!, false);

            return ResponseModelFactory<List<TicketDTO>>.CreateResponse($"All tickets By {numOfStations} stations By bus type {BusType} retrieved successfully.", tickets.ToList());
        }

        public ResponseModel<TicketDTO> UpdateTicket(int id, TicketDTO UpdatedTicket)
        {
            var ticket = unitOfWork.Tickets.Find(t => t.Id == id && !t.IsDeleted);  

            ticket.MinStations = UpdatedTicket.MinStations;
            ticket.BusType = UpdatedTicket.BusType;
            ticket.Price = UpdatedTicket.Price;

            try
            {
                unitOfWork.Tickets.UpdateAsync(ticket);
                unitOfWork.SaveAsync();

                return ResponseModelFactory<TicketDTO>.CreateResponse("Ticket updated successfully", UpdatedTicket);
            }
            catch (Exception ex)
            {
                return ResponseModelFactory<TicketDTO>.CreateResponse($"Error Updateing ticket: {ex.Message}", null!,false);
            }
        }
    }
}
