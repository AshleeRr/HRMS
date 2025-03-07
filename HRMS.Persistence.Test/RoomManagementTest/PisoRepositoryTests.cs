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

public class PisoRepositoryTests
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<ILogger<PisoRepository>> _mockLogger = new();
    private readonly Mock<IValidator<Piso>> _mockValidator = new();

    private HRMSContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HRMSContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        return new HRMSContext(options);
    }

    private PisoRepository CreateRepository(HRMSContext context)
    {
        var loggerMock = new Mock<ILogger<PisoRepository>>();
        var configurationMock = new Mock<IConfiguration>();
        var validatorMock = new Mock<IValidator<Piso>>();
    
        validatorMock.Setup(v => v.Validate(It.IsAny<Piso>()))
            .Returns(new OperationResult { IsSuccess = true });
    
        return new PisoRepository(
            context, 
            loggerMock.Object,
            configurationMock.Object,
            validatorMock.Object);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActivePisos()
    {
        // Arrange
        using var context = CreateContext();
        context.Pisos.AddRange(
            new Piso { Estado = true },
            new Piso { Estado = false }
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
    public async Task GetEntityByIdAsync_WithValidId_ReturnsPiso()
    {
        // Arrange
        using var context = CreateContext();
        var expected = new Piso { IdPiso = 1 };
        context.Pisos.Add(expected);
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetEntityByIdAsync(1);

        // Assert
        Assert.Equal(expected.IdPiso, result?.IdPiso);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetPisoByDescripcion_InvalidInput_ReturnsValidationError(string input)
    {
        // Arrange
        using var context = CreateContext();
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetPisoByDescripcion(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("no puede estar vacía", result.Message);
    }
    
    [Fact]
    public async Task GetPisoByDescripcion_CaseInsensitiveMatch_ReturnsResults()
    {
        // Arrange
        using var context = CreateContext();
        context.Pisos.Add(new Piso { 
            Descripcion = "Piso PRINCIPAL", 
            Estado = true 
        });
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetPisoByDescripcion("principal");

        // Assert
        Assert.Single(result.Data as List<Piso>);
    }
    [Fact]
    public async Task GetPisoByDescripcion_WithSpecialCharacters_ReturnsMatches()
    {
        // Arrange
        using var context = CreateContext();
        context.Pisos.Add(new Piso { 
            Descripcion = "Piso #2-A", 
            Estado = true 
        });
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetPisoByDescripcion("#2-A");

        // Assert
        Assert.Single(result.Data as List<Piso>);
    }
    [Fact]
    public async Task UpdateEntityAsync_ValidationFails_ReturnsFailure()
    {
        // Arrange
        using var context = CreateContext();
    
        var validatorMock = new Mock<IValidator<Piso>>();
        validatorMock.Setup(v => v.Validate(It.IsAny<Piso>()))
            .Returns(new OperationResult { Message = "Invalid data", IsSuccess = false });
    
        var loggerMock = new Mock<ILogger<PisoRepository>>();
        var configurationMock = new Mock<IConfiguration>();
        var repo = new PisoRepository(
            context,
            loggerMock.Object,
            configurationMock.Object,
            validatorMock.Object);
    
        var piso = new Piso { IdPiso = 1, Descripcion = "Test" };
    
        // Act
        var result = await repo.UpdateEntityAsync(piso);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid data", result.Message);
    }
    
    [Fact]
    public async Task UpdateEntityAsync_NonExistentPiso_ReturnsFailure()
    {
        // Arrange
        using var context = CreateContext();
        var repo = CreateRepository(context);
        var piso = new Piso { IdPiso = 999 , Descripcion = "Juan"};

        // Act
        var result = await repo.UpdateEntityAsync(piso);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("no existe", result.Message);
    }
    
    [Fact]
    public async Task UpdateEntityAsync_DuplicateDescription_ReturnsFailure()
    {
        // Arrange
        using var context = CreateContext();
        context.Pisos.AddRange(
            new Piso { IdPiso = 1, Descripcion = "Original" },
            new Piso { IdPiso = 2, Descripcion = "Existing" }
        );
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);
        var updated = new Piso { IdPiso = 1, Descripcion = "Existing" };

        // Act
        var result = await repo.UpdateEntityAsync(updated);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Ya existe", result.Message);
    }
    
    [Fact]
    public async Task UpdateEntityAsync_DeactivatedPiso_UpdatesSuccessfully()
    {
        // Arrange
        using var context = CreateContext();
        var original = new Piso { Estado = true };
        context.Pisos.Add(original);
        await context.SaveChangesAsync();
            
        var repo = CreateRepository(context);
        var updated = new Piso { 
            IdPiso = original.IdPiso, 
            Estado = false 
        };

        // Act
        var result = await repo.UpdateEntityAsync(updated);
        var updatedEntity = await repo.GetEntityByIdAsync(original.IdPiso);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(updatedEntity?.Estado);
    }
}