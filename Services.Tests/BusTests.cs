using AutoFixture;
using AutoFixture.AutoMoq;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Services;
using Core_Layer;
using Core_Layer.IRepositries;
using Microsoft.Extensions.Logging;
using Moq;
using Service_Layer.IServices;
using System.Linq.Expressions;

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
        public void GetBuses_BusesExist_ReturnsAllBuses()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            // 2. دع Fixture ينشئ لك الكائنات الوهمية
            var mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
            // Arrange
            var mockLogger = new Mock<ILogger<BusService>>();
            //var mockUnitOfWork = new Mock<IUnitOfWork>();

            var expectedBuses = new List<BusDTO>
            {
                new BusDTO { BusId = 3, BusCode = "Bus1-23689", BusType = "Normal", TotalSeats = 30 },
                new BusDTO { BusId = 4, BusCode = "Bus1-28729", BusType = "Luxury", TotalSeats = 25 },
                new BusDTO { BusId = 5, BusCode = "Bus2-28123", BusType = "AirConditioned", TotalSeats = 25 },
            };

            var mockBusRepo = new Mock<IBaseRepository<Bus>>();

            mockBusRepo
                .Setup(r => r.FindAll<BusDTO>(It.IsAny<Expression<Func<Bus, bool>>>(), null))
                .Returns(expectedBuses.AsQueryable());

            mockUnitOfWork.Setup(u => u.Buses).Returns(mockBusRepo.Object);

            var busService = fixture.Create<BusService>();

            // Act
            var response = busService.GetBuses();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Equal("AllBuses", response.Message);
            Assert.Equal(expectedBuses.Count, response.Result.Count);
            Assert.Equal(expectedBuses.Select(b => b.BusId), response.Result.Select(b => b.BusId));
        }


    }
}
