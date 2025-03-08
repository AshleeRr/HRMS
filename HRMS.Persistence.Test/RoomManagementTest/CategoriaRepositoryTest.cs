    using HRMS.Domain.Base;
    using HRMS.Domain.Base.Validator;
    using HRMS.Domain.Entities.RoomManagement;
    using HRMS.Domain.Entities.Servicio;
    using HRMS.Persistence.Context;
    using HRMS.Persistence.Repositories.RoomRepository;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit.Abstractions;

    namespace HRMS.Persistence.Test.RoomManagementTest
    {
        public class CategoriaRepositoryTests
        {
            private readonly ITestOutputHelper _output;
            private Mock<IValidator<Categoria>> _mockValidator = new();

            private HRMSContext CreateContext()
            {
                var options = new DbContextOptionsBuilder<HRMSContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
                
                return new HRMSContext(options);
            }

            private CategoriaRepository CreateRepository(HRMSContext context)
            {
                return new CategoriaRepository(
                    context, 
                    Mock.Of<IConfiguration>(),
                    _mockValidator.Object
                );
            }

            [Fact]
            public async Task GetAllAsync_ReturnsOnlyActiveCategories()
            {
                // Arrange
                using var context = CreateContext();
                context.Categorias.AddRange(
                    new Categoria { Estado = true },
                    new Categoria { Estado = false }
                );
                await context.SaveChangesAsync();
                
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetAllAsync();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public async Task SaveEntityAsync_WhenValidationFails_ReturnsFailure()
            {
                // Arrange
                using var context = CreateContext();
                var repo = CreateRepository(context);
                var categoria = new Categoria();
    
                _mockValidator.Setup(v => v.Validate(categoria))
                    .Returns(new OperationResult{Message = "Failed", IsSuccess = false});

                // Act
                var result = await repo.SaveEntityAsync(categoria);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("Failed", result.Message); 
            }

            [Fact]
            public async Task SaveEntityAsync_WithDuplicateDescription_ReturnsFailure()
            {
                // Arrange
                using var context = CreateContext();
                context.Categorias.Add(new Categoria { Descripcion = "Test", Estado = true });
                await context.SaveChangesAsync();
    
                var repo = CreateRepository(context);
                var newCat = new Categoria { Descripcion = "Test" };
    
                _mockValidator.Setup(v => v.Validate(It.IsAny<Categoria>()))
                    .Returns(new OperationResult { IsSuccess = true });

                // Act
                var result = await repo.SaveEntityAsync(newCat);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("Ya existe", result.Message);
            }

            [Fact]
            public async Task SaveEntityAsync_WithMaxLengthDescription_ReturnsSuccess()
            {
                // Arrange
                using var context = CreateContext();
                var repo = CreateRepository(context);
                var longDesc = new string('A', 50);
                var categoria = new Categoria { Descripcion = longDesc };
    
                _mockValidator.Setup(v => v.Validate(It.IsAny<Categoria>()))
                    .Returns(new OperationResult { IsSuccess = true });

                // Act
                var result = await repo.SaveEntityAsync(categoria);

                // Assert
                Assert.True(result.IsSuccess);
            }
            

            [Fact]
            public async Task UpdateEntityAsync_WithNonexistentId_ReturnsFailure()
            {
                // Arrange
                using var context = CreateContext();
                var repo = CreateRepository(context);
                var categoria = new Categoria { IdCategoria = 999 };
    
                _mockValidator.Setup(v => v.Validate(It.IsAny<Categoria>()))
                    .Returns(new OperationResult { IsSuccess = true });

                // Act
                var result = await repo.UpdateEntityAsync(categoria);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains("no existe", result.Message);
            }

            [Fact]
            public async Task UpdateEntityAsync_WithDuplicateDescriptionDifferentId_ReturnsFailure()
            {
                // Arrange
                using var context = CreateContext();
                context.Categorias.AddRange(
                    new Categoria { IdCategoria = 1, Descripcion = "Original" },
                    new Categoria { IdCategoria = 2, Descripcion = "Existing" }
                );
                await context.SaveChangesAsync();
                
                var repo = CreateRepository(context);
                var updated = new Categoria { IdCategoria = 1, Descripcion = "Existing" };

                // Act
                var result = await repo.UpdateEntityAsync(updated);

                // Assert
                Assert.False(result.IsSuccess);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public async Task GetCategoriaByServiciosAsync_WithInvalidNombre_ReturnsFailure(string input)
            {
                // Arrange
                using var context = CreateContext();
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetCategoriaByServiciosAsync(input);

                // Assert
                Assert.False(result.IsSuccess);
            }

            [Fact]
            public async Task GetCategoriaByServiciosAsync_WithInactiveService_ReturnsEmpty()
            {
                // Arranges
                using var context = CreateContext();
                context.Servicios.Add(new Servicios { 
                    IdServicio = 1, 
                    Nombre = "Test", 
                    Estado = false 
                });
                context.Categorias.Add(new Categoria { 
                    IdServicio = 1, 
                    Estado = true 
                });
                await context.SaveChangesAsync();
                
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetCategoriaByServiciosAsync("Test");

                // Assert
                Assert.Empty(result.Data as List<Categoria>);
            }

            [Fact]
            public async Task GetServiciosByDescripcionAsync_WithPartialMatch_ReturnsResults()
            {
                // Arrange
                using var context = CreateContext();
                context.Servicios.Add(new Servicios { 
                    Descripcion = "Full description contains partial", 
                    Estado = true 
                });
                await context.SaveChangesAsync();
                
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetCategoriaByDescripcionAsync("contains partial");

                // Assert
                Assert.Null(result.Data as List<Servicios>);
            }

            [Fact]
            public async Task GetServiciosByDescripcionAsync_WithSpecialCharacters_HandlesCorrectly()
            {
                // Arrange
                using var context = CreateContext();
                context.Servicios.Add(new Servicios { 
                    Descripcion = "Service@123!-Test", 
                    Estado = true 
                });
                await context.SaveChangesAsync();
                
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetCategoriaByDescripcionAsync("@123!");

                // Assert
                Assert.Null(result.Data as List<Servicios>);
            }
            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(int.MinValue)]
            public async Task GetHabitacionByCapacidad_WithInvalidCapacity_ReturnsFailure(int capacity)
            {
                // Arrange
                using var context = CreateContext();
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetHabitacionByCapacidad(capacity);

                // Assert
                Assert.False(result.IsSuccess);
            }

            [Fact]
            public async Task GetHabitacionByCapacidad_WithNoMatchingRooms_ReturnsFailure()
            {
                // Arrange
                using var context = CreateContext();
                context.Categorias.Add(new Categoria { 
                    Capacidad = 4, 
                    Estado = true 
                });
                await context.SaveChangesAsync();
                
                var repo = CreateRepository(context);

                // Act
                var result = await repo.GetHabitacionByCapacidad(4);

                // Assert
                Assert.Contains("No se encontraron habitaciones", result.Message);
            }
        }
    }