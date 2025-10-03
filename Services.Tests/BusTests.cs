using AutoFixture;
using AutoFixture.AutoMoq;
using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Services;
using Core_Layer;
using Core_Layer.IRepositries;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Moq;
using Service_Layer.IServices;
using System.Linq.Expressions;

namespace Services.Tests
{
    public class BusTests
    {
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
            var buses = new List<BusDTO>
            {
                 new BusDTO { BusId = 1, BusCode = "B1", BusType = "Normal", TotalSeats = 40 },
                 new BusDTO { BusId = 2, BusCode = "B2", BusType = "Luxury", TotalSeats = 25 }
            };

            var fakeLogger = A.Fake<ILogger<BusService>>(); 
            var fakeunitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => fakeunitOfWork.Buses.FindAll<BusDTO>(A<Expression<Func<Bus, bool>>>.Ignored,null!))
                  .Returns(buses.AsQueryable());

            // Act
            var busService = new BusService(fakeLogger, fakeunitOfWork);

            var result = busService.GetBuses().Result;

            // Assert
            Assert.Equal(buses, result);
        }
        [Fact]
        public void GetBuses_BusesAreEmpty_ReturnsEmptyList()
        {
            // Arrange
            //List<BusDTO> buses = new List<BusDTO>();

            var fakeLogger = A.Fake<ILogger<BusService>>();
            var fakeunitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => fakeunitOfWork.Buses.FindAll<BusDTO>(A<Expression<Func<Bus, bool>>>.Ignored, null!))
                  .Returns(null!);

            // Act
            var busService = new BusService(fakeLogger, fakeunitOfWork);

            var result = busService.GetBuses().Result;

            // Assert
            Assert.Null(result);
        }
    }
}
