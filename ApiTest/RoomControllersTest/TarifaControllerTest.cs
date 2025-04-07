using HRMS.APIs.Controllers.RoomManagementControllers;
using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTest.RoomControllersTest
{
    public class TarifaControllerTests
    {
        private readonly Mock<ITarifaService> _mockTarifaService;
        private readonly Mock<ILogger<TarifaController>> _mockLogger;
        private readonly TarifaController _controller;

        public TarifaControllerTests()
        {
            _mockTarifaService = new Mock<ITarifaService>();
            _mockLogger = new Mock<ILogger<TarifaController>>();
            _controller = new TarifaController(_mockTarifaService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenTarifasExist()
        {
            // Arrange
            var tarifas = new List<TarifaDto>
            {
                new TarifaDto { IdTarifa = 1, PrecioPorNoche = 100.00m, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(30) },
                new TarifaDto { IdTarifa = 2, PrecioPorNoche = 150.00m, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(30) }
            };
            
            var operationResult = OperationResult.Success(tarifas);
            
            _mockTarifaService
                .Setup(service => service.GetAll())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<TarifaDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<TarifaDto>).Count);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequest_WhenNoTarifasExist()
        {
            // Arrange
            var operationResult = OperationResult.Failure("No se encontraron tarifas");
            
            _mockTarifaService
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
        public async Task GetById_ReturnsOkResult_WhenTarifaExists()
        {
            // Arrange
            int tarifaId = 1;
            var tarifa = new TarifaDto { IdTarifa = 1, PrecioPorNoche = 100.00m, FechaInicio = DateTime.Now, FechaFin = DateTime.Now.AddDays(30) };
            
            var operationResult = OperationResult.Success(tarifa);
            
            _mockTarifaService
                .Setup(service => service.GetById(tarifaId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(tarifaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TarifaDto>(okResult.Value);
            Assert.Equal(tarifaId, returnValue.IdTarifa);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenTarifaDoesNotExist()
        {
            // Arrange
            int tarifaId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró la tarifa con ID: {tarifaId}");
            
            _mockTarifaService
                .Setup(service => service.GetById(tarifaId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(tarifaId);

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

        #region GetTarifasVigentes Tests

        [Fact]
        public async Task GetTarifasVigentes_ReturnsOkResult_WhenTarifasExist()
        {
            // Arrange
            string fecha = "2023-05-15";
            
            var tarifas = new List<TarifaDto>
            {
                new TarifaDto { IdTarifa = 1, PrecioPorNoche = 100.00m, FechaInicio = DateTime.Parse("2023-05-01"), FechaFin = DateTime.Parse("2023-05-31") },
                new TarifaDto { IdTarifa = 2, PrecioPorNoche = 150.00m, FechaInicio = DateTime.Parse("2023-05-01"), FechaFin = DateTime.Parse("2023-05-31") }
            };
            
            var operationResult = OperationResult.Success(tarifas);
            
            _mockTarifaService
                .Setup(service => service.GetTarifasVigentes(fecha))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetTarifasVigentes(fecha);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<TarifaDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<TarifaDto>).Count);
        }

        [Fact]
        public async Task GetTarifasVigentes_ReturnsBadRequest_WhenNoTarifasExist()
        {
            // Arrange
            string fecha = "2023-12-31";
            
            var operationResult = OperationResult.Failure($"No se encontraron tarifas vigentes para la fecha: {fecha}");
            
            _mockTarifaService
                .Setup(service => service.GetTarifasVigentes(fecha))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetTarifasVigentes(fecha);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetHabitacionesByPrecio Tests

        [Fact]
        public async Task GetHabitacionesByPrecio_ReturnsOkResult_WhenHabitacionesExist()
        {
            // Arrange
            decimal precio = 100.00m;
            
            var habitaciones = new List<object>(); 
            
            var operationResult = OperationResult.Success(habitaciones);
            
            _mockTarifaService
                .Setup(service => service.GetHabitacionesByPrecio(precio))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetHabitacionesByPrecio(precio);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetHabitacionesByPrecio_ReturnsBadRequest_WhenPrecioIsInvalid()
        {
            // Arrange
            decimal invalidPrecio = 0;

            // Act
            var result = await _controller.GetHabitacionesByPrecio(invalidPrecio);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetHabitacionesByPrecio_ReturnsBadRequest_WhenNoHabitacionesExist()
        {
            // Arrange
            decimal precio = 999.99m;
            
            var operationResult = OperationResult.Failure($"No se encontraron habitaciones con precio: {precio}");
            
            _mockTarifaService
                .Setup(service => service.GetHabitacionesByPrecio(precio))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetHabitacionesByPrecio(precio);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenModelStateIsValid()
        {
            // Arrange
            var createDto = new CreateTarifaDto
            {
                PrecioPorNoche = 100.00m,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
                IdCategoria = 1
            };
            
            var tarifaDto = new TarifaDto
            {
                IdTarifa = 1,
                PrecioPorNoche = 100.00m,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
            };
            
            var operationResult = OperationResult.Success(tarifaDto);
            
            _mockTarifaService
                .Setup(service => service.Save(It.IsAny<CreateTarifaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TarifaController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(tarifaDto.IdTarifa, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<TarifaDto>(createdAtActionResult.Value);
            Assert.Equal(tarifaDto.IdTarifa, returnValue.IdTarifa);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateTarifaDto
            {
            };
            
            _controller.ModelState.AddModelError("Precio", "El campo Precio es obligatorio");

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
            var createDto = new CreateTarifaDto
            {
                PrecioPorNoche = 100.00m,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
                IdCategoria = 1
            };
            
            var operationResult = OperationResult.Failure("Error al crear la tarifa");
            
            _mockTarifaService
                .Setup(service => service.Save(It.IsAny<CreateTarifaDto>()))
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
            int tarifaId = 1;
            var updateDto = new UpdateTarifaDto
            {
                IdTarifa = tarifaId,
                PrecioPorNoche = 150.00m,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
                IdCategoria = 1
            };
            
            var operationResult = OperationResult.Success();
            
            _mockTarifaService
                .Setup(service => service.Update(It.IsAny<UpdateTarifaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(tarifaId, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            int tarifaId = 1;
            var updateDto = new UpdateTarifaDto
            {
                IdTarifa = 2, 
                PrecioPorNoche = 150.00m,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
                IdCategoria = 1
            };

            // Act
            var result = await _controller.Update(tarifaId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int tarifaId = 1;
            var updateDto = new UpdateTarifaDto
            {
                IdTarifa = tarifaId,
            };
            
            _controller.ModelState.AddModelError("Precio", "El campo Precio es obligatorio");

            // Act
            var result = await _controller.Update(tarifaId, updateDto);

            // Assert
            Assert.IsType<ValidationProblemDetails>(
                (result as ObjectResult)?.Value
            );
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenTarifaDoesNotExist()
        {
            // Arrange
            int tarifaId = 999;
            var updateDto = new UpdateTarifaDto
            {
                IdTarifa = tarifaId,
                PrecioPorNoche = 150.00m,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
                IdCategoria = 1
            };
            
            var operationResult = OperationResult.Failure($"No se encontró la tarifa con ID: {tarifaId}");
            
            _mockTarifaService
                .Setup(service => service.Update(It.IsAny<UpdateTarifaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(tarifaId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenDeleteSuccessful()
        {
            // Arrange
            int tarifaId = 1;
            
            var operationResult = OperationResult.Success();
            
            _mockTarifaService
                .Setup(service => service.Remove(It.IsAny<DeleteTarifaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(tarifaId);

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
        public async Task Delete_ReturnsBadRequest_WhenTarifaDoesNotExist()
        {
            // Arrange
            int tarifaId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró la tarifa con ID: {tarifaId}");
            
            _mockTarifaService
                .Setup(service => service.Remove(It.IsAny<DeleteTarifaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(tarifaId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}