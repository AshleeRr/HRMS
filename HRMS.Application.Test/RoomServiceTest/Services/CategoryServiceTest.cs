using System.Linq.Expressions;
using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Interfaces.IServicioRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.Services
{
    public class CategoriaServicesTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IHabitacionRepository> _mockHabitacionRepository;
        private readonly Mock<IServicioRepository> _mockServicioRepository;
        private readonly Mock<ITarifaRepository> _mockTarifaRepository;
        private readonly Mock<IValidator<CreateCategoriaDto>> _mockValidator;
        private readonly Mock<ILogger<CategoriaServices>> _mockLogger;
        private readonly CategoriaServices _categoriaServices;

        public CategoriaServicesTests()
        {
            // Configuración de mocks
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockHabitacionRepository = new Mock<IHabitacionRepository>();
            _mockServicioRepository = new Mock<IServicioRepository>();
            _mockTarifaRepository = new Mock<ITarifaRepository>();
            _mockValidator = new Mock<IValidator<CreateCategoriaDto>>();
            _mockLogger = new Mock<ILogger<CategoriaServices>>();

            // Instancia del servicio con los mocks
            _categoriaServices = new CategoriaServices(
                _mockCategoryRepository.Object,
                _mockLogger.Object,
                _mockHabitacionRepository.Object,
                _mockServicioRepository.Object,
                _mockTarifaRepository.Object,
                _mockValidator.Object
            );
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_CategoriesExist_ReturnsSuccess()
        {
            // Arrange
            var categories = new List<Categoria>
            {
                new Categoria { IdCategoria = 1, Descripcion = "Categoría 1", Estado = true },
                new Categoria { IdCategoria = 2, Descripcion = "Categoría 2", Estado = true }
            };

            _mockCategoryRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(categories);

            // La lista de DTOs que esperamos recibir
            var expectedDtos = categories.Select(c => new CategoriaDto 
            { 
                Descripcion = c.Descripcion,
                Capacidad = c.Capacidad
            }).ToList();

            // Act
            var result = await _categoriaServices.GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categorías obtenidas correctamente.", result.Message);
    
            var resultDtos = Assert.IsType<List<CategoriaDto>>(result.Data);
            Assert.Equal(expectedDtos.Count, resultDtos.Count);
    
            for (int i = 0; i < expectedDtos.Count; i++)
            {
                Assert.Equal(expectedDtos[i].Descripcion, resultDtos[i].Descripcion);
                Assert.Equal(expectedDtos[i].Capacidad, resultDtos[i].Capacidad);
            }
        }

        [Fact]
        public async Task GetAll_NoCategoriesExist_ReturnsFailure()
        {
            // Arrange
            _mockCategoryRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Categoria>());

            // Act
            var result = await _categoriaServices.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontraron categorías.", result.Message);
        }

        [Fact]
        public async Task GetAll_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockCategoryRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new Exception("Error de conexión"));

            // Act
            var result = await _categoriaServices.GetAll();

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
            var category = new Categoria { 
                IdCategoria = id, 
                Descripcion = "Categoría Test", 
                Estado = true 
            };

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(id))
                .ReturnsAsync(category);

            // Act
            var result = await _categoriaServices.GetById(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categoría obtenida correctamente.", result.Message);
    
            var categoryDto = Assert.IsType<CategoriaDto>(result.Data);
            Assert.Equal(category.Descripcion, categoryDto.Descripcion);
            Assert.Equal(category.Capacidad, categoryDto.Capacidad);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_InvalidId_ReturnsFailure(int id)
        {
            // Act
            var result = await _categoriaServices.GetById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la categoría debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task GetById_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            int id = 999;
            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(id))
                .ReturnsAsync((Categoria)null);

            // Act
            var result = await _categoriaServices.GetById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"No se encontró la categoría con ID {id}", result.Message);
        }

        [Fact]
        public async Task GetById_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            int id = 1;
            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(id))
                .ThrowsAsync(new Exception("Error al buscar la categoría"));

            // Act
            var result = await _categoriaServices.GetById(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar la categoría", result.Message);
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Nueva Categoría",
                Capacidad = 2,
                IdServicio = 1
            };

            var servicio = new Servicios { IdServicio = 1, Estado = true };
            var validationResult = new OperationResult { IsSuccess = true };
            var categoria = new Categoria
            {
                Descripcion = dto.Descripcion,
                Capacidad = dto.Capacidad,
                IdServicio = dto.IdServicio
            };
            var operationResult = OperationResult.Success(categoria, "Categoría guardada correctamente.");

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateCategoriaDto>()))
                .Returns(validationResult);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            _mockCategoryRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(false);

            _mockCategoryRepository.Setup(repo => repo.SaveEntityAsync(It.IsAny<Categoria>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _categoriaServices.Save(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categoría creada exitosamente.", result.Message);
            Assert.NotNull(result.Data);
        }


        [Fact]
        public async Task Save_ValidationFails_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto();
            var validationResult = new OperationResult { IsSuccess = false, Message = "Errores de validación" };

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateCategoriaDto>()))
                .Returns(validationResult);

            // Act
            var result = await _categoriaServices.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Errores de validación", result.Message);
        }

        [Fact]
        public async Task Save_ServicioNotExists_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Nueva Categoría",
                Capacidad = 2,
                IdServicio = 999
            };

            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateCategoriaDto>()))
                .Returns(validationResult);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync((Servicios)null);

            // Act
            var result = await _categoriaServices.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"El servicio con ID {dto.IdServicio} no existe.", result.Message);
        }

        [Fact]
        public async Task Save_ServicioInactive_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Nueva Categoría",
                Capacidad = 2,
                IdServicio = 1
            };

            var servicio = new Servicios { IdServicio = 1, Estado = false };
            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateCategoriaDto>()))
                .Returns(validationResult);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            // Act
            var result = await _categoriaServices.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"El servicio con ID {dto.IdServicio} está inactivo.", result.Message);
        }

        [Fact]
        public async Task Save_DuplicateDescription_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Categoría Existente",
                Capacidad = 2,
                IdServicio = 1
            };

            var servicio = new Servicios { IdServicio = 1, Estado = true };
            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateCategoriaDto>()))
                .Returns(validationResult);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            _mockCategoryRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _categoriaServices.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe una categoría con la descripción '{dto.Descripcion}'.", result.Message);
        }

        [Fact]
        public async Task Save_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Nueva Categoría",
                Capacidad = 2,
                IdServicio = 1
            };

            var servicio = new Servicios { IdServicio = 1, Estado = true };
            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateCategoriaDto>()))
                .Returns(validationResult);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            _mockCategoryRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(false);

            _mockCategoryRepository.Setup(repo => repo.SaveEntityAsync(It.IsAny<Categoria>()))
                .ThrowsAsync(new Exception("Error al guardar la categoría"));

            // Act
            var result = await _categoriaServices.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar la categoría", result.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new UpdateCategoriaDto
            {
                IdCategoria = 1,
                Descripcion = "Categoría Actualizada",
                Capacidad = 3,
                IdServicio = 1
            };

            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría Original",
                Capacidad = 2,
                IdServicio = 1,
                Estado = true
            };

            var servicio = new Servicios { IdServicio = 1, Estado = true };
            var validationResult = new OperationResult { IsSuccess = true };
            var operationResult = OperationResult.Success(category, "Categoría actualizada correctamente.");

            _mockValidator.Setup(v => v.Validate(It.IsAny<UpdateCategoriaDto>()))
                .Returns(validationResult);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            _mockCategoryRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(false);

            _mockCategoryRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Categoria>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categoría actualizada correctamente.", result.Message);
            Assert.NotNull(result.Data);
            var updatedCategory = (CategoriaDto)result.Data;
            Assert.Equal(dto.Descripcion, updatedCategory.Descripcion);
            Assert.Equal(dto.Capacidad, updatedCategory.Capacidad);
            Assert.Equal(dto.IdServicio, updatedCategory.IdServicio);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Update_InvalidId_ReturnsFailure(int id)
        {
            // Arrange
            var dto = new UpdateCategoriaDto { IdCategoria = id };

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la categoría debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task Update_ValidationFails_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateCategoriaDto { IdCategoria = 1 };
            var validationResult = new OperationResult { IsSuccess = false, Message = "Errores de validación" };

            _mockValidator.Setup(v => v.Validate(It.IsAny<UpdateCategoriaDto>()))
                .Returns(validationResult);

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Errores de validación", result.Message);
        }

        [Fact]
        public async Task Update_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateCategoriaDto
            {
                IdCategoria = 999,
                Descripcion = "Categoría Actualizada",
                Capacidad = 3,
                IdServicio = 1
            };

            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<UpdateCategoriaDto>()))
                .Returns(validationResult);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync((Categoria)null);

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"No se encontró la categoría con ID {dto.IdCategoria}.", result.Message);
        }

        [Fact]
        public async Task Update_ServicioNotExists_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateCategoriaDto
            {
                IdCategoria = 1,
                Descripcion = "Categoría Actualizada",
                Capacidad = 3,
                IdServicio = 999
            };

            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría Original",
                Capacidad = 2,
                IdServicio = 1,
                Estado = true
            };

            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<UpdateCategoriaDto>()))
                .Returns(validationResult);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync((Servicios)null);

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"El servicio con ID {dto.IdServicio} no existe.", result.Message);
        }

        [Fact]
        public async Task Update_ServicioInactive_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateCategoriaDto
            {
                IdCategoria = 1,
                Descripcion = "Categoría Actualizada",
                Capacidad = 3,
                IdServicio = 1
            };

            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría Original",
                Capacidad = 2,
                IdServicio = 1,
                Estado = true
            };

            var servicio = new Servicios { IdServicio = 1, Estado = false };
            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<UpdateCategoriaDto>()))
                .Returns(validationResult);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"El servicio con ID {dto.IdServicio} está inactivo.", result.Message);
        }

        [Fact]
        public async Task Update_DuplicateDescription_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateCategoriaDto
            {
                IdCategoria = 1,
                Descripcion = "Categoría Duplicada",
                Capacidad = 3,
                IdServicio = 1
            };

            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría Original",
                Capacidad = 2,
                IdServicio = 1,
                Estado = true
            };

            var servicio = new Servicios { IdServicio = 1, Estado = true };
            var validationResult = new OperationResult { IsSuccess = true };

            _mockValidator.Setup(v => v.Validate(It.IsAny<UpdateCategoriaDto>()))
                .Returns(validationResult);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockServicioRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdServicio))
                .ReturnsAsync(servicio);

            _mockCategoryRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _categoriaServices.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe otra categoría con la descripción '{dto.Descripcion}'.", result.Message);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new DeleteCategoriaDto { IdCategoria = 1 };
            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría a Eliminar",
                Capacidad = 4, 
                Estado = true
            };
            
            var categoryDto = new CategoriaDto
            {
                Descripcion = category.Descripcion,
                Capacidad = category.Capacidad
            };

            var operationResult = OperationResult.Success(category, "Categoría actualizada correctamente.");

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);

            _mockTarifaRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Tarifas, bool>>>()))
                .ReturnsAsync(false);

            _mockCategoryRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Categoria>()))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _categoriaServices.Remove(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categoría eliminada correctamente.", result.Message);
            Assert.NotNull(result.Data);

            var removedCategoryDto = Assert.IsType<CategoriaDto>(result.Data);

            Assert.Equal(category.Descripcion, removedCategoryDto.Descripcion);
            Assert.Equal(category.Capacidad, removedCategoryDto.Capacidad);


            _mockCategoryRepository.Verify(repo => repo.UpdateEntityAsync(It.Is<Categoria>(c =>
                c.IdCategoria == dto.IdCategoria && c.Estado == false)), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Remove_InvalidId_ReturnsFailure(int id)
        {
            // Arrange
            var dto = new DeleteCategoriaDto { IdCategoria = id };

            // Act
            var result = await _categoriaServices.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la categoría debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task Remove_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteCategoriaDto { IdCategoria = 999 };

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync((Categoria)null);

            // Act
            var result = await _categoriaServices.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"No se encontró la categoría con ID {dto.IdCategoria}", result.Message);
        }

        [Fact]
        public async Task Remove_WithAssociatedRooms_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteCategoriaDto { IdCategoria = 1 };
            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría con Habitaciones",
                Estado = true
            };

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _categoriaServices.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se puede eliminar la categoría porque tiene habitaciones asociadas.", result.Message);
        }

        [Fact]
        public async Task Remove_WithAssociatedRates_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteCategoriaDto { IdCategoria = 1 };
            var category = new Categoria
            {
                IdCategoria = 1,
                Descripcion = "Categoría con Tarifas",
                Estado = true
            };

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(dto.IdCategoria))
                .ReturnsAsync(category);

            _mockHabitacionRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);

            _mockTarifaRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Tarifas, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _categoriaServices.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se puede eliminar la categoría porque tiene tarifas activas asociadas.", result.Message);
        }

        #endregion

        #region GetCategoriaByServicio Tests

        [Fact]
        public async Task GetCategoriaByServicio_ValidName_ReturnsSuccess()
        {
            // Arrange
            string nombreServicio = "Servicio Test";
            var categorias = new List<Categoria>
            {
                new Categoria
                {
                    IdCategoria = 1,
                    Descripcion = "Categoría de Servicio",
                    Estado = true
                }
            };

            var repositoryResult = OperationResult.Success(categorias, null);

            _mockCategoryRepository.Setup(repo => repo.GetCategoriaByServiciosAsync(nombreServicio))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _categoriaServices.GetCategoriaByServicio(nombreServicio);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categoría encontrada.", result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetCategoriaByServicio_EmptyName_ReturnsFailure(string nombreServicio)
        {
            // Act
            var result = await _categoriaServices.GetCategoriaByServicio(nombreServicio);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El nombre del servicio no puede estar vacío.", result.Message);
        }

        [Fact]
        public async Task GetCategoriaByServicio_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            string nombreServicio = "Servicio Inexistente";

            var emptyList = new List<Categoria>();
            var repositoryResult = OperationResult.Success(emptyList,
                $"No se encontraron categorías para el servicio '{nombreServicio}'.");

            _mockCategoryRepository.Setup(repo => repo.GetCategoriaByServiciosAsync(nombreServicio))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _categoriaServices.GetCategoriaByServicio(nombreServicio);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontró una categoría para el servicio indicado.", result.Message);
        }

        #endregion

        #region GetCategoriaByDescripcion Tests

        [Fact]
        public async Task GetCategoriaByDescripcion_ValidDescription_ReturnsSuccess()
        {
            // Arrange
            string descripcion = "Categoría Buscada";
            var operationResult = OperationResult.Success(new Categoria(), "Categoría encontrada.");

            _mockCategoryRepository.Setup(repo => repo.GetCategoriaByDescripcionAsync(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _categoriaServices.GetCategoriaByDescripcion(descripcion);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Categoría encontrada.", result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetCategoriaByDescripcion_EmptyDescription_ReturnsFailure(string descripcion)
        {
            // Act
            var result = await _categoriaServices.GetCategoriaByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción de la categoría no puede estar vacía.", result.Message);
        }

        [Fact]
        public async Task GetCategoriaByDescripcion_CategoryNotFound_ReturnsFailure()
        {
            // Arrange
            string descripcion = "Categoría Inexistente";
            var operationResult = OperationResult.Failure("No se encontró la categoría.");

            _mockCategoryRepository.Setup(repo => repo.GetCategoriaByDescripcionAsync(descripcion))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _categoriaServices.GetCategoriaByDescripcion(descripcion);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontró la categoría.", result.Message);
        }

        #endregion

        #region GetHabitacionesByCapacidad Tests

        [Fact]
        public async Task GetHabitacionesByCapacidad_ValidCapacity_ReturnsSuccess()
        {
            // Arrange
            int capacidad = 2;
            var habitaciones = new List<Habitacion>
            {
                new Habitacion { IdHabitacion = 1, Numero = "101", Estado = true },
                new Habitacion { IdHabitacion = 2, Numero = "102", Estado = true }
            };

            var repositoryResult = OperationResult.Success(habitaciones, "Habitaciones encontradas");

            _mockCategoryRepository.Setup(repo => repo.GetHabitacionByCapacidad(capacidad))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _categoriaServices.GetHabitacionesByCapacidad(capacidad);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitaciones obtenidas correctamente.", result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetHabitacionesByCapacidad_InvalidCapacity_ReturnsFailure(int capacidad)
        {
            // Act
            var result = await _categoriaServices.GetHabitacionesByCapacidad(capacidad);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La capacidad de la habitación debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task GetHabitacionesByCapacidad_RoomsNotFound_ReturnsFailure()
        {
            // Arrange
            int capacidad = 10;

            var repositoryResult =
                OperationResult.Failure($"No se encontraron habitaciones con capacidad para {capacidad} personas.");

            _mockCategoryRepository.Setup(repo => repo.GetHabitacionByCapacidad(capacidad))
                .ReturnsAsync(repositoryResult);

            // Act
            var result = await _categoriaServices.GetHabitacionesByCapacidad(capacidad);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontraron habitaciones con la capacidad indicada.", result.Message);
        }

        #endregion
    }
}