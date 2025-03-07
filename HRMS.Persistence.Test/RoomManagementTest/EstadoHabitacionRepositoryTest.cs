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

namespace HRMS.Persistence.Test.RoomManagementTest;

public class EstadoHabitacionRepositoryTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<ILogger<EstadoHabitacionRepository>> _mockLogger = new();
    private readonly Mock<IValidator<EstadoHabitacion>> _mockValidator = new();

    public EstadoHabitacionRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    private HRMSContext CreateContext(string testName = null)
    {
        string dbName = testName ?? Guid.NewGuid().ToString();
    
        var options = new DbContextOptionsBuilder<HRMSContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
            
        return new HRMSContext(options);
    }
    private EstadoHabitacionRepository CreateRepository(HRMSContext context)
    {
        var logger = Mock.Of<ILogger<EstadoHabitacionRepository>>();
        var configuration = Mock.Of<IConfiguration>();
        var validatorMock = new Mock<IValidator<EstadoHabitacion>>();
    
        validatorMock
            .Setup(v => v.Validate(It.IsAny<EstadoHabitacion>()))
            .Returns((EstadoHabitacion estado) => {
                if (estado.Descripcion != null && estado.Descripcion.Length > 50)
                {
                    return new OperationResult 
                    { 
                        IsSuccess = false, 
                        Message = "La descripción no puede exceder los 50 caracteres."
                    };
                }
                return new OperationResult { IsSuccess = true };
            });
    
        return new EstadoHabitacionRepository(
            context, 
            logger, 
            configuration, 
            validatorMock.Object);
    }
    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveEstados()
    {
        // Arrange
        using var context = CreateContext();
        context.EstadoHabitaciones.AddRange(
            new EstadoHabitacion { Estado = true },
            new EstadoHabitacion { Estado = false }
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
    [InlineData(int.MinValue)]
    public async Task GetEntityByIdAsync_WithInvalidIds_ReturnsNull(int invalidId)
    {
        // Arrange
        using var context = CreateContext();
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetEntityByIdAsync(invalidId);

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task GetEntityByIdAsync_WithDeletedEntity_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var estado = new EstadoHabitacion { Estado = false };
        context.EstadoHabitaciones.Add(estado);
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetEntityByIdAsync(estado.IdEstadoHabitacion);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task SaveEntityAsync_WithDuplicateDescription_ReturnsFailure()
    {
        // Arrange
        using var context = CreateContext();
        context.EstadoHabitaciones.Add(new EstadoHabitacion { Descripcion = "Existente" });
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);
        var newEstado = new EstadoHabitacion { Descripcion = "Existente" };

        // Act
        var result = await repo.SaveEntityAsync(newEstado);

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
        var exactMaxLength = new string('A', 50);
        var estado = new EstadoHabitacion { Descripcion = exactMaxLength };

        // Act
        var result = await repo.SaveEntityAsync(estado);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SaveEntityAsync_WithExcessiveLength_ReturnsFailure()
    {
        // Arrange
        using var context = CreateContext();
        var repo = CreateRepository(context);
        var tooLong = new string('A', 51); 
        var estado = new EstadoHabitacion { Descripcion = tooLong };

        // Act
        var result = await repo.SaveEntityAsync(estado);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("50 caracteres", result.Message);
    }
    
    [Fact]
    public async Task SaveEntityAsync_WithInvalidValidator_ReturnsFailure()
    {
        // Arrange
        using var context = CreateContext();
    
        var validatorMock = new Mock<IValidator<EstadoHabitacion>>();
    
        validatorMock
            .Setup(v => v.Validate(It.IsAny<EstadoHabitacion>()))
            .Returns(new OperationResult { Message = "Validation error", IsSuccess = false });
    
        var loggerMock = Mock.Of<ILogger<EstadoHabitacionRepository>>();
        var configMock = Mock.Of<IConfiguration>();
    
        var repo = new EstadoHabitacionRepository(
            context, 
            loggerMock, 
            configMock, 
            validatorMock.Object);
    
        var estado = new EstadoHabitacion();

        // Act
        var result = await repo.SaveEntityAsync(estado);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Validation error", result.Message);
    }
    [Fact]
    public async Task UpdateEntityAsync_UpdateToSameDescription_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateContext();
        var estado = new EstadoHabitacion { Descripcion = "Original" };
        context.EstadoHabitaciones.Add(estado);
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);
        estado.Descripcion = "Original"; 

        // Act
        var result = await repo.UpdateEntityAsync(estado);

        // Assert
        Assert.True(result.IsSuccess);
    }
    [Fact]
    public async Task UpdateEntityAsync_ReactivateEstado_UpdatesCorrectly()
    {
        // Arrange
        using var context = CreateContext();
        var estado = new EstadoHabitacion { Estado = false };
        context.EstadoHabitaciones.Add(estado);
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);
        estado.Estado = true;

        // Act
        var result = await repo.UpdateEntityAsync(estado);
        var updated = await repo.GetEntityByIdAsync(estado.IdEstadoHabitacion);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(updated?.Estado);
    }
}