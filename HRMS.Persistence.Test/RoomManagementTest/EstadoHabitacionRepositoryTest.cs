using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class EstadoHabitacionRepositoryTests
    {
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<ILogger<EstadoHabitacionRepository>> _loggerMock;
        private readonly Mock<IValidator<EstadoHabitacion>> _validatorMock;

        public EstadoHabitacionRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _loggerMock = new Mock<ILogger<EstadoHabitacionRepository>>();
            _validatorMock = new Mock<IValidator<EstadoHabitacion>>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveEstados()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.EstadoHabitaciones.AddRange(
                    new EstadoHabitacion { IdEstadoHabitacion = 1, Estado = true },
                    new EstadoHabitacion { IdEstadoHabitacion = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object , _validatorMock.Object);

                // Act
                var result = await repo.GetAllAsync();

                // Assert
                Assert.Single(result);
                Assert.All(result, e => Assert.True(e.Estado));
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_InvalidId_ReturnsNull()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object ,_validatorMock.Object );
                
                // Act
                var result = await repo.GetEntityByIdAsync(0);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_ValidId_ReturnsEntity()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.EstadoHabitaciones.Add(new EstadoHabitacion { IdEstadoHabitacion = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetEntityByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.IdEstadoHabitacion);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ValidEntity_SavesSuccessfully()
        {
            // Arrange
            var estado = new EstadoHabitacion { Descripcion = "Disponible" };

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.SaveEntityAsync(estado);

                // Assert
                Assert.True(result.IsSuccess);
                var savedEstado = await context.EstadoHabitaciones.FirstOrDefaultAsync(e => e.Descripcion == "Disponible");
                Assert.NotNull(savedEstado);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ExistingNotFound_ReturnsFailure()
        {
            // Arrange
            var estado = new EstadoHabitacion { IdEstadoHabitacion = 99 };

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(estado);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("no existe", result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ValidUpdate_ModifiesEstado()
        {
            // Arrange
            var original = new EstadoHabitacion { IdEstadoHabitacion = 1, Descripcion = "Antiguo" };
            using (var context = new HRMSContext(_dbOptions))
            {
                context.EstadoHabitaciones.Add(original);
                await context.SaveChangesAsync();
            }

            var updated = new EstadoHabitacion { IdEstadoHabitacion = 1, Descripcion = "Nuevo" };

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(updated);

                // Assert
                Assert.True(result.IsSuccess);
                var modified = await context.EstadoHabitaciones.FindAsync(1);
                Assert.Equal("Nuevo", modified.Descripcion);
            }
        }
        
        [Fact]
        public async Task GetEstadoByDescripcionAsync_NoMatches_ReturnsEmptyList()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetEstadoByDescripcionAsync("NoExiste");

                // Assert
                Assert.True(result.IsSuccess); // Ahora siempre devuelve success, posiblemente con lista vacía
                Assert.Empty((List<EstadoHabitacion>)result.Data);
            }
        }

        [Fact]
        public async Task GetEstadoByDescripcionAsync_MatchingDescripcion_ReturnsEstados()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.EstadoHabitaciones.AddRange(
                    new EstadoHabitacion { Descripcion = "Mantenimiento", Estado = true },
                    new EstadoHabitacion { Descripcion = "Limpieza", Estado = true }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new EstadoHabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetEstadoByDescripcionAsync("teni");

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<EstadoHabitacion>)result.Data);
            }
        }
    }
}