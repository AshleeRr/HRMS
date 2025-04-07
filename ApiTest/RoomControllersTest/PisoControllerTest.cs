using HRMS.APIs.Controllers.RoomManagementControllers;
using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTest.RoomControllersTest
{
    public class PisoControllerTests
    {
        private readonly Mock<IPisoService> _mockPisoService;
        private readonly Mock<ILogger<PisoController>> _mockLogger;
        private readonly PisoController _controller;

        public PisoControllerTests()
        {
            _mockPisoService = new Mock<IPisoService>();
            _mockLogger = new Mock<ILogger<PisoController>>();
            _controller = new PisoController(_mockPisoService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenPisosExist()
        {
            // Arrange
            var pisos = new List<PisoDto>
            {
                new PisoDto { IdPiso = 1, Descripcion = "Primer Piso"},
                new PisoDto { IdPiso = 2, Descripcion = "Segundo Piso" }
            };
            
            var operationResult = OperationResult.Success(pisos);
            
            _mockPisoService
                .Setup(service => service.GetAll())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PisoDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<PisoDto>).Count);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequest_WhenNoPisosExist()
        {
            // Arrange
            var operationResult = OperationResult.Failure("No se encontraron pisos");
            
            _mockPisoService
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
        public async Task GetById_ReturnsOkResult_WhenPisoExists()
        {
            // Arrange
            int pisoId = 1;
            var piso = new PisoDto { IdPiso = 1, Descripcion = "Primer Piso" };
            
            var operationResult = OperationResult.Success(piso);
            
            _mockPisoService
                .Setup(service => service.GetById(pisoId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(pisoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PisoDto>(okResult.Value);
            Assert.Equal(pisoId, returnValue.IdPiso);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenPisoDoesNotExist()
        {
            // Arrange
            int pisoId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró el piso con ID: {pisoId}");
            
            _mockPisoService
                .Setup(service => service.GetById(pisoId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(pisoId);

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
        public async Task GetByDescripcion_ReturnsOkResult_WhenPisosExist()
        {
            // Arrange
            string descripcion = "Primer";
            
            var pisos = new List<PisoDto>
            {
                new PisoDto { IdPiso = 1, Descripcion = "Primer Piso"}
            };
            
            var operationResult = OperationResult.Success(pisos);
            
            _mockPisoService
                .Setup(service => service.GetPisoByDescripcion(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByDescripcion(descripcion);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PisoDto>>(okResult.Value);
            Assert.Single(returnValue as List<PisoDto>);
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
        public async Task GetByDescripcion_ReturnsBadRequest_WhenNoPisosExist()
        {
            // Arrange
            string descripcion = "NoExiste";
            
            var operationResult = OperationResult.Failure($"No se encontraron pisos con descripción: {descripcion}");
            
            _mockPisoService
                .Setup(service => service.GetPisoByDescripcion(descripcion))
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
            var createDto = new CreatePisoDto
            {
                Descripcion = "Nuevo Piso"
            };
            
            var pisoDto = new PisoDto
            {
                IdPiso = 1,
                Descripcion = "Nuevo Piso",
            };
            
            var operationResult = OperationResult.Success(pisoDto);
            
            _mockPisoService
                .Setup(service => service.Save(It.IsAny<CreatePisoDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PisoController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(pisoDto.IdPiso, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<PisoDto>(createdAtActionResult.Value);
            Assert.Equal(pisoDto.IdPiso, returnValue.IdPiso);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreatePisoDto
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
            var createDto = new CreatePisoDto
            {
                Descripcion = "Nuevo Piso"
            };
            
            var operationResult = OperationResult.Failure("Error al crear el piso");
            
            _mockPisoService
                .Setup(service => service.Save(It.IsAny<CreatePisoDto>()))
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
            int pisoId = 1;
            var updateDto = new UpdatePisoDto
            {
                IdPiso = pisoId,
                Descripcion = "Piso Actualizado"
            };
            
            var operationResult = OperationResult.Success();
            
            _mockPisoService
                .Setup(service => service.Update(It.IsAny<UpdatePisoDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(pisoId, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            int pisoId = 1;
            var updateDto = new UpdatePisoDto
            {
                IdPiso = 2, 
                Descripcion = "Piso Actualizado"
            };

            // Act
            var result = await _controller.Update(pisoId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int pisoId = 1;
            var updateDto = new UpdatePisoDto
            {
                IdPiso = pisoId,
            };
            
            _controller.ModelState.AddModelError("Descripcion", "El campo Descripcion es obligatorio");

            // Act
            var result = await _controller.Update(pisoId, updateDto);

            // Assert
            Assert.IsType<ValidationProblemDetails>(
                (result as ObjectResult)?.Value
            );
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenPisoDoesNotExist()
        {
            // Arrange
            int pisoId = 999;
            var updateDto = new UpdatePisoDto
            {
                IdPiso = pisoId,
                Descripcion = "Piso Actualizado"
            };
            
            var operationResult = OperationResult.Failure($"No se encontró el piso con ID: {pisoId}");
            
            _mockPisoService
                .Setup(service => service.Update(It.IsAny<UpdatePisoDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(pisoId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenDeleteSuccessful()
        {
            // Arrange
            int pisoId = 1;
            
            var operationResult = OperationResult.Success();
            
            _mockPisoService
                .Setup(service => service.Remove(It.IsAny<DeletePisoDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(pisoId);

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
        public async Task Delete_ReturnsBadRequest_WhenPisoDoesNotExist()
        {
            // Arrange
            int pisoId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró el piso con ID: {pisoId}");
            
            _mockPisoService
                .Setup(service => service.Remove(It.IsAny<DeletePisoDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(pisoId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}