using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class HabitacionRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<ILogger<HabitacionRepository>> _loggerMock;
        private readonly Mock<IValidator<Habitacion>> _validatorMock;

        public HabitacionRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _loggerMock = new Mock<ILogger<HabitacionRepository>>();
            _validatorMock = new Mock<IValidator<Habitacion>>();
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
                var repo = new HabitacionRepository(context, _loggerMock.Object , _validatorMock.Object);

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
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetByPisoAsync(0);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("El ID de piso no puede ser menor o igual a cero", result.Message);
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
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

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
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetByCategoriaAsync("");

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("categoría", result.Message);
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
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

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
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetInfoHabitacionesAsync();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
                _testOutputHelper.WriteLine($"JSON: {json}");

                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                Assert.NotNull(deserialized.mensaje);
                string mensaje = deserialized.mensaje.ToString();
                Assert.Contains("No se encontraron habitaciones", mensaje);
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
                var categoria = new Categoria { IdCategoria = 1, IdServicio = 1, Estado = true, Descripcion = "Premium" };
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
                    IdPiso = 1,
                    Numero = "101"
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
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetInfoHabitacionesAsync();

                // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);

                // Verificar que los datos tengan la estructura esperada
                var list = result.Data as IEnumerable<object>;
                Assert.NotEmpty(list);

                var firstItem = list.First();
                var properties = firstItem.GetType().GetProperties();
                
                // Imprimir propiedades para depuración
                _testOutputHelper.WriteLine("Properties of result data:");
                foreach (var prop in properties)
                {
                    _testOutputHelper.WriteLine($"{prop.Name}: {prop.GetValue(firstItem)}");
                }

                // Verificar que estén presentes las propiedades clave
                Assert.NotNull(properties.FirstOrDefault(p => p.Name == "IdHabitacion"));
                Assert.NotNull(properties.FirstOrDefault(p => p.Name == "Numero"));
                Assert.NotNull(properties.FirstOrDefault(p => p.Name == "PrecioPorNoche"));
                Assert.NotNull(properties.FirstOrDefault(p => p.Name == "NombreServicio"));
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ValidData_SavesHabitacion()
        {
            // Arrange
            var habitacion = new Habitacion { 
                IdPiso = 1, 
                IdCategoria = 1, 
                IdEstadoHabitacion = 1,
                Numero = "101",
                Detalle = "Habitación Standard",
                Precio = 100.0m 
            };

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.SaveEntityAsync(habitacion);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Contains("exitosa", result.Message);
                Assert.NotNull(await context.Habitaciones.FirstOrDefaultAsync(h => h.Numero == "101"));
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_NonExistentHabitacion_ReturnsFailure()
        {
            // Arrange
            var habitacion = new Habitacion { IdHabitacion = 999 };

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(habitacion);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("no existe", result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ExistingHabitacion_UpdatesCorrectly()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Habitaciones.Add(new Habitacion { 
                    IdHabitacion = 1, 
                    Numero = "101", 
                    Detalle = "Viejo detalle",
                    Estado = true
                });
                await context.SaveChangesAsync();
            }

            var updated = new Habitacion { 
                IdHabitacion = 1, 
                Numero = "101", 
                Detalle = "Nuevo detalle",
                Estado = true
            };

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(updated);

                // Assert
                Assert.True(result.IsSuccess);
                var habitacion = await context.Habitaciones.FindAsync(1);
                Assert.Equal("Nuevo detalle", habitacion.Detalle);
            }
        }

        [Fact]
        public async Task GetByNumeroAsync_ValidNumero_ReturnsHabitacion()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Habitaciones.Add(new Habitacion { 
                    IdHabitacion = 1, 
                    Numero = "A-101", 
                    Estado = true 
                });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetByNumeroAsync("A-101");

                // Assert
                Assert.True(result.IsSuccess);
                var habitacion = result.Data as Habitacion;
                Assert.NotNull(habitacion);
                Assert.Equal("A-101", habitacion.Numero);
            }
        }

        [Fact]
        public async Task GetByNumeroAsync_EmptyNumero_ReturnsFailure()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new HabitacionRepository(context, _loggerMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetByNumeroAsync("");

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("vacío", result.Message);
            }
        }
    }
}