using City_Bus_Management_System.DataLayer;
using City_Bus_Management_System.DataLayer.DTOs;

namespace City_Bus_Management_System.Services
{
    public class DriverScheduleService : IDriverScheduleService
    {

        public ResponseModel<List<DriverScheduleDTO>> GetSchedules()
        {
            throw new NotImplementedException();
        }

        public ResponseModel<DriverScheduleDTO> GetSchedulesByDriverId()
        {
            throw new NotImplementedException();
        }

        public ResponseModel<DriverScheduleDTO> GetSchedulesByDriverName()
        {
            throw new NotImplementedException();
        }

        public void AssignDriverToBus(DriverScheduleDTO driverSchedule)
        {
            throw new NotImplementedException();
        }

        public void UpdateDriverSchedule(int SchId, string newSchedule)
        {
            throw new NotImplementedException();
        }

        public void RemoveDriverSchedule(int SchId)
        {
            throw new NotImplementedException();
        }
    }
}
