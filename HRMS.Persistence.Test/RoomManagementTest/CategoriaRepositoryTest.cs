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

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class CategoriaRepositoryTests
    {
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<IValidator<Categoria>> _validatorMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<CategoriaRepository>> _loggerMock;

        public CategoriaRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _validatorMock = new Mock<IValidator<Categoria>>();
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<CategoriaRepository>>();
        }
        
        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveCategorias()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Categorias.AddRange(
                    new Categoria { IdCategoria = 1, Estado = true },
                    new Categoria { IdCategoria = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.GetAllAsync();

                // Assert
                Assert.Single(result);
                Assert.All(result, c => Assert.True(c.Estado));
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ValidationFails_ReturnsFailure()
        {
            // Arrange
            var categoria = new Categoria { Descripcion = "Test" };
            _validatorMock.Setup(v => v.Validate(categoria))
                .Returns(new OperationResult { IsSuccess = false, Message = "Error validation" });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.SaveEntityAsync(categoria);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Equal("Error validation", result.Message);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_DuplicateDescripcion_ReturnsFailure()
        {
            // Arrange
            var existingCategoria = new Categoria { Descripcion = "Test", Estado = true };
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Categorias.Add(existingCategoria);
                await context.SaveChangesAsync();
            }

            var newCategoria = new Categoria { Descripcion = "Test" };
            _validatorMock.Setup(v => v.Validate(newCategoria))
                .Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.SaveEntityAsync(newCategoria);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("Ya existe una categoría", result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_CategoriaNotFound_ReturnsFailure()
        {
            // Arrange
            var categoria = new Categoria { IdCategoria = 99 };
            _validatorMock.Setup(v => v.Validate(categoria))
                .Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(categoria);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("no existe", result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ValidUpdate_ModifiesCategoria()
        {
            // Arrange
            var originalCategoria = new Categoria { IdCategoria = 1, Descripcion = "Old" };
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Categorias.Add(originalCategoria);
                await context.SaveChangesAsync();
            }

            var updatedCategoria = new Categoria { IdCategoria = 1, Descripcion = "New" };
            _validatorMock.Setup(v => v.Validate(updatedCategoria))
                .Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(updatedCategoria);

                // Assert
                var updated = await context.Categorias.FindAsync(1);
                Assert.Equal("New", updated.Descripcion);
            }
        }

        [Fact]
        public async Task GetCategoriaByServiciosAsync_ValidNombre_ReturnsCategorias()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Servicios.Add(new Servicios { IdServicio = 1, Nombre = "Limpieza", Estado = true });
                context.Categorias.Add(new Categoria { IdCategoria = 1, IdServicio = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.GetCategoriaByServiciosAsync("limpieza");

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Categoria>)result.Data);
            }
        }

        [Fact]
        public async Task GetHabitacionByCapacidad_InvalidCapacidad_ReturnsError()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.GetHabitacionByCapacidad(0);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("mayor que cero", result.Message);
            }
        }

        [Fact]
        public async Task GetHabitacionByCapacidad_ValidCapacidad_ReturnsHabitaciones()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Categorias.Add(new Categoria { IdCategoria = 1, Capacidad = 2, Estado = true });
                context.Habitaciones.Add(new Habitacion { IdCategoria = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new CategoriaRepository(context, _configMock.Object, _validatorMock.Object, _loggerMock.Object);

                // Act
                var result = await repo.GetHabitacionByCapacidad(2);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Habitacion>)result.Data);
            }
        }
    }
}