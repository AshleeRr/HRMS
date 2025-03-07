using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class HabitacionRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HRMSContext _context;
        private readonly IHabitacionRepository _repository;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IValidator<Habitacion>> _mockValidator;
        private readonly Mock<ILogger<HabitacionRepository>> _mockLogger;

        public HabitacionRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var options = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HRMSContext(options);
            _mockConfig = new Mock<IConfiguration>();
            _mockValidator = new Mock<IValidator<Habitacion>>();
            _mockLogger = new Mock<ILogger<HabitacionRepository>>();

            _repository = new HabitacionRepository(
                _context, _mockConfig.Object, _mockValidator.Object, _mockLogger.Object);

            SeedRequiredEntities();
        }

        private void SeedRequiredEntities()
        {
            // Seed essential reference data
            _context.Pisos.AddRange(new[]
            {
                new Piso { IdPiso = 1, Estado = true },
                new Piso { IdPiso = 2, Estado = false },  // Inactive floor
                new Piso { IdPiso = 3, Estado = true }    // Agregar piso 3 activo
            });

            _context.Categorias.AddRange(new[]
            {
                new Categoria { IdCategoria = 1, Descripcion = "Standard", Estado = true },
                new Categoria { IdCategoria = 2, Descripcion = "Deluxe", Estado = false }  // Inactive category
            });

            _context.EstadoHabitaciones.AddRange(new[]
            {
                new EstadoHabitacion { IdEstadoHabitacion = 1, Estado = true },
                new EstadoHabitacion { IdEstadoHabitacion = 2, Estado = false }  // Inactive state
            });

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsOnlyActiveRooms()
        {
            // Arrange
            _context.Habitaciones.AddRange(
                new Habitacion { Estado = true },
                new Habitacion { Estado = false });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoActiveRoomsExist_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task GetEntityByIdAsync_WithInvalidIds_ReturnsNull(int invalidId)
        {
            // Act
            var result = await _repository.GetEntityByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEntityByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetEntityByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1, 2, true)]   
        [InlineData(2, 0, false)]   
        public async Task GetByPisoAsync_WithDifferentFloorStates_ReturnsAppropriateResults(
            int pisoId, int expectedCount, bool successExpected)
        {
            // Arrange
            _context.Habitaciones.Add(new Habitacion { IdPiso = 1, Estado = true });
            _context.Habitaciones.Add(new Habitacion { IdPiso = 1, Estado = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPisoAsync(pisoId);

            // Assert
            Assert.Equal(successExpected, result.IsSuccess);
            if (successExpected)
            {
                Assert.Equal(expectedCount, (result.Data as List<Habitacion>).Count);
            }
            else
            {
                Assert.Contains($"No se encontraron habitaciones en el piso {pisoId}", result.Message);
            }
        }

        [Theory]
        [InlineData("standard", 1)]  // Lowercase
        [InlineData("STANDARD", 1)]  // Uppercase
        [InlineData("StAnDaRd", 1)]  // Mixed case
        public async Task GetByCategoriaAsync_WithCaseVariations_ReturnsMatches(string searchTerm, int expectedCount)
        {
            // Arrange
            _context.Habitaciones.Add(new Habitacion 
            { 
                IdCategoria = 1, 
                Estado = true,
                IdPiso = 1,
                IdEstadoHabitacion = 1
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCategoriaAsync(searchTerm);

            // Assert
            Assert.Equal(expectedCount, (result.Data as List<Habitacion>)?.Count);
        }

        [Fact]
        public async Task GetByCategoriaAsync_WithInactiveCategory_ReturnsEmptyList()
        {
            // Arrange
            _context.Habitaciones.Add(new Habitacion 
            { 
                IdCategoria = 2,  // Inactive category
                Estado = true,
                IdPiso = 1,
                IdEstadoHabitacion = 1
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCategoriaAsync("Deluxe");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se encontraron", result.Message);
            
        }

        [Fact]
        public async Task SaveEntityAsync_WhenValidatorFails_ReturnsValidationError()
        {
            // Arrange
            var habitacion = new Habitacion();
            _mockValidator.Setup(v => v.Validate(It.IsAny<Habitacion>()))
                .Returns(new OperationResult{IsSuccess = false, Message = "Validation failed"});

            // Act
            var result = await _repository.SaveEntityAsync(habitacion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Validation failed", result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_WithInactiveForeignKey_ReturnsError()
        {
            // Arrange
            var inactivePiso = await _context.Pisos.FindAsync(2);
            Assert.NotNull(inactivePiso);
            Assert.False(inactivePiso.Estado, "El piso 2 debería estar inactivo");
    
            _mockValidator.Setup(v => v.Validate(It.IsAny<Habitacion>()))
                .Returns(new OperationResult { IsSuccess = true });
    
            var habitacion = new Habitacion
            {
                Numero = "201",      
                Detalle = "Test Room", 
                Precio = 100,          
                IdPiso = 2,            
                IdCategoria = 1,
                IdEstadoHabitacion = 1,
                Estado = true
            };

            // Act
            var result = await _repository.SaveEntityAsync(habitacion);

            // Assert
            Assert.False(result.IsSuccess);
            _testOutputHelper.WriteLine($"Mensaje de error: {result.Message}");
    
            // Usar una aseveración más flexible
            Assert.Contains("inactivo", result.Message.ToLower());
        }
        
        [Fact]
        public async Task GetInfoHabitacionesAsync_WhenNoActiveTariffs_ReturnsFallbackMessage()
        {
            // Arrange
            _context.Habitaciones.Add(new Habitacion { Estado = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetInfoHabitacionesAsync();

            // Assert
            Assert.True(result.IsSuccess);
    
            var json = System.Text.Json.JsonSerializer.Serialize(result.Data);
            Assert.Contains("No hay tarifas vigentes", json);
        }
        [Fact]
        public async Task GetInfoHabitacionesAsync_WhenNoServicesAssociated_ReturnsFallbackServiceInfo()
        {
            // Arrange
            var categoria = _context.Categorias.First(c => c.Estado == true); // Usar una categoría activa
    
            _context.Habitaciones.Add(new Habitacion { 
                Estado = true,
                IdCategoria = categoria.IdCategoria,
                IdPiso = 1,
                IdEstadoHabitacion = 1
            });
    
            _context.Tarifas.Add(new Tarifas
            { 
                IdCategoria = categoria.IdCategoria,
                FechaInicio = DateTime.Today.AddDays(-1),
                FechaFin = DateTime.Today.AddDays(1)
            });
    
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetInfoHabitacionesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            var json = System.Text.Json.JsonSerializer.Serialize(result.Data);
            Assert.Contains("Sin servicio", json);
        }
    }
}