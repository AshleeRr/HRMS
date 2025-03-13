using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace HRMS.Persistence.Test.RoomManagementTest
{
    public class PisoRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly DbContextOptions<HRMSContext> _dbOptions;

        public PisoRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
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
                var repo = new PisoRepository(context);

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
                var repo = new PisoRepository(context);
                
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
                var repo = new PisoRepository(context);

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
                var repo = new PisoRepository(context);

                // Act
                var result = await repo.GetPisoByDescripcion("");

                // Assert
                Assert.False(result.IsSuccess);
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
                var repo = new PisoRepository(context);

                // Act
                var result = await repo.GetPisoByDescripcion("Ejec");

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Single((List<Piso>)result.Data);
            }
        }

        [Fact]
        public async Task GetPisoByDescripcion_NoMatchingDescripcion_ReturnsFailure()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(new Piso { Descripcion = "Piso Ejecutivo", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context);

                // Act
                var result = await repo.GetPisoByDescripcion("Inexistente");

                // Assert
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public async Task ExistsByDescripcionAsync_ExistingDescripcion_ReturnsTrue()
        {
            // Arrange
            string descripcion = "Único";
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(new Piso { IdPiso = 1, Descripcion = descripcion, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context);

                // Act
                var result = await repo.ExistsByDescripcionAsync(descripcion);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task ExistsByDescripcionAsync_NonExistingDescripcion_ReturnsFalse()
        {
            // Arrange
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(new Piso { IdPiso = 1, Descripcion = "Existente", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context);

                // Act
                var result = await repo.ExistsByDescripcionAsync("Inexistente");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task ExistsByDescripcionAsync_ExcludingCurrentId_ReturnsFalse()
        {
            // Arrange
            string descripcion = "Único";
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Pisos.Add(new Piso { IdPiso = 1, Descripcion = descripcion, Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new PisoRepository(context);

                // Act
                var result = await repo.ExistsByDescripcionAsync(descripcion, 1);

                // Assert
                Assert.False(result);
            }
        }
    }
}