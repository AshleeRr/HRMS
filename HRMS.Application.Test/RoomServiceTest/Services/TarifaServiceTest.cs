using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.Services
{
    public class TarifaServicesTests
    {
        private readonly Mock<ITarifaRepository> _mockTarifaRepository;
        private readonly Mock<ILogger<TarifaServices>> _mockLogger;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IValidator<CreateTarifaDto>> _mockValidator;
        private readonly TarifaServices _service;

        public TarifaServicesTests()
        {
            // Arrange - Configuración global
            _mockTarifaRepository = new Mock<ITarifaRepository>();
            _mockLogger = new Mock<ILogger<TarifaServices>>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockValidator = new Mock<IValidator<CreateTarifaDto>>();

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Success());
            _service = new TarifaServices(
                _mockTarifaRepository.Object,
                _mockLogger.Object,
                _mockCategoryRepository.Object,
                _mockValidator.Object
            );
        }

        private static Tarifas CreateSampleTarifa(int id = 1)
        {
            return new Tarifas
            {
                IdTarifa = id,
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                Estado = true,
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
        }

        private static CreateTarifaDto CreateSampleCreateDto()
        {
            return new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
        }

        private static UpdateTarifaDto CreateSampleUpdateDto(int id = 1)
        {
            return new UpdateTarifaDto
            {
                IdTarifa = id,
                IdCategoria = 1,
                Descripcion = "Tarifa actualizada",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 120.0m,
                Descuento = 5
            };
        }

        private static DeleteTarifaDto CreateSampleDeleteDto(int id = 1)
        {
            return new DeleteTarifaDto
            {
                IdTarifa = id
            };
        }

        private static Categoria CreateSampleCategory(int id = 1)
        {
            return new Categoria()
            {
                IdCategoria = id,
                Descripcion = "Categoría de prueba",
                Estado = true
            };
        }

        private static Habitacion CreateSampleHabitacion(int id = 1)
        {
            return new Habitacion
            {
                IdHabitacion = id,
                Numero = "101",
                Estado = true
            };
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_WhenTarifasExist_ReturnsSuccess()
        {
            // Arrange
            var tarifas = new List<Tarifas>
            {
                CreateSampleTarifa(1),
                CreateSampleTarifa(2)
            };

            _mockTarifaRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tarifas);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            var enumerable = (result.Data as IEnumerable<object>);
            Assert.NotNull(enumerable);
            Assert.Equal(2, enumerable.Count());
        }

        [Fact]
        public async Task GetAll_WhenNoTarifasExist_ReturnsFailure()
        {
            // Arrange
            _mockTarifaRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Tarifas>());

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontraron tarifas.", result.Message);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsSuccess()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(1))
                .ReturnsAsync(tarifa);

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsFailure()
        {
            // Act
            var result = await _service.GetById(0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la tarifa debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task GetById_WithNonExistentId_ReturnsFailure()
        {
            // Arrange
            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(999))
                .ReturnsAsync((Tarifas)null);

            // Act
            var result = await _service.GetById(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontró la tarifa con ID 999.", result.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            var updateDto = CreateSampleUpdateDto();

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdCategoria))
                .ReturnsAsync(CreateSampleCategory());

            _mockTarifaRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Tarifas>()))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = tarifa });

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Tarifa actualizada correctamente.", result.Message);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var updateDto = CreateSampleUpdateDto(0);

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la tarifa debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task Update_WithNonExistentTarifa_ReturnsFailure()
        {
            // Arrange
            var updateDto = CreateSampleUpdateDto(999);

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync((Tarifas)null);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdCategoria))
                .ReturnsAsync(CreateSampleCategory());

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontró la tarifa con ID 999.", result.Message);
        }

        [Fact]
        public async Task Update_WithNonExistentCategoria_ReturnsFailure()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            var updateDto = CreateSampleUpdateDto();
            updateDto.IdCategoria = 999;

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdCategoria))
                .ReturnsAsync((Categoria)null);

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No existe una categoría con ID 999 en la base de datos.", result.Message);
        }

        [Fact]
        public async Task Update_WithInactiveCategoria_ReturnsFailure()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            var updateDto = CreateSampleUpdateDto();

            var inactiveCategory = CreateSampleCategory();
            inactiveCategory.Estado = false;

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdCategoria))
                .ReturnsAsync(inactiveCategory);

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La categoría con ID 1 está inactiva. Active la categoría antes de asociarla a una tarifa.",
                result.Message);
        }

        [Fact]
        public async Task Update_WithInvalidDto_ReturnsFailure()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            var updateDto = CreateSampleUpdateDto();
            updateDto.PrecioPorNoche = -50; // Precio inválido

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdCategoria))
                .ReturnsAsync(CreateSampleCategory());

            _mockValidator.Setup<OperationResult>(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("El precio por noche debe ser mayor que 0"));

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio por noche debe ser mayor que 0", result.Message);
        }
        
        #endregion

        #region Save Tests
        
        [Fact]
        public async Task Save_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba válida",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
    
            var savedTarifa = CreateSampleTarifa();

            var mockValidator = new Mock<IValidator<CreateTarifaDto>>();
            mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Success());

            var serviceWithMockedValidator = new TarifaServices(
                _mockTarifaRepository.Object,
                _mockLogger.Object,
                _mockCategoryRepository.Object,
                mockValidator.Object
            );

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(createDto.IdCategoria))
                .ReturnsAsync(CreateSampleCategory());

            _mockTarifaRepository.Setup(repo => repo.SaveEntityAsync(It.IsAny<Tarifas>()))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = savedTarifa });

            // Act
            var result = await serviceWithMockedValidator.Save(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Tarifa creada correctamente.", result.Message);
        }

        [Fact]
        public async Task Save_WithInvalidDto_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.PrecioPorNoche = -50; // Precio inválido

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("El precio por noche debe ser mayor que 0"));

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio por noche debe ser mayor que 0", result.Message);
        }
        
        [Fact]
        public async Task Save_WithNonExistentCategoria_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.IdCategoria = 999;

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(createDto.IdCategoria))
                .ReturnsAsync((Categoria)null);

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No existe una categoría con ID 999 en la base de datos.", result.Message);
        }

        [Fact]
        public async Task Save_WithInactiveCategoria_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();

            var inactiveCategory = CreateSampleCategory();
            inactiveCategory.Estado = false;

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(createDto.IdCategoria))
                .ReturnsAsync(inactiveCategory);

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La categoría con ID 1 está inactiva. Active la categoría antes de asociarla a una tarifa.",
                result.Message);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var deleteDto = CreateSampleDeleteDto();
            var tarifa = CreateSampleTarifa();

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(deleteDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockTarifaRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Tarifas>()))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = tarifa });

            // Act
            var result = await _service.Remove(deleteDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Tarifa eliminada correctamente.", result.Message);
        }

        [Fact]
        public async Task Remove_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var deleteDto = CreateSampleDeleteDto(0);

            // Act
            var result = await _service.Remove(deleteDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la tarifa debe ser mayor que 0.", result.Message);
        }

        [Fact]
        public async Task Remove_WithNonExistentTarifa_ReturnsFailure()
        {
            // Arrange
            var deleteDto = CreateSampleDeleteDto(999);

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(deleteDto.IdTarifa))
                .ReturnsAsync((Tarifas)null);

            // Act
            var result = await _service.Remove(deleteDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontró la tarifa con ID 999.", result.Message);
        }

        #endregion

        #region GetTarifasVigentes Tests

        [Fact]
        public async Task GetTarifasVigentes_WithValidDate_ReturnsSuccess()
        {
            // Arrange
            string fechaInput = DateTime.Now.ToString("dd/MM/yyyy");
            var tarifas = new List<Tarifas> { CreateSampleTarifa() };

            _mockTarifaRepository.Setup(repo => repo.GetTarifasVigentesAsync(fechaInput))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = tarifas });

            // Act
            var result = await _service.GetTarifasVigentes(fechaInput);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetTarifasVigentes_WithInvalidDate_ReturnsFailure()
        {
            // Arrange
            string invalidDate = "not-a-date";

            // Act
            var result = await _service.GetTarifasVigentes(invalidDate);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El formato de la fecha", result.Message);
        }

        [Fact]
        public async Task GetTarifasVigentes_WithEmptyDate_ReturnsFailure()
        {
            // Arrange
            string emptyDate = "";

            // Act
            var result = await _service.GetTarifasVigentes(emptyDate);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La fecha no puede estar vacía.", result.Message);
        }

        #endregion

        #region GetHabitacionesByPrecio Tests

        [Fact]
        public async Task GetHabitacionesByPrecio_WithValidPrice_ReturnsSuccess()
        {
            // Arrange
            decimal precio = 100.0m;
            var habitaciones = new List<Habitacion> { CreateSampleHabitacion() };

            _mockTarifaRepository.Setup(repo => repo.GetHabitacionByPrecioAsync(precio))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = habitaciones });

            // Act
            var result = await _service.GetHabitacionesByPrecio(precio);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetHabitacionesByPrecio_WithInvalidPrice_ReturnsFailure()
        {
            // Arrange
            decimal invalidPrice = -50.0m;

            // Act
            var result = await _service.GetHabitacionesByPrecio(invalidPrice);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio de la tarifa debe ser mayor a 0.", result.Message);
        }

        [Fact]
        public async Task GetHabitacionesByPrecio_WithNoResults_ReturnsFailure()
        {
            // Arrange
            decimal precio = 999.0m;

            _mockTarifaRepository.Setup(repo => repo.GetHabitacionByPrecioAsync(precio))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = new List<Habitacion>() });

            // Act
            var result = await _service.GetHabitacionesByPrecio(precio);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No se encontraron habitaciones con el precio de tarifa indicado.", result.Message);
        }

        #endregion

        #region Edge Cases - Validador

        [Fact]
        public async Task Save_WithNegativeDiscount_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.Descuento = -5;

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("El descuento no puede ser negativo"));

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El descuento no puede ser negativo", result.Message);
        }

        [Fact]
        public async Task Save_WithTooLongDescription_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.Descripcion = new string('A', 256); 

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("La descripción no puede tener más de 255 caracteres"));

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede tener más de 255 caracteres", result.Message);
        }
        
        [Fact]
        public async Task Save_WithTooShortDescription_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.Descripcion = "AB"; 

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("La descripción debe tener al menos 3 caracteres"));

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción debe tener al menos 3 caracteres", result.Message);
        }

        [Fact]
        public async Task Save_WithStartDateBeforeToday_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.FechaInicio = DateTime.Now.AddDays(-1); 

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("La fecha de inicio tiene que ser mayor a la fecha actual"));

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La fecha de inicio tiene que ser mayor a la fecha actual", result.Message);
        }

        [Fact]
        public async Task Save_WithEndDateBeforeStartDate_ReturnsFailure()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();
            createDto.FechaFin = createDto.FechaInicio.AddDays(-1);

            _mockValidator.Setup(v => v.Validate(It.IsAny<CreateTarifaDto>()))
                .Returns(OperationResult.Failure("La fecha de inicio debe ser menor a la fecha de fin"));

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La fecha de inicio debe ser menor a la fecha de fin", result.Message);
        }
        
        #endregion

        #region Edge Cases - Manejo de Errores

        [Fact]
        public async Task Update_WithIdCategoriaZero_SkipsCategoriaValidation()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            var updateDto = CreateSampleUpdateDto();
            updateDto.IdCategoria = 0; 

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockTarifaRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Tarifas>()))
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = tarifa });

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockCategoryRepository.Verify(repo => repo.GetEntityByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Save_WithRepositoryError_PropagatesError()
        {
            // Arrange
            var createDto = CreateSampleCreateDto();

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(createDto.IdCategoria))
                .ReturnsAsync(CreateSampleCategory());

            _mockTarifaRepository.Setup(repo => repo.SaveEntityAsync(It.IsAny<Tarifas>()))
                .ReturnsAsync(new OperationResult { IsSuccess = false, Message = "Error de base de datos" });

            // Act
            var result = await _service.Save(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error de base de datos", result.Message);
        }

        [Fact]
        public async Task Update_WithRepositoryUpdateError_PropagatesError()
        {
            // Arrange
            var tarifa = CreateSampleTarifa();
            var updateDto = CreateSampleUpdateDto();

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(tarifa);

            _mockCategoryRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdCategoria))
                .ReturnsAsync(CreateSampleCategory());

            _mockTarifaRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Tarifas>()))
                .ReturnsAsync(new OperationResult
                    { IsSuccess = false, Message = "Error al actualizar en base de datos" });

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error al actualizar en base de datos", result.Message);
        }

        [Fact]
        public async Task Update_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var existingTarifa = CreateSampleTarifa();
            existingTarifa.Descripcion = "Descripción original";
            existingTarifa.PrecioPorNoche = 80.0m;

            var updateDto = new UpdateTarifaDto
            {
                IdTarifa = 1,
                IdCategoria = 0,
                Descripcion = null, 
                FechaInicio = DateTime.Now.AddDays(2),
                FechaFin = DateTime.Now.AddDays(12),
                PrecioPorNoche = 90.0m,
                Descuento = 10
            };

            Tarifas capturedTarifa = null;

            _mockTarifaRepository.Setup(repo => repo.GetEntityByIdAsync(updateDto.IdTarifa))
                .ReturnsAsync(existingTarifa);

            _mockTarifaRepository.Setup(repo => repo.UpdateEntityAsync(It.IsAny<Tarifas>()))
                .Callback<Tarifas>(t => capturedTarifa = t)
                .ReturnsAsync(new OperationResult { IsSuccess = true, Data = existingTarifa });

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(capturedTarifa);
            Assert.Equal("Descripción original", capturedTarifa.Descripcion);
            Assert.Equal(90.0m, capturedTarifa.PrecioPorNoche); 
            Assert.Equal(10, capturedTarifa.Descuento);
            Assert.Equal(1, capturedTarifa.IdCategoria); 
        }

        #endregion
    }
}