using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class HabitacionRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<IValidator<Habitacion>> _validatorMock;
        private readonly Mock<ILogger<HabitacionRepository>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;

        public HabitacionRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _validatorMock = new Mock<IValidator<Habitacion>>();
            _loggerMock = new Mock<ILogger<HabitacionRepository>>();
            _configMock = new Mock<IConfiguration>();
        }


        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveHabitaciones()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Habitaciones.AddRange(
                    new Habitacion { IdHabitacion = 1, Estado = true },
                    new Habitacion { IdHabitacion = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetAllAsync();

                // Assert
                Assert.Single(result);
                Assert.All(result, h => Assert.True(h.Estado));
            }
        }

        [Fact]
        public async Task GetByPisoAsync_InvalidPisoId_ReturnsFailure()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetByPisoAsync(0);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("mayor que cero", result.Message);
            }
        }

        [Fact]
        public async Task GetByPisoAsync_ValidPiso_ReturnsHabitaciones()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Habitaciones.Add(new Habitacion { IdHabitacion = 1, IdPiso = 5, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetByPisoAsync(5);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Habitacion>)result.Data);
            }
        }

        [Fact]
        public async Task GetByCategoriaAsync_EmptyCategoria_ReturnsFailure()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetByCategoriaAsync("");

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("ingresar una categoría", result.Message);
            }
        }

        [Fact]
        public async Task GetByCategoriaAsync_MatchingCategoria_ReturnsHabitaciones()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                var categoria = new Categoria { IdCategoria = 1, Descripcion = "Premium", Estado = true };
                context.Categorias.Add(categoria);
                context.Habitaciones.Add(new Habitacion { IdCategoria = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetByCategoriaAsync("Prem");

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Habitacion>)result.Data);
            }
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_NoActiveHabitaciones_ReturnsCustomMessage()
        {

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetInfoHabitacionesAsync();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);

                var dataType = result.Data.GetType();
                _testOutputHelper.WriteLine($"Data type: {dataType.FullName}");

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
                _testOutputHelper.WriteLine($"JSON: {json}");

                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                Assert.NotNull(deserialized.mensaje);
                string mensaje = deserialized.mensaje.ToString();
                Assert.Equal("No hay habitaciones activas en la base de datos", mensaje);
            }
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_NoTarifasVigentes_ReturnsCorrectMessage()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                // Seed habitaciones but no tarifas
                var servicio = new Servicios { IdServicio = 1, Nombre = "SPA", Descripcion = "Servicio completo" };
                var categoria = new Categoria { IdCategoria = 1, IdServicio = 1, Estado = true };
                var habitacion = new Habitacion
                {
                    IdHabitacion = 1,
                    IdCategoria = 1,
                    Estado = true,
                    IdPiso = 1
                };
                var piso = new Piso
                {
                    IdPiso = 1,
                    Descripcion = "Primer Piso"
                };

                // Note: Not adding any tarifas
                context.AddRange(servicio, categoria, habitacion, piso);
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetInfoHabitacionesAsync();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);

                // Use JSON serialization as a workaround for type issues
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                Assert.NotNull(deserialized.mensaje);
                string mensaje = deserialized.mensaje.ToString();
                Assert.Equal("No hay tarifas vigentes para la fecha actual", mensaje);
            }
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_NoCategoriasConServicios_ReturnsCorrectMessage()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                // Create piso and habitacion
                var piso = new Piso
                {
                    IdPiso = 1,
                    Descripcion = "Primer Piso"
                };

                // Create categoria without IdServicio
                var categoria = new Categoria
                {
                    IdCategoria = 1,
                    Estado = true
                    // Not setting IdServicio
                };

                var habitacion = new Habitacion
                {
                    IdHabitacion = 1,
                    IdCategoria = 1,
                    Estado = true,
                    IdPiso = 1
                };

                // Add tarifa with current dates
                var tarifa = new Tarifas
                {
                    IdTarifa = 1,
                    IdCategoria = 1,
                    FechaInicio = DateTime.Now.AddDays(-1),
                    FechaFin = DateTime.Now.AddDays(1),
                    PrecioPorNoche = 150,
                    Estado = true
                };

                context.AddRange(piso, categoria, habitacion, tarifa);
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetInfoHabitacionesAsync();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);

                // Use JSON serialization as a workaround for type issues
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                Assert.NotNull(deserialized.mensaje);
                string mensaje = deserialized.mensaje.ToString();
                Assert.Equal("No hay categorías con servicios asociados", mensaje);
            }
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_WithValidData_ReturnsCompleteInfo()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                // Seed data
                var servicio = new Servicios { IdServicio = 1, Nombre = "SPA", Descripcion = "Servicio completo" };
                var categoria = new Categoria { IdCategoria = 1, IdServicio = 1, Estado = true };
                var tarifa = new Tarifas
                {
                    IdTarifa = 1, IdCategoria = 1, FechaInicio = DateTime.Now.AddDays(-1),
                    FechaFin = DateTime.Now.AddDays(1), PrecioPorNoche = 150, Estado = true
                };
                var habitacion = new Habitacion
                {
                    IdHabitacion = 1,
                    IdCategoria = 1,
                    Estado = true,
                    IdPiso = 1
                };
                var piso = new Piso
                {
                    IdPiso = 1,
                    Descripcion = "Primer Piso"
                };

                context.AddRange(servicio, categoria, tarifa, habitacion, piso);
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.GetInfoHabitacionesAsync();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data); // Check if data is not null

                // Now safely access the data
                var data = result.Data;
                Assert.NotEmpty((IEnumerable<object>)data); // Make sure data has elements

                var list = data as IEnumerable<object>;
                var firstItem = list.First();

                // Print properties for debugging
                Console.WriteLine("Properties of result data:");
                foreach (var prop in firstItem.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(firstItem)}");
                }

                var price = firstItem.GetType().GetProperty("PrecioPorNoche").GetValue(firstItem);
                var serviceName = firstItem.GetType().GetProperty("NombreServicio").GetValue(firstItem);

                Assert.Equal(150m, price);
                Assert.Equal("SPA", serviceName);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_InvalidForeignKey_ReturnsFailure()
        {
            // Arrange
            var habitacion = new Habitacion { IdPiso = 99, IdCategoria = 99, IdEstadoHabitacion = 99 };
            _validatorMock.Setup(v => v.Validate(habitacion)).Returns(OperationResult.Success());

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.SaveEntityAsync(habitacion);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("no existe", result.Message);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ValidData_SavesHabitacion()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                // Required foreign keys
                context.Pisos.Add(new Piso { IdPiso = 1, Estado = true });
                context.Categorias.Add(new Categoria { IdCategoria = 1, Estado = true });
                context.EstadoHabitaciones.Add(new EstadoHabitacion { IdEstadoHabitacion = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            var habitacion = new Habitacion { IdPiso = 1, IdCategoria = 1, IdEstadoHabitacion = 1 };
            _validatorMock.Setup(v => v.Validate(habitacion)).Returns(OperationResult.Success());

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _configMock.Object, _validatorMock.Object,
                    _loggerMock.Object);

                // Act
                var result = await repo.SaveEntityAsync(habitacion);

                // Assert
                Assert.True(await context.Habitaciones.AnyAsync());
                Assert.Contains("exitosa", result.Message);
            }
        }
    }
}