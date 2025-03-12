using System.Linq.Expressions;
using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.Services
{
    public class PisoServicesTests
    {
        private readonly Mock<IPisoRepository> _mockPisoRepository;
        private readonly Mock<ILogger<PisoServices>> _mockLogger;
        private readonly Mock<IHabitacionRepository> _mockHabitacionRepository;
        private readonly PisoServices _service;

        public PisoServicesTests()
        {
            _mockPisoRepository = new Mock<IPisoRepository>();
            _mockLogger = new Mock<ILogger<PisoServices>>();
            _mockHabitacionRepository = new Mock<IHabitacionRepository>();
    
            var validator = new PisoServiceValidator();

            _service = new PisoServices(
                _mockPisoRepository.Object,
                _mockLogger.Object,
                _mockHabitacionRepository.Object,
                validator
            );
        }

        #region GetAll Tests
        [Fact]
        public async Task GetAll_WithExistingPisos_ReturnsSuccess()
        {
            // Arrange
            var pisos = new List<Piso>
            {
                new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true },
                new Piso { IdPiso = 2, Descripcion = "Segundo Piso", Estado = true }
            };

            _mockPisoRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(pisos);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
    
            
            var enumerable = (result.Data as IEnumerable<object>);
            Assert.NotNull(enumerable);
            Assert.True(enumerable.Any());
        }

        [Fact]
        public async Task GetAll_WithNoPisos_ReturnsFailure()
        {
            // Arrange
            _mockPisoRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Piso>());

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontraron pisos registrados", result.Message);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockPisoRepository.Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database error", result.Message);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsPiso()
        {
            // Arrange
            var piso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(1))
                .ReturnsAsync(piso);

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            var dto = Assert.IsType<PisoDto>(result.Data);
            Assert.Equal(piso.Descripcion, dto.Descripcion);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsFailure()
        {
            
            // Act
            var result = await _service.GetById(0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para obtener el piso, el ID debe ser mayor que cero.", result.Message);
        }

        [Fact]
        public async Task GetById_WithNonExistentPiso_ReturnsFailure()
        {
            // Arrange
            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(99))
                .ReturnsAsync((Piso)null);

            // Act
            var result = await _service.GetById(99);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontró el piso con ID 99.", result.Message);
        }

        [Fact]
        public async Task GetById_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database error", result.Message);
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreatePisoDto { Descripcion = "Primer Piso" };
            var piso = new Piso { IdPiso = 1, Descripcion = dto.Descripcion, Estado = true };

            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(false);

            _mockPisoRepository.Setup(r => r.SaveEntityAsync(It.IsAny<Piso>()))
                .ReturnsAsync(OperationResult.Success(piso));

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Piso creado correctamente.", result.Message);
            Assert.NotNull(result.Data);
            var resultDto = Assert.IsType<PisoDto>(result.Data);
            Assert.Equal(dto.Descripcion, resultDto.Descripcion);
        }

        [Fact]
        public async Task Save_WithInvalidDto_ReturnsFailure()
        {
            // Arrange
            var dto = new CreatePisoDto { Descripcion = "" };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("descripción", result.Message.ToLower());
        }

        [Fact]
        public async Task Save_WithDuplicateDescripcion_ReturnsFailure()
        {
            // Arrange
            var dto = new CreatePisoDto { Descripcion = "Primer Piso" };

            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe un piso con la descripción '{dto.Descripcion}'.", result.Message);
        }

        [Fact]
        public async Task Save_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var dto = new CreatePisoDto { Descripcion = "Primer Piso" };
            
            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(false);

            _mockPisoRepository.Setup(r => r.SaveEntityAsync(It.IsAny<Piso>()))
                .ReturnsAsync(OperationResult.Failure("Error al guardar el piso en la base de datos."));

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error al guardar el piso en la base de datos.", result.Message);
        }

        [Fact]
        public async Task Save_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new CreatePisoDto { Descripcion = "Primer Piso" };
            
            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(false);

            _mockPisoRepository.Setup(r => r.SaveEntityAsync(It.IsAny<Piso>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database error", result.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 1, Descripcion = "Primer Piso Actualizado" };
            var existingPiso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true };
            var updatedPiso = new Piso { IdPiso = 1, Descripcion = dto.Descripcion, Estado = true };
            
            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(existingPiso);

            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(false);

            _mockPisoRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Piso>()))
                .ReturnsAsync(OperationResult.Success(updatedPiso));

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Piso actualizado correctamente.", result.Message);
            Assert.NotNull(result.Data);
            var resultDto = Assert.IsType<PisoDto>(result.Data);
            Assert.Equal(dto.Descripcion, resultDto.Descripcion);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 0, Descripcion = "Primer Piso" };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para actualizar el piso, el ID debe ser mayor que cero.", result.Message);
        }

        [Fact]
        public async Task Update_WithInvalidDto_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 1, Descripcion = "" };
            var existingPiso = new Piso { IdPiso = 1, Descripcion = "Piso Original", Estado = true };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(existingPiso);
            
            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("descripción", result.Message.ToLower());
        }

        [Fact]
        public async Task Update_WithNonExistentPiso_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 99, Descripcion = "Primer Piso" };
            
            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync((Piso)null);

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontró el piso con ID {dto.IdPiso}.", result.Message);
        }

        [Fact]
        public async Task Update_WithDuplicateDescripcion_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 1, Descripcion = "Primer Piso" };
            var existingPiso = new Piso { IdPiso = 1, Descripcion = "Piso Original", Estado = true };
            
            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(existingPiso);

            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe un piso con la descripción '{dto.Descripcion}'.", result.Message);
        }

        [Fact]
        public async Task Update_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 1, Descripcion = "Primer Piso" };
            var existingPiso = new Piso { IdPiso = 1, Descripcion = "Piso Original", Estado = true };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(existingPiso);

            _mockPisoRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Piso, bool>>>()))
                .ReturnsAsync(false);

            _mockPisoRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Piso>()))
                .ReturnsAsync(OperationResult.Failure("Error al actualizar el piso en la base de datos."));

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error al actualizar el piso en la base de datos.", result.Message);
        }

        [Fact]
        public async Task Update_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdatePisoDto { IdPiso = 1, Descripcion = "Primer Piso" };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database error", result.Message);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new DeletePisoDto { IdPiso = 1 };
            var piso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true };
            var updatedPiso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = false };
            
            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(piso);
                
            _mockHabitacionRepository.Setup(r => r.GetByPisoAsync(dto.IdPiso))
                .ReturnsAsync(OperationResult.Success(new List<Habitacion>()));

            _mockPisoRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Piso>()))
                .ReturnsAsync(OperationResult.Success(updatedPiso));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Piso eliminado correctamente.", result.Message);
            Assert.NotNull(result.Data);
            var resultDto = Assert.IsType<PisoDto>(result.Data);
            Assert.Equal(piso.Descripcion, resultDto.Descripcion);
        }

        [Fact]
        public async Task Remove_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var dto = new DeletePisoDto { IdPiso = 0 };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para eliminar el piso, el ID debe ser mayor que cero.", result.Message);
        }

        [Fact]
        public async Task Remove_WithNonExistentPiso_ReturnsFailure()
        {
            // Arrange
            var dto = new DeletePisoDto { IdPiso = 99 };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync((Piso)null);

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontró el piso con ID {dto.IdPiso}.", result.Message);
        }
        
        [Fact]
        public async Task Remove_WithHabitacionesAsociadas_ReturnsFailure()
        {
            // Arrange
            var dto = new DeletePisoDto { IdPiso = 1 };
            var piso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true };
            var habitaciones = new List<Habitacion>
            {
                new Habitacion { IdHabitacion = 1, IdPiso = 1, Estado = true }
            };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(piso);

            _mockHabitacionRepository.Setup(r => r.GetByPisoAsync(dto.IdPiso))
                .ReturnsAsync(OperationResult.Success(habitaciones));
            
            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "No se puede eliminar el piso porque tiene habitaciones asociadas. Debe eliminar o reubicar las habitaciones primero.",
                result.Message);
        }

        [Fact]
        public async Task Remove_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var dto = new DeletePisoDto { IdPiso = 1 };
            var piso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ReturnsAsync(piso);

            _mockHabitacionRepository.Setup(r => r.GetByPisoAsync(dto.IdPiso))
                .ReturnsAsync(OperationResult.Success(new List<Habitacion>()));

            _mockPisoRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Piso>()))
                .ReturnsAsync(OperationResult.Failure("Error al actualizar el piso."));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error al eliminar el piso.", result.Message);
        }

        [Fact]
        public async Task Remove_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new DeletePisoDto { IdPiso = 1 };

            _mockPisoRepository.Setup(r => r.GetEntityByIdAsync(dto.IdPiso))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database error", result.Message);
        }

        #endregion

        #region GetPisoByDescripcion Tests

        [Fact]
        public async Task GetPisoByDescripcion_WithValidDescripcion_ReturnsPiso()
        {
            // Arrange
            var descripcion = "Primer";
            var piso = new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true };

            _mockPisoRepository.Setup(r => r.GetPisoByDescripcion(descripcion))
                .ReturnsAsync(OperationResult.Success(piso));

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            var resultDto = Assert.IsType<PisoDto>(result.Data);
            Assert.Equal(piso.Descripcion, resultDto.Descripcion);
        }

        [Fact]
        public async Task GetPisoByDescripcion_WithValidDescripcionMultipleResults_ReturnsPisos()
        {
            // Arrange
            var descripcion = "Piso";
            var pisos = new List<Piso>
            {
                new Piso { IdPiso = 1, Descripcion = "Primer Piso", Estado = true },
                new Piso { IdPiso = 2, Descripcion = "Segundo Piso", Estado = true }
            };

            _mockPisoRepository.Setup(r => r.GetPisoByDescripcion(descripcion))
                .ReturnsAsync(OperationResult.Success(pisos));

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
    
            var enumerable = (result.Data as IEnumerable<object>);
            Assert.NotNull(enumerable);
            Assert.True(enumerable.Any());
        }
        
        [Fact]
        public async Task GetPisoByDescripcion_WithEmptyDescripcion_ReturnsFailure()
        {
            // Arrange
            string descripcion = "";

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede estar vacía.", result.Message);
        }

        [Fact]
        public async Task GetPisoByDescripcion_WithNullDescripcion_ReturnsFailure()
        {
            // Arrange
            string descripcion = null;

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede estar vacía.", result.Message);
        }

        [Fact]
        public async Task GetPisoByDescripcion_WithNoPisosFound_ReturnsFailure()
        {
            // Arrange
            var descripcion = "NoExistente";

            _mockPisoRepository.Setup(r => r.GetPisoByDescripcion(descripcion))
                .ReturnsAsync(OperationResult.Success(new List<Piso>()));

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontraron pisos con la descripción '{descripcion}'.", result.Message);
        }

        [Fact]
        public async Task GetPisoByDescripcion_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var descripcion = "Primer";

            _mockPisoRepository.Setup(r => r.GetPisoByDescripcion(descripcion))
                .ReturnsAsync(OperationResult.Failure("Error al buscar pisos."));

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontraron pisos con la descripción '{descripcion}'." , result.Message);
        }

        [Fact]
        public async Task GetPisoByDescripcion_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var descripcion = "Primer";

            _mockPisoRepository.Setup(r => r.GetPisoByDescripcion(descripcion))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetPisoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Database error", result.Message);
        }

        #endregion
    }
}