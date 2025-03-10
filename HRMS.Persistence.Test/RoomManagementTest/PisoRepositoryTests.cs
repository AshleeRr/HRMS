using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class PisoRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<IValidator<Piso>> _validatorMock;
        private readonly Mock<ILogger<PisoRepository>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;

        public PisoRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _validatorMock = new Mock<IValidator<Piso>>();
            _loggerMock = new Mock<ILogger<PisoRepository>>();
            _configMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActivePisos()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.AddRange(
                    new Piso { IdPiso = 1, Estado = true },
                    new Piso { IdPiso = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetAllAsync();

                // Assert
                Assert.Single(result);
                Assert.All(result, p => Assert.True(p.Estado));
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_InvalidId_ReturnsNull()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);
                
                // Act
                var result = await repo.GetEntityByIdAsync(0);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_ValidId_ReturnsPiso()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(new Piso { IdPiso = 1, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetEntityByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.IdPiso);
            }
        }

        [Fact]
        public async Task GetPisoByDescripcion_EmptyDescripcion_ReturnsFailure()
        {
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetPisoByDescripcion("");

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("vacía", result.Message);
            }
        }

        [Fact]
        public async Task GetPisoByDescripcion_MatchingDescripcion_ReturnsPisos()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(new Piso { Descripcion = "Piso Ejecutivo", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.GetPisoByDescripcion("Ejec");

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Piso>)result.Data);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_DuplicateDescripcion_ReturnsFailure()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                // Create two pisos
                context.Pisos.Add(new Piso { IdPiso = 1, Descripcion = "Original", Estado = true });
                context.Pisos.Add(new Piso { IdPiso = 2, Descripcion = "Different", Estado = true });
                await context.SaveChangesAsync();
            }

            var updatedPiso = new Piso { IdPiso = 2, Descripcion = "Original" };
            _validatorMock.Setup(v => v.Validate(updatedPiso)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(updatedPiso);

                // Assert
                Assert.False(result.IsSuccess);
        
                _testOutputHelper.WriteLine($"Error message: {result.Message}");
                Assert.Contains("existe", result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ValidUpdate_ModifiesPiso()
        {
            // Arrange
            var original = new Piso { IdPiso = 1, Descripcion = "Antiguo", Estado = true };
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(original);
                await context.SaveChangesAsync();
            }

            var updated = new Piso { IdPiso = 1, Descripcion = "Nuevo", Estado = false };
            _validatorMock.Setup(v => v.Validate(updated)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                // Act
                var result = await repo.UpdateEntityAsync(updated);

                // Assert
                var modified = await context.Pisos.FindAsync(1);
                Assert.Equal("Nuevo", modified.Descripcion);
                Assert.False(modified.Estado);
            }
        }
    }
}