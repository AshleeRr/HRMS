using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class TarifaRepositoryTests
    {
        private readonly Mock<ILogger<TarifaRepository>> _mockLogger = new();
        private readonly Mock<IValidator<Tarifas>> _mockValidator = new();

        private HRMSContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new HRMSContext(options);
        }

        private TarifaRepository CreateRepository(HRMSContext context)
        {
            return new TarifaRepository(
                context,
                _mockLogger.Object,
                Mock.Of<IConfiguration>(),
                _mockValidator.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveTarifas()
        {
            // Arrange
            using var context = CreateContext();
            context.Tarifas.AddRange(
                new Tarifas { Estado = true },
                new Tarifas { Estado = false }
            );
            await context.SaveChangesAsync();
            
            var repo = CreateRepository(context);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Single(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetEntityByIdAsync_InvalidIds_ThrowsException(int invalidId)
        {
            // Arrange
            using var context = CreateContext();
            var repo = CreateRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repo.GetEntityByIdAsync(invalidId));
        }

        [Fact]
        public async Task GetEntityByIdAsync_NonExistentId_ThrowsException()
        {
            // Arrange
            using var context = CreateContext();
            var repo = CreateRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.GetEntityByIdAsync(999));
        }

        [Fact]
        public async Task SaveEntityAsync_ValidTarifa_ReturnsSuccess()
        {
            // Arrange
            using var context = CreateContext();
            var repo = CreateRepository(context);
            var newTarifa = new Tarifas { 
                PrecioPorNoche = 100,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(10),
                Estado = true
            };
    
            _mockValidator.Setup(v => v.Validate(It.IsAny<Tarifas>()))
                .Returns(new OperationResult { IsSuccess = true });

            // Act
            var result = await repo.SaveEntityAsync(newTarifa);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("guardada exitosamente", result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_InvalidValidator_ReturnsFailure()
        {
            // Arrange
            using var context = CreateContext();
            var repo = CreateRepository(context);
            var tarifa = new Tarifas();
            
            _mockValidator.Setup(v => v.Validate(tarifa))
                .Returns( new OperationResult{IsSuccess = false , Message = "Validation error"});

            // Act
            var result = await repo.SaveEntityAsync(tarifa);

            // Assert
            Assert.False(result.IsSuccess);
        }

     
        [Fact]
        public async Task UpdateEntityAsync_SamePriceDifferentId_ReturnsSuccess()
        {
            // Arrange
            using var context = CreateContext();
    
            var tarifa = new Tarifas { 
                PrecioPorNoche = 100,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(10),
                Estado = true
            };
    
            context.Tarifas.Add(tarifa);
            await context.SaveChangesAsync();
    
            _mockValidator.Setup(v => v.Validate(It.IsAny<Tarifas>()))
                .Returns(new OperationResult { IsSuccess = true });
    
            var repo = CreateRepository(context);
    

            // Act
            var result = await repo.UpdateEntityAsync(tarifa);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData("2023-01-15", 1)]  // Within range
        [InlineData("2023-01-01", 1)]  // Start date
        [InlineData("2023-01-31", 1)]  // End date
        [InlineData("2022-12-31", 0)]  // Before range
        [InlineData("2023-02-01", 0)]  // After range
        public async Task GetTarifasVigentesAsync_DateScenarios_ReturnsCorrectCount(string date, int expected)
        {
            // Arrange
            using var context = CreateContext();
            context.Tarifas.Add(new Tarifas {
                FechaInicio = new DateTime(2023, 1, 1),
                FechaFin = new DateTime(2023, 1, 31),
                Estado = true
            });
            await context.SaveChangesAsync();
            
            var repo = CreateRepository(context);

            // Act
            var result = await repo.GetTarifasVigentesAsync(date);

            // Assert
            Assert.Equal(expected, result.IsSuccess ? ((List<Tarifas>?)result.Data)?.Count : 0);            
        }

        [Theory]
        [InlineData("invalid-date")]
        [InlineData("32/13/2023")]
        public async Task GetTarifasVigentesAsync_InvalidFormats_ReturnsError(string invalidDate)
        {
            // Arrange
            using var context = CreateContext();
            var repo = CreateRepository(context);

            // Act
            var result = await repo.GetTarifasVigentesAsync(invalidDate);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("formato de la fecha", result.Message);
        }

        [Fact]
        public async Task GetHabitacionByPrecioAsync_ValidPrice_ReturnsRooms()
        {
            // Arrange
            using var context = CreateContext();
            var tarifa = new Tarifas {
                PrecioPorNoche = 200,
                FechaInicio = DateTime.Today.AddDays(-1),
                FechaFin = DateTime.Today.AddDays(1),
                Estado = true,
                IdCategoria = 1
            };
            context.Tarifas.Add(tarifa);
            context.Habitaciones.Add(new Habitacion {
                IdCategoria = 1,
                Estado = true
            });
            await context.SaveChangesAsync();
            
            var repo = CreateRepository(context);

            // Act
            var result = await repo.GetHabitacionByPrecioAsync(200);

            // Assert
            Assert.Single(result.Data as List<Habitacion>);
        }

        [Fact]
        public async Task GetHabitacionByPrecioAsync_NoActiveTarifas_ReturnsError()
        {
            // Arrange
            using var context = CreateContext();
            var repo = CreateRepository(context);

            // Act
            var result = await repo.GetHabitacionByPrecioAsync(200);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se encontraron tarifas vigentes", result.Message);
        }


        [Fact]
        public async Task UpdateEntityAsync_MaxPriceValue_HandlesCorrectly()
        {
            // Arrange
            using var context = CreateContext();
    
            var tarifa = new Tarifas { 
                PrecioPorNoche = decimal.MaxValue,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(10),
                Estado = true,
            };
    
            context.Tarifas.Add(tarifa);
            await context.SaveChangesAsync();
    
            var tarifaId = tarifa.IdTarifa;
    
            context.ChangeTracker.Clear();
    
            var retrievedTarifa = await context.Tarifas.FindAsync(tarifaId);
    
            var repo = CreateRepository(context);
            _mockValidator.Setup(v => v.Validate(It.IsAny<Tarifas>()))
                .Returns(new OperationResult { IsSuccess = true });
    
            retrievedTarifa.PrecioPorNoche = decimal.MaxValue - 1;

            // Act
            var result = await repo.UpdateEntityAsync(retrievedTarifa);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetTarifasVigentesAsync_CrossYearDate_ReturnsCorrect()
        {
            // Arrange
            using var context = CreateContext();
            context.Tarifas.Add(new Tarifas {
                FechaInicio = new DateTime(2022, 12, 20),
                FechaFin = new DateTime(2023, 1, 10),
                Estado = true
            });
            await context.SaveChangesAsync();
            
            var repo = CreateRepository(context);

            // Act
            var result = await repo.GetTarifasVigentesAsync("2023-01-01");

            // Assert
            Assert.Single(result.Data as List<Tarifas>);
        }
    }
}