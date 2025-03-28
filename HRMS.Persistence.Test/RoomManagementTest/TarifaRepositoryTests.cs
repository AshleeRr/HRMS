using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class TarifaRepositoryTests
    {
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<IValidator<Tarifas>> _validatorMock;
        private readonly Mock<ILogger<TarifaRepository>> _loggerMock;

        public TarifaRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _validatorMock = new Mock<IValidator<Tarifas>>();
            _loggerMock = new Mock<ILogger<TarifaRepository>>();
        }

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveTarifas()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Tarifas.AddRange(
                    new Tarifas { IdTarifa = 1, Estado = true },
                    new Tarifas { IdTarifa = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new TarifaRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetAllAsync();

                // Assert
                Assert.Single(result);
                Assert.All(result, t => Assert.True(t.Estado));
            }
        }
        #endregion

        #region GetTarifasVigentesAsync
        [Fact]
        public async Task GetTarifasVigentesAsync_InvalidDate_ReturnsFailure()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new TarifaRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetTarifasVigentesAsync("invalid-date");

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("Formato de fecha no válido.", result.Message);
            }
        }

        [Fact]
        public async Task GetTarifasVigentesAsync_ValidDate_ReturnsVigentes()
        {
            // Arrange
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Tarifas.AddRange(
                    new Tarifas { 
                        FechaInicio = DateTime.Today.AddDays(-5), 
                        FechaFin = DateTime.Today.AddDays(5), 
                        Estado = true 
                    },
                    new Tarifas { 
                        FechaInicio = DateTime.Today.AddDays(-10), 
                        FechaFin = DateTime.Today.AddDays(-5), 
                        Estado = true 
                    }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new TarifaRepository(context, _loggerMock.Object,  _validatorMock.Object);

                // Act
                var result = await repo.GetTarifasVigentesAsync(today);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Tarifas>)result.Data);
            }
        }
        #endregion

        #region GetHabitacionByPrecioAsync
        [Fact]
        public async Task GetHabitacionByPrecioAsync_InvalidPrecio_ReturnsFailure()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new TarifaRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetHabitacionByPrecioAsync(-100);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("El precio debe ser mayor que cero", result.Message);
            }
        }

        [Fact]
        public async Task GetHabitacionByPrecioAsync_MatchingPrecio_ReturnsHabitaciones()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                var tarifa = new Tarifas { 
                    IdTarifa = 1, 
                    PrecioPorNoche = 150, 
                    Estado = true,
                    IdCategoria = 1, 
                    FechaInicio = DateTime.Now.AddDays(-1), 
                    FechaFin = DateTime.Now.AddDays(10)      
                };
                var categoria = new Categoria { 
                    IdCategoria = 1, 
                    Estado = true 
                };
                var habitacion = new Habitacion { 
                    IdHabitacion = 1, 
                    IdCategoria = 1, 
                    Estado = true 
                };

                context.AddRange(tarifa, categoria, habitacion);
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new TarifaRepository(context, _loggerMock.Object,  _validatorMock.Object);

                // Act
                var result = await repo.GetHabitacionByPrecioAsync(150);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Habitacion>)result.Data);
            }
        }
        #endregion

        #region BaseMethods
        [Fact]
        public async Task GetEntityByIdAsync_ValidId_ReturnsTarifa()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Tarifas.Add(new Tarifas { IdTarifa = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new TarifaRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetEntityByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.IdTarifa);
            }
        }
        #endregion
    }
}