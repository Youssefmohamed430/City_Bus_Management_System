using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.Services;
using Core_Layer;
using Microsoft.Extensions.Logging;
using Moq;
using Service_Layer.IServices;

namespace Services.Tests
{
    public class BusTests
    {
        private ILogger<BusService> logger;
        private IUnitOfWork unitOfWork;
        public BusTests(ILogger<BusService> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        //[Fact]
        //public void Method_Scenario_Outcome()
        //{
        // Arrange

        // Act

        // Assert 
        //}
        [Fact]
        public void GetBuses_BusesAreExisted_FetchedAllBuses()
        {
            // Arrange
            List<BusDTO> buses = new List<BusDTO>()
            {
                new BusDTO { BusId = 3, BusCode = "Bus1-23689", BusType = "Normal", TotalSeats = 30 },
                new BusDTO { BusId = 4, BusCode = "Bus1-28729", BusType = "Luxury", TotalSeats = 25 },
                new BusDTO { BusId = 5, BusCode = "Bus2-28123", BusType = "AirConditioned", TotalSeats = 25 },
            };

            var movkBusService = new Mock<IBusService>();

            // Act
            List<BusDTO> result = null;

            movkBusService.Setup(s => s.GetBuses().Result).Returns(result);

            // Assert
            Assert.Equal(buses, result);
        }

    }
}
