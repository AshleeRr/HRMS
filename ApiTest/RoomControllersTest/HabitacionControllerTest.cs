using HRMS.APIs.Controllers.RoomManagementControllers;
using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTest.RoomControllersTest
{
    public class HabitacionControllerTests
    {
        private readonly Mock<IHabitacionService> _mockHabitacionService;
        private readonly Mock<ILogger<HabitacionController>> _mockLogger;
        private readonly HabitacionController _controller;

        public HabitacionControllerTests()
        {
            _mockHabitacionService = new Mock<IHabitacionService>();
            _mockLogger = new Mock<ILogger<HabitacionController>>();
            _controller = new HabitacionController(_mockHabitacionService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenHabitacionesExist()
        {
            // Arrange
            var habitaciones = new List<HabitacionDto>
            {
                new HabitacionDto { IdHabitacion = 1, Numero = "101"},
                new HabitacionDto { IdHabitacion = 2, Numero = "102"}
            };
            
            var operationResult = OperationResult.Success(habitaciones);
            
            _mockHabitacionService
                .Setup(service => service.GetAll())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<HabitacionDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<HabitacionDto>).Count);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequest_WhenNoHabitacionesExist()
        {
            // Arrange
            var operationResult = OperationResult.Failure("No se encontraron habitaciones");
            
            _mockHabitacionService
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
        public async Task GetById_ReturnsOkResult_WhenHabitacionExists()
        {
            // Arrange
            int habitacionId = 1;
            var habitacion = new HabitacionDto { IdHabitacion = 1, Numero = "101"};
            
            var operationResult = OperationResult.Success(habitacion);
            
            _mockHabitacionService
                .Setup(service => service.GetById(habitacionId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(habitacionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(operationResult, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenOperationFails()
        {
            // Arrange
            int habitacionId = 1;
            
            var operationResult = OperationResult.Failure("Error al obtener la habitación");
            
            _mockHabitacionService
                .Setup(service => service.GetById(habitacionId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(habitacionId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(operationResult, badRequestResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenHabitacionDoesNotExist()
        {
            // Arrange
            int habitacionId = 999;
            
            var operationResult = OperationResult.Success(null);
            
            _mockHabitacionService
                .Setup(service => service.GetById(habitacionId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(habitacionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(operationResult, notFoundResult.Value);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenModelStateIsValid()
        {
            // Arrange
            var createDto = new CreateHabitacionDTo
            {
                Numero = "103",
                IdPiso = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1
            };
            
            var habitacionDto = new HabitacionDto
            {
                IdHabitacion = 1,
                Numero = "103",
            };
            
            var operationResult = OperationResult.Success(habitacionDto);
            
            _mockHabitacionService
                .Setup(service => service.Save(It.IsAny<CreateHabitacionDTo>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(HabitacionController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(habitacionDto.IdHabitacion, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<HabitacionDto>(createdAtActionResult.Value);
            Assert.Equal(habitacionDto.IdHabitacion, returnValue.IdHabitacion);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateHabitacionDTo
            {
            };
            
            _controller.ModelState.AddModelError("Numero", "El campo Numero es obligatorio");

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
            var createDto = new CreateHabitacionDTo
            {
                Numero = "103",
                IdPiso = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1
            };
            
            var operationResult = OperationResult.Failure("Error al crear la habitación");
            
            _mockHabitacionService
                .Setup(service => service.Save(It.IsAny<CreateHabitacionDTo>()))
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
            int habitacionId = 1;
            var updateDto = new UpdateHabitacionDto
            {
                IdHabitacion = habitacionId,
                Numero = "103-Updated",
                IdPiso = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1
            };
            
            var operationResult = OperationResult.Success();
            
            _mockHabitacionService
                .Setup(service => service.Update(It.IsAny<UpdateHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(habitacionId, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            int habitacionId = 1;
            var updateDto = new UpdateHabitacionDto
            {
                IdHabitacion = 2,
                Numero = "103-Updated",
                IdPiso = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1
            };

            // Act
            var result = await _controller.Update(habitacionId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int habitacionId = 1;
            var updateDto = new UpdateHabitacionDto
            {
                IdHabitacion = habitacionId,
            };
            
            _controller.ModelState.AddModelError("Numero", "El campo Numero es obligatorio");

            // Act
            var result = await _controller.Update(habitacionId, updateDto);

            // Assert
            Assert.IsType<ValidationProblemDetails>(
                (result as ObjectResult)?.Value
            );
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenHabitacionDoesNotExist()
        {
            // Arrange
            int habitacionId = 999;
            var updateDto = new UpdateHabitacionDto
            {
                IdHabitacion = habitacionId,
                Numero = "103-Updated",
                IdPiso = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1
            };
            
            var operationResult = OperationResult.Failure($"No se encontró la habitación con ID: {habitacionId}");
            
            _mockHabitacionService
                .Setup(service => service.Update(It.IsAny<UpdateHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(habitacionId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenDeleteSuccessful()
        {
            // Arrange
            int habitacionId = 1;
            
            var operationResult = OperationResult.Success();
            
            _mockHabitacionService
                .Setup(service => service.Remove(It.IsAny<DeleteHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(habitacionId);

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
        public async Task Delete_ReturnsBadRequest_WhenHabitacionDoesNotExist()
        {
            // Arrange
            int habitacionId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró la habitación con ID: {habitacionId}");
            
            _mockHabitacionService
                .Setup(service => service.Remove(It.IsAny<DeleteHabitacionDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(habitacionId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetByPiso Tests

        [Fact]
        public async Task GetByPiso_ReturnsOkResult_WhenHabitacionesExist()
        {
            // Arrange
            int pisoId = 1;
            
            var habitaciones = new List<HabitacionDto>
            {
                new HabitacionDto { IdHabitacion = 1, Numero = "101" },
                new HabitacionDto { IdHabitacion = 2, Numero = "102"}
            };
            
            var operationResult = OperationResult.Success(habitaciones);
            
            _mockHabitacionService
                .Setup(service => service.GetByPiso(pisoId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByPiso(pisoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<HabitacionDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<HabitacionDto>).Count);
        }

        [Fact]
        public async Task GetByPiso_ReturnsBadRequest_WhenPisoIdIsInvalid()
        {
            // Arrange
            int invalidPisoId = 0;

            // Act
            var result = await _controller.GetByPiso(invalidPisoId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByPiso_ReturnsBadRequest_WhenNoHabitacionesExist()
        {
            // Arrange
            int pisoId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontraron habitaciones en el piso con ID: {pisoId}");
            
            _mockHabitacionService
                .Setup(service => service.GetByPiso(pisoId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByPiso(pisoId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetByCategoria Tests

        [Fact]
        public async Task GetByCategoria_ReturnsOkResult_WhenHabitacionesExist()
        {
            // Arrange
            string categoria = "Standard";
            
            var habitaciones = new List<HabitacionDto>
            {
                new HabitacionDto { IdHabitacion = 1, Numero = "101" },
                new HabitacionDto { IdHabitacion = 2, Numero = "102"}
            };
            
            var operationResult = OperationResult.Success(habitaciones);
            
            _mockHabitacionService
                .Setup(service => service.GetByCategoria(categoria))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByCategoria(categoria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<HabitacionDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<HabitacionDto>).Count);
        }

        [Fact]
        public async Task GetByCategoria_ReturnsBadRequest_WhenCategoriaIsInvalid()
        {
            // Arrange
            string invalidCategoria = "";

            // Act
            var result = await _controller.GetByCategoria(invalidCategoria);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByCategoria_ReturnsBadRequest_WhenNoHabitacionesExist()
        {
            // Arrange
            string categoria = "CategoriaInexistente";
            
            var operationResult = OperationResult.Failure($"No se encontraron habitaciones en la categoría: {categoria}");
            
            _mockHabitacionService
                .Setup(service => service.GetByCategoria(categoria))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByCategoria(categoria);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetByNumero Tests

        [Fact]
        public async Task GetByNumero_ReturnsOkResult_WhenHabitacionExists()
        {
            // Arrange
            string numero = "101";
            
            var habitacion = new HabitacionDto { IdHabitacion = 1, Numero = "101"};
            
            var operationResult = OperationResult.Success(habitacion);
            
            _mockHabitacionService
                .Setup(service => service.GetByNumero(numero))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByNumero(numero);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<HabitacionDto>(okResult.Value);
            Assert.Equal(numero, returnValue.Numero);
        }

        [Fact]
        public async Task GetByNumero_ReturnsBadRequest_WhenNumeroIsInvalid()
        {
            // Arrange
            string invalidNumero = "";

            // Act
            var result = await _controller.GetByNumero(invalidNumero);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByNumero_ReturnsBadRequest_WhenHabitacionDoesNotExist()
        {
            // Arrange
            string numero = "999";
            
            var operationResult = OperationResult.Failure($"No se encontró la habitación con número: {numero}");
            
            _mockHabitacionService
                .Setup(service => service.GetByNumero(numero))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByNumero(numero);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetInfoHabitaciones Tests

        [Fact]
        public async Task GetInfoHabitaciones_ReturnsOkResult_WhenInfoExists()
        {
            // Arrange
            var infoHabitaciones = new List<object>();
            
            var operationResult = OperationResult.Success(infoHabitaciones);
            
            _mockHabitacionService
                .Setup(service => service.GetInfoHabitacionesAsync())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetInfoHabitaciones();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetInfoHabitaciones_ReturnsBadRequest_WhenOperationFails()
        {
            // Arrange
            var operationResult = OperationResult.Failure("Error al obtener la información de habitaciones");
            
            _mockHabitacionService
                .Setup(service => service.GetInfoHabitacionesAsync())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetInfoHabitaciones();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}