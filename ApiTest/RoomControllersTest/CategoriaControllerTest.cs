using HRMS.APIs.Controllers.RoomManagementControllers;
using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiTest.RoomControllersTest
{
    public class CategoriaControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<ILogger<CategoriaController>> _mockLogger;
        private readonly CategoriaController _controller;

        public CategoriaControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _mockLogger = new Mock<ILogger<CategoriaController>>();
            _controller = new CategoriaController(_mockCategoryService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenCategoriesExist()
        {
            // Arrange
            var categorias = new List<CategoriaDto>
            {
                new CategoriaDto { IdCategoria = 1, Descripcion = "Categoria 1" },
                new CategoriaDto { IdCategoria = 2, Descripcion = "Categoria 2" }
            };
            
            var operationResult = OperationResult.Success(categorias);
            
            _mockCategoryService
                .Setup(service => service.GetAll())
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoriaDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<CategoriaDto>).Count);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenNoCategoriesExist()
        {
            // Arrange
            var operationResult = OperationResult.Failure("No se encontraron categorías");
            
            _mockCategoryService
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
        public async Task GetById_ReturnsOkResult_WhenCategoriaExists()
        {
            // Arrange
            int categoriaId = 1;
            var categoria = new CategoriaDto { IdCategoria = 1, Descripcion = "Categoria 1" };
            
            var operationResult = OperationResult.Success(categoria);
            
            _mockCategoryService
                .Setup(service => service.GetById(categoriaId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(categoriaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoriaDto>(okResult.Value);
            Assert.Equal(categoriaId, returnValue.IdCategoria);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenCategoriaDoesNotExist()
        {
            // Arrange
            int categoriaId = 999;
    
            var operationResult = OperationResult.Failure($"No se encontró la categoría con ID: {categoriaId}");
    
            _mockCategoryService
                .Setup(service => service.GetById(categoriaId))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetById(categoriaId);

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

        #region Create Tests

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenModelStateIsValid()
        {
            // Arrange
            var createDto = new CreateCategoriaDto
            {
                Descripcion = "Nueva Categoria",
                Capacidad = 4
            };
            
            var categoriaDto = new CategoriaDto
            {
                IdCategoria = 1,
                Descripcion = "Nueva Categoria",
                Capacidad = 4,
            };
            
            var operationResult = OperationResult.Success(categoriaDto);
            
            _mockCategoryService
                .Setup(service => service.Save(It.IsAny<CreateCategoriaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CategoriaController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(categoriaDto.IdCategoria, createdAtActionResult.RouteValues["id"]);
            var returnValue = Assert.IsType<CategoriaDto>(createdAtActionResult.Value);
            Assert.Equal(categoriaDto.IdCategoria, returnValue.IdCategoria);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateCategoriaDto
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
            var createDto = new CreateCategoriaDto
            {
                Descripcion = "Nueva Categoria",
                Capacidad = 4
            };
            
            var operationResult = OperationResult.Failure("Error al crear la categoría");
            
            _mockCategoryService
                .Setup(service => service.Save(It.IsAny<CreateCategoriaDto>()))
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
            int categoriaId = 1;
            var updateDto = new UpdateCategoriaDto
            {
                IdCategoria = categoriaId,
                Descripcion = "Categoria Actualizada",
                Capacidad = 5
            };
            
            var operationResult = OperationResult.Success();
            
            _mockCategoryService
                .Setup(service => service.Update(It.IsAny<UpdateCategoriaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(categoriaId, updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            int categoriaId = 1;
            var updateDto = new UpdateCategoriaDto
            {
                IdCategoria = 2,
                Descripcion = "Categoria Actualizada",
                Capacidad = 5
            };

            // Act
            var result = await _controller.Update(categoriaId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int categoriaId = 1;
            var updateDto = new UpdateCategoriaDto
            {
                IdCategoria = categoriaId,
            };
            
            _controller.ModelState.AddModelError("Descripcion", "El campo Descripcion es obligatorio");

            // Act
            var result = await _controller.Update(categoriaId, updateDto);

            // Assert
            Assert.IsType<ValidationProblemDetails>(
                (result as ObjectResult)?.Value
            );
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenCategoriaDoesNotExist()
        {
            // Arrange
            int categoriaId = 999;
            var updateDto = new UpdateCategoriaDto
            {
                IdCategoria = categoriaId,
                Descripcion = "Categoria Actualizada",
                Capacidad = 5
            };
            
            var operationResult = OperationResult.Failure($"No se encontró la categoría con ID: {categoriaId}");
            
            _mockCategoryService
                .Setup(service => service.Update(It.IsAny<UpdateCategoriaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Update(categoriaId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenDeleteSuccessful()
        {
            // Arrange
            int categoriaId = 1;
            
            var operationResult = OperationResult.Success();
            
            _mockCategoryService
                .Setup(service => service.Remove(It.IsAny<DeleteCategoriaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(categoriaId);

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
        public async Task Delete_ReturnsNotFound_WhenCategoriaDoesNotExist()
        {
            // Arrange
            int categoriaId = 999;
            
            var operationResult = OperationResult.Failure($"No se encontró la categoría con ID: {categoriaId}");
            
            _mockCategoryService
                .Setup(service => service.Remove(It.IsAny<DeleteCategoriaDto>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(categoriaId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetByServicio Tests

        [Fact]
        public async Task GetByServicio_ReturnsOkResult_WhenCategoriasExist()
        {
            // Arrange
            string nombreServicio = "Servicio1";
            
            var categorias = new List<CategoriaDto>
            {
                new CategoriaDto { IdCategoria = 1, Descripcion = "Categoria 1" },
                new CategoriaDto { IdCategoria = 2, Descripcion = "Categoria 2" }
            };
            
            var operationResult = OperationResult.Success(categorias);
            
            _mockCategoryService
                .Setup(service => service.GetCategoriaByServicio(nombreServicio))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByServicio(nombreServicio);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoriaDto>>(okResult.Value);
            Assert.Equal(2, (returnValue as List<CategoriaDto>).Count);
        }

        [Fact]
        public async Task GetByServicio_ReturnsBadRequest_WhenNombreServicioIsInvalid()
        {
            // Arrange
            string invalidNombreServicio = "";

            // Act
            var result = await _controller.GetByServicio(invalidNombreServicio);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByServicio_ReturnsNotFound_WhenNoCategoriasExist()
        {
            // Arrange
            string nombreServicio = "ServicioInexistente";
            
            var operationResult = OperationResult.Failure($"No se encontraron categorías con el servicio: {nombreServicio}");
            
            _mockCategoryService
                .Setup(service => service.GetCategoriaByServicio(nombreServicio))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetByServicio(nombreServicio);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetServiciosByDescripcion Tests

        [Fact]
        public async Task GetServiciosByDescripcion_ReturnsOkResult_WhenCategoriaExists()
        {
            // Arrange
            string descripcion = "Categoria1";
            
            var categoria = new CategoriaDto { IdCategoria = 1, Descripcion = "Categoria1" };
            
            var operationResult = OperationResult.Success(categoria);
            
            _mockCategoryService
                .Setup(service => service.GetCategoriaByDescripcion(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetServiciosByDescripcion(descripcion);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoriaDto>(okResult.Value);
            Assert.Equal(descripcion, returnValue.Descripcion);
        }

        [Fact]
        public async Task GetServiciosByDescripcion_ReturnsBadRequest_WhenDescripcionIsInvalid()
        {
            // Arrange
            string invalidDescripcion = "";

            // Act
            var result = await _controller.GetServiciosByDescripcion(invalidDescripcion);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetServiciosByDescripcion_ReturnsNotFound_WhenCategoriaDoesNotExist()
        {
            // Arrange
            string descripcion = "CategoriaInexistente";
            
            var operationResult = OperationResult.Failure($"No se encontró la categoría con descripción: {descripcion}");
            
            _mockCategoryService
                .Setup(service => service.GetCategoriaByDescripcion(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetServiciosByDescripcion(descripcion);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetHabitacionesByCapacidad Tests

        [Fact]
        public async Task GetHabitacionesByCapacidad_ReturnsOkResult_WhenHabitacionesExist()
        {
            // Arrange
            int capacidad = 4;
            
            var habitaciones = new List<object>(); 
            
            var operationResult = OperationResult.Success(habitaciones);
            
            _mockCategoryService
                .Setup(service => service.GetHabitacionesByCapacidad(capacidad))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetHabitacionesByCapacidad(capacidad);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetHabitacionesByCapacidad_ReturnsBadRequest_WhenCapacidadIsInvalid()
        {
            // Arrange
            int invalidCapacidad = 0;

            // Act
            var result = await _controller.GetHabitacionesByCapacidad(invalidCapacidad);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetHabitacionesByCapacidad_ReturnsNotFound_WhenNoHabitacionesExist()
        {
            // Arrange
            int capacidad = 999;
            
            var operationResult = OperationResult.Failure($"No se encontraron habitaciones con capacidad: {capacidad}");
            
            _mockCategoryService
                .Setup(service => service.GetHabitacionesByCapacidad(capacidad))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _controller.GetHabitacionesByCapacidad(capacidad);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion
    }
}