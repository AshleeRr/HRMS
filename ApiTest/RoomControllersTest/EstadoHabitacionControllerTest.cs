using HRMS.APIs.Controllers.RoomManagementControllers;
using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTest.RoomControllersTest
{
    public class EstadoHabitacionControllerTests
    {
        private readonly Mock<IEstadoHabitacionService> _mockEstadoHabitacionService;
        private readonly Mock<ILogger<EstadoHabitacionController>> _mockLogger;
        private readonly EstadoHabitacionController _controller;

        public EstadoHabitacionControllerTests()
        {
            _mockEstadoHabitacionService = new Mock<IEstadoHabitacionService>();
            _mockLogger = new Mock<ILogger<EstadoHabitacionController>>();
            _controller = new EstadoHabitacionController(_mockEstadoHabitacionService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenEstadosExist()
        {
            // Arrange
            var estados = new List<EstadoHabitacionDto>
            {
                new EstadoHabitacionDto { IdEstadoHabitacion = 1, Descripcion = "Disponible" },
                new EstadoHabitacionDto { IdEstadoHabitacion = 2, Descripcion = "Ocupada" }
            };
            
            var operationResult = OperationResult.Success(estados);
            
            _mockEstadoHabitacionService
                .Setup(service => service.GetAll())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EstadoHabitacionDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<EstadoHabitacionDto>).Count);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequest_WhenNoEstadosExist()
        {
            // Arrange
            var operationResult = OperationResult.Failure("No se encontraron estados de habitación");
            
            _mockEstadoHabitacionService
                .Setup(service => service.GetAll())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenEstadoExists()
        {
            // Arrange
            int estadoId = 1;
            var estado = new EstadoHabitacionDto { IdEstadoHabitacion = 1, Descripcion = "Disponible" };
            
            var operationResult = OperationResult.Success(estado);
            
            _mockEstadoHabitacionService
                .Setup(service => service.GetById(estadoId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(estadoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<EstadoHabitacionDto>(okResult.Value);
            Assert.Equal(estadoId, returnValue.IdEstadoHabitacion);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenEstadoDoesNotExist()
        {
            // Arrange
            int estadoId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró el estado de habitación con ID: {estadoId}");
            
            _mockEstadoHabitacionService
                .Setup(service => service.GetById(estadoId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(estadoId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            int invalidId = 0;

            // Act
            var result = await _controller.GetById(invalidId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetByDescripcion Tests

        [Fact]
        public async Task GetByDescripcion_ReturnsOkResult_WhenEstadosExist()
        {
            // Arrange
            string descripcion = "Disponible";
            
            var estados = new List<EstadoHabitacionDto>
            {
                new EstadoHabitacionDto { IdEstadoHabitacion = 1, Descripcion = "Disponible" }
            };
            
            var operationResult = OperationResult.Success(estados);
            
            _mockEstadoHabitacionService
                .Setup(service => service.GetEstadoByDescripcion(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByDescripcion(descripcion);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EstadoHabitacionDto>>(okResult.Value);
            Assert.Single(returnValue as List<EstadoHabitacionDto>);
        }

        [Fact]
        public async Task GetByDescripcion_ReturnsBadRequest_WhenDescripcionIsInvalid()
        {
            // Arrange
            string invalidDescripcion = "";

            // Act
            var result = await _controller.GetByDescripcion(invalidDescripcion);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByDescripcion_ReturnsBadRequest_WhenNoEstadosExist()
        {
            // Arrange
            string descripcion = "NoExiste";
            
            var operationResult = OperationResult.Failure($"No se encontraron estados de habitación con descripción: {descripcion}");
            
            _mockEstadoHabitacionService
                .Setup(service => service.GetEstadoByDescripcion(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByDescripcion(descripcion);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenModelStateIsValid()
        {
            // Arrange
            var createDto = new CreateEstadoHabitacionDto
            {
                Descripcion = "Nuevo Estado"
            };
            
            var estadoDto = new EstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Nuevo Estado",
            };
            
            var operationResult = OperationResult.Success(estadoDto);
            
            _mockEstadoHabitacionService
                .Setup(service => service.Save(It.IsAny<CreateEstadoHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(EstadoHabitacionController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(estadoDto.IdEstadoHabitacion, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<EstadoHabitacionDto>(createdAtActionResult.Value);
            Assert.Equal(estadoDto.IdEstadoHabitacion, returnValue.IdEstadoHabitacion);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateEstadoHabitacionDto
            {
            };
            
            _controller.ModelState.AddModelError("Descripcion", "El campo Descripcion es obligatorio");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.IsType<ValidationProblemDetails>(
                (result as ObjectResult)?.Value
            );
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var createDto = new CreateEstadoHabitacionDto
            {
                Descripcion = "Nuevo Estado"
            };
            
            var operationResult = OperationResult.Failure("Error al crear el estado de habitación");
            
            _mockEstadoHabitacionService
                .Setup(service => service.Save(It.IsAny<CreateEstadoHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ReturnsOkResult_WhenUpdateSuccessful()
        {
            // Arrange
            int estadoId = 1;
            var updateDto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = estadoId,
                Descripcion = "Estado Actualizado"
            };
            
            var operationResult = OperationResult.Success();
            
            _mockEstadoHabitacionService
                .Setup(service => service.Update(It.IsAny<UpdateEstadoHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(estadoId, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            int estadoId = 1;
            var updateDto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 2, 
                Descripcion = "Estado Actualizado"
            };

            // Act
            var result = await _controller.Update(estadoId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int estadoId = 1;
            var updateDto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = estadoId,
            };
            
            _controller.ModelState.AddModelError("Descripcion", "El campo Descripcion es obligatorio");

            // Act
            var result = await _controller.Update(estadoId, updateDto);

            // Assert
            Assert.IsType<ValidationProblemDetails>(
                (result as ObjectResult)?.Value
            );
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenEstadoDoesNotExist()
        {
            // Arrange
            int estadoId = 999;
            var updateDto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = estadoId,
                Descripcion = "Estado Actualizado"
            };
            
            var operationResult = OperationResult.Failure($"No se encontró el estado de habitación con ID: {estadoId}");
            
            _mockEstadoHabitacionService
                .Setup(service => service.Update(It.IsAny<UpdateEstadoHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(estadoId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenDeleteSuccessful()
        {
            // Arrange
            int estadoId = 1;
            
            var operationResult = OperationResult.Success();
            
            _mockEstadoHabitacionService
                .Setup(service => service.Remove(It.IsAny<DeleteEstadoHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(estadoId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            int invalidId = 0;

            // Act
            var result = await _controller.Delete(invalidId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenEstadoDoesNotExist()
        {
            // Arrange
            int estadoId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró el estado de habitación con ID: {estadoId}");
            
            _mockEstadoHabitacionService
                .Setup(service => service.Remove(It.IsAny<DeleteEstadoHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(estadoId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}