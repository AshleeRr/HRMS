using System.Linq.Expressions;
using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.Services
{
    public class EstadoHabitacionServiceTests
    {
        private readonly Mock<IEstadoHabitacionRepository> _mockEstadoHabitacionRepository;
        private readonly Mock<IHabitacionRepository> _mockHabitacionRepository;
        private readonly Mock<ILogger<EstadoHabitacionService>> _mockLogger;
        private readonly EstadoHabitacionService _estadoHabitacionService;
        private readonly Mock<IValidator<CreateEstadoHabitacionDto>> _mockValidator;

        public EstadoHabitacionServiceTests()
        {
            _mockEstadoHabitacionRepository = new Mock<IEstadoHabitacionRepository>();
            _mockHabitacionRepository = new Mock<IHabitacionRepository>();
            _mockLogger = new Mock<ILogger<EstadoHabitacionService>>();
            _mockValidator = new Mock<IValidator<CreateEstadoHabitacionDto>>();

            // Configurar el validador para pasar por defecto
            _mockValidator
                .Setup(v => v.Validate(It.IsAny<CreateEstadoHabitacionDto>()))
                .Returns(OperationResult.Success());

            _estadoHabitacionService = new EstadoHabitacionService(
                _mockEstadoHabitacionRepository.Object,
                _mockLogger.Object,
                _mockHabitacionRepository.Object,
                _mockValidator.Object
            );
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_EstadosExist_ReturnsSuccess()
        {
            // Arrange
            var estados = new List<EstadoHabitacion>
            {
                new EstadoHabitacion { IdEstadoHabitacion = 1, Descripcion = "Disponible", Estado = true },
                new EstadoHabitacion { IdEstadoHabitacion = 2, Descripcion = "Ocupado", Estado = true }
            };

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(estados);

            // Act
            var result = await _estadoHabitacionService.GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsAssignableFrom<List<EstadoHabitacionDto>>(result.Data);
        }

        [Fact]
        public async Task GetAll_NoEstadosExist_ReturnsFailure()
        {
            // Arrange
            _mockEstadoHabitacionRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<EstadoHabitacion>());

            // Act
            var result = await _estadoHabitacionService.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontraron estados de habitación.", result.Message);
        }

        [Fact]
        public async Task GetAll_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockEstadoHabitacionRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new Exception("Error de conexión"));

            // Act
            var result = await _estadoHabitacionService.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error de conexión", result.Message);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ValidId_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var estado = new EstadoHabitacion { IdEstadoHabitacion = id, Descripcion = "Disponible", Estado = true };

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(id))
                .ReturnsAsync(estado);

            // Act
            var result = await _estadoHabitacionService.GetById(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsType<EstadoHabitacionDto>(result.Data);
            var dto = (EstadoHabitacionDto)result.Data;
            Assert.Equal(estado.Descripcion, dto.Descripcion);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_InvalidId_ReturnsFailure(int id)
        {
            // Act
            var result = await _estadoHabitacionService.GetById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para obtener el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }

        [Fact]
        public async Task GetById_EstadoNotFound_ReturnsFailure()
        {
            // Arrange
            int id = 999;
            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(id))
                .ReturnsAsync((EstadoHabitacion)null);

            // Act
            var result = await _estadoHabitacionService.GetById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontró el estado con ID {id}.", result.Message);
        }

        [Fact]
        public async Task GetById_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            int id = 1;
            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(id))
                .ThrowsAsync(new Exception("Error al buscar el estado"));

            // Act
            var result = await _estadoHabitacionService.GetById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar el estado", result.Message);
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = "Nuevo Estado"
            };

            var estado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = dto.Descripcion,
                Estado = true
            };

            var operationResult = OperationResult.Success(estado, "Estado de habitación guardado correctamente");

            _mockEstadoHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<EstadoHabitacion, bool>>>()))
                .ReturnsAsync(false);

            _mockEstadoHabitacionRepository.Setup(repo => repo.SaveEntityAsync(It.IsAny<EstadoHabitacion>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _estadoHabitacionService.Save(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación ha sido guardada correctamente.", result.Message);
            Assert.NotNull(result.Data);
            Assert.IsType<EstadoHabitacionDto>(result.Data);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Save_EmptyDescripcion_ReturnsFailure(string descripcion)
        {
            // Arrange
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = descripcion
            };

            // Configurar el validador para fallar en este caso
            _mockValidator
                .Setup(v => v.Validate(It.Is<CreateEstadoHabitacionDto>(d => string.IsNullOrWhiteSpace(d.Descripcion))))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));

            // Act
            var result = await _estadoHabitacionService.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }

        [Fact]
        public async Task Save_DescripcionTooLong_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = new string('A', 51) // 51 caracteres
            };

            // Configurar el validador para fallar en este caso
            _mockValidator
                .Setup(v => v.Validate(It.Is<CreateEstadoHabitacionDto>(d => d.Descripcion.Length > 50)))
                .Returns(OperationResult.Failure("La descripción no puede exceder los 50 caracteres."));

            // Act
            var result = await _estadoHabitacionService.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede exceder los 50 caracteres.", result.Message);
        }

        [Fact]
        public async Task Save_DescripcionTooShort_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = "AB" // 2 caracteres (menos que el mínimo de 3)
            };

            // Configurar el validador para fallar en este caso
            _mockValidator
                .Setup(v => v.Validate(It.Is<CreateEstadoHabitacionDto>(d => d.Descripcion.Length < 3)))
                .Returns(OperationResult.Failure("La descripción debe tener al menos 3 caracteres."));

            // Act
            var result = await _estadoHabitacionService.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción debe tener al menos 3 caracteres.", result.Message);
        }

        [Fact]
        public async Task Save_DuplicateDescripcion_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = "Estado Existente"
            };

            _mockEstadoHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<EstadoHabitacion, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _estadoHabitacionService.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe un estado con la descripción '{dto.Descripcion}'.", result.Message);
        }

        [Fact]
        public async Task Save_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = "Nuevo Estado"
            };

            _mockEstadoHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<EstadoHabitacion, bool>>>()))
                .ReturnsAsync(false);

            _mockEstadoHabitacionRepository.Setup(repo => repo.SaveEntityAsync(It.IsAny<EstadoHabitacion>()))
                .ThrowsAsync(new Exception("Error al guardar el estado"));

            // Act
            var result = await _estadoHabitacionService.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar el estado de habitación", result.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado Actualizado"
            };

            var estado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado Original",
                Estado = true
            };

            var updatedEstado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = dto.Descripcion,
                Estado = true
            };

            var operationResult = OperationResult.Success(updatedEstado, "Estado de habitación actualizado correctamente.");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync(estado);

            _mockEstadoHabitacionRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<EstadoHabitacion>()))
                .ReturnsAsync(operationResult);

            // Asegurar que el validador acepta este DTO
            _mockValidator
                .Setup(v => v.Validate(It.Is<CreateEstadoHabitacionDto>(d => d.Descripcion == dto.Descripcion)))
                .Returns(OperationResult.Success());

            // Act
            var result = await _estadoHabitacionService.Update(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.IsType<EstadoHabitacionDto>(result.Data);
            var resultDto = (EstadoHabitacionDto)result.Data;
            Assert.Equal(dto.Descripcion, resultDto.Descripcion);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Update_InvalidId_ReturnsFailure(int id)
        {
            // Arrange
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = id,
                Descripcion = "Estado Actualizado"
            };

            // Act
            var result = await _estadoHabitacionService.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para actualizar el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Update_EmptyDescripcion_ReturnsFailure(string descripcion)
        {
            // Arrange
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = descripcion
            };

            // Configurar el validador para fallar en este caso
            _mockValidator
                .Setup(v => v.Validate(It.Is<CreateEstadoHabitacionDto>(d => string.IsNullOrWhiteSpace(d.Descripcion))))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));

            // Act
            var result = await _estadoHabitacionService.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }

        [Fact]
        public async Task Update_DescripcionTooLong_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = new string('A', 51) // 51 caracteres
            };

            // Configurar el validador para fallar en este caso
            _mockValidator
                .Setup(v => v.Validate(It.Is<CreateEstadoHabitacionDto>(d => d.Descripcion.Length > 50)))
                .Returns(OperationResult.Failure("La descripción no puede exceder los 50 caracteres."));

            // Act
            var result = await _estadoHabitacionService.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede exceder los 50 caracteres.", result.Message);
        }

        [Fact]
        public async Task Update_EstadoNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 999,
                Descripcion = "Estado Actualizado"
            };

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync((EstadoHabitacion)null);

            // Act
            var result = await _estadoHabitacionService.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontró el estado con ID {dto.IdEstadoHabitacion}.", result.Message);
        }

        [Fact]
        public async Task Update_RepositoryUpdateFails_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado Actualizado"
            };

            var estado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado Original",
                Estado = true
            };

            var failedResult = OperationResult.Failure("Error en la actualización");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync(estado);

            _mockEstadoHabitacionRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<EstadoHabitacion>()))
                .ReturnsAsync(failedResult);

            // Act
            var result = await _estadoHabitacionService.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error en la actualización", result.Message);
        }

        #endregion

        #region Remove Tests
        [Fact]
        public async Task Remove_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new DeleteEstadoHabitacionDto { IdEstadoHabitacion = 1 };
            var estado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado a Eliminar",
                Estado = true
            };

            var updatedEstado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado a Eliminar",
                Estado = false
            };

            var operationResult = OperationResult.Success(updatedEstado, "Estado actualizado correctamente");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync(estado);

            _mockHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);

            _mockEstadoHabitacionRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<EstadoHabitacion>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _estadoHabitacionService.Remove(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Remove_InvalidId_ReturnsFailure(int id)
        {
            // Arrange
            var dto = new DeleteEstadoHabitacionDto { IdEstadoHabitacion = id };

            // Act
            var result = await _estadoHabitacionService.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para eliminar el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }

        [Fact]
        public async Task Remove_EstadoNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteEstadoHabitacionDto { IdEstadoHabitacion = 999 };

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync((EstadoHabitacion)null);

            // Act
            var result = await _estadoHabitacionService.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontró el estado con ID {dto.IdEstadoHabitacion}", result.Message);
        }

        [Fact]
        public async Task Remove_EstadoInUse_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteEstadoHabitacionDto { IdEstadoHabitacion = 1 };
            var estado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado en Uso",
                Estado = true
            };

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync(estado);

            _mockHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _estadoHabitacionService.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se puede eliminar el estado porque está en uso por habitaciones", result.Message);
        }

        [Fact]
        public async Task Remove_UpdateFails_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteEstadoHabitacionDto { IdEstadoHabitacion = 1 };
            var estado = new EstadoHabitacion
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Estado a Eliminar",
                Estado = true
            };

            var failedResult = OperationResult.Failure("Error en la actualización");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ReturnsAsync(estado);

            _mockHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);

            _mockEstadoHabitacionRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<EstadoHabitacion>()))
                .ReturnsAsync(failedResult);

            // Act
            var result = await _estadoHabitacionService.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error en la actualización", result.Message);
        }

        [Fact]
        public async Task Remove_ThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteEstadoHabitacionDto { IdEstadoHabitacion = 1 };

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdEstadoHabitacion))
                .ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _estadoHabitacionService.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar el estado de habitación", result.Message);
        }

        #endregion

        #region GetEstadoByDescripcion Tests

        [Fact]
        public async Task GetEstadoByDescripcion_ValidDescripcion_ReturnsSingleEstado()
        {
            // Arrange
            string descripcion = "Disponible";
            var estado = new EstadoHabitacion { IdEstadoHabitacion = 1, Descripcion = descripcion, Estado = true };
            var estados = new List<EstadoHabitacion> { estado };
            var operationResult = OperationResult.Success(estados, "Estados encontrados correctamente");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEstadoByDescripcionAsync(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetEstadoByDescripcion_ValidDescripcion_ReturnsMultipleEstados()
        {
            // Arrange
            string descripcion = "habitacion";
            var estados = new List<EstadoHabitacion>
            {
                new EstadoHabitacion { IdEstadoHabitacion = 1, Descripcion = "Habitacion Disponible", Estado = true },
                new EstadoHabitacion { IdEstadoHabitacion = 2, Descripcion = "Habitacion Ocupada", Estado = true }
            };
            var operationResult = OperationResult.Success(estados, "Estados encontrados correctamente");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEstadoByDescripcionAsync(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetEstadoByDescripcion_EmptyDescripcion_ReturnsFailure(string descripcion)
        {
            // Act
            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado no puede estar vacía.", result.Message);
        }

        [Fact]
        public async Task GetEstadoByDescripcion_NoEstadosFound_ReturnsNoResults()
        {
            // Arrange
            string descripcion = "Inexistente";
            var estados = new List<EstadoHabitacion>();
            var operationResult = OperationResult.Success(estados, "No se encontraron estados con la descripción 'Inexistente'");

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEstadoByDescripcionAsync(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetEstadoByDescripcion_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            string descripcion = "Disponible";

            _mockEstadoHabitacionRepository.Setup(repo => repo.GetEstadoByDescripcionAsync(descripcion))
                .ThrowsAsync(new Exception("Error al buscar estados"));

            // Act
            var result = await _estadoHabitacionService.GetEstadoByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar estados", result.Message);
        }

        #endregion
    }
}