using System.Linq.Expressions;
using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Services.RoomServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Repository;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.Services
{
    public class HabitacionServicesTests
    {
        private readonly Mock<IHabitacionRepository> _mockRepository;
        private readonly Mock<ILogger<HabitacionServices>> _mockLogger;
        private readonly Mock<IValidator<CreateHabitacionDTo>> _mockValidator;
        private readonly HabitacionServices _service;
        private readonly Mock<IReservationRepository> _mockReservationRepository;

        public HabitacionServicesTests()
        {
            _mockRepository = new Mock<IHabitacionRepository>();
            _mockLogger = new Mock<ILogger<HabitacionServices>>();
            _mockValidator = new Mock<IValidator<CreateHabitacionDTo>>();
            _mockReservationRepository = new Mock<IReservationRepository>();
            _service = new HabitacionServices(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _mockReservationRepository.Object
            );
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_WithExistingHabitaciones_ReturnsSuccess()
        {
            // Arrange
            var habitaciones = new List<Habitacion>
            {
                new Habitacion { 
                    IdHabitacion = 1, 
                    Numero = "101", 
                    Detalle = "Habitación Simple",
                    Precio = 100.0m,
                    IdEstadoHabitacion = 1,
                    IdPiso = 1, 
                    IdCategoria = 1,
                    Estado = true 
                },
                new Habitacion { 
                    IdHabitacion = 2, 
                    Numero = "102", 
                    Detalle = "Habitación Doble",
                    Precio = 150.0m,
                    IdEstadoHabitacion = 1,
                    IdPiso = 1,
                    IdCategoria = 2,
                    Estado = true 
                }
            };
    
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(habitaciones);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitaciones obtenidas correctamente", result.Message);
    
            var resultDtos = Assert.IsType<List<HabitacionDto>>(result.Data);
            Assert.Equal(habitaciones.Count, resultDtos.Count);
    
            for (int i = 0; i < habitaciones.Count; i++)
            {
                Assert.Equal(habitaciones[i].Numero, resultDtos[i].Numero);
                Assert.Equal(habitaciones[i].Detalle, resultDtos[i].Detalle);
                Assert.Equal(habitaciones[i].Precio, resultDtos[i].Precio);
                Assert.Equal(habitaciones[i].IdEstadoHabitacion, resultDtos[i].IdEstadoHabitacion);
                Assert.Equal(habitaciones[i].IdPiso, resultDtos[i].IdPiso);
                Assert.Equal(habitaciones[i].IdCategoria, resultDtos[i].IdCategoria);
            }
        }

        [Fact]
        public async Task GetAll_WithNoHabitaciones_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Habitacion>());

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("No se encontraron habitaciones registradas", result.Message);
            Assert.IsType<List<HabitacionDto>>(result.Data);
            Assert.Empty((List<HabitacionDto>)result.Data);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener todas las habitaciones: Database error", result.Message);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsHabitacion()
        {
            // Arrange
            var habitacion = new Habitacion { 
                IdHabitacion = 1, 
                Numero = "101", 
                Detalle = "Habitación Simple",
                Precio = 100.0m,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                Estado = true 
            };
    
            _mockRepository.Setup(r => r.GetEntityByIdAsync(1))
                .ReturnsAsync(habitacion);

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitación obtenida correctamente", result.Message);
    
            // Verificar que el resultado es un HabitacionDto
            var habitacionDto = Assert.IsType<HabitacionDto>(result.Data);
    
            // Verificar que las propiedades del DTO coinciden con la entidad original
            Assert.Equal(habitacion.Numero, habitacionDto.Numero);
            Assert.Equal(habitacion.Detalle, habitacionDto.Detalle);
            Assert.Equal(habitacion.Precio, habitacionDto.Precio);
            Assert.Equal(habitacion.IdEstadoHabitacion, habitacionDto.IdEstadoHabitacion);
            Assert.Equal(habitacion.IdPiso, habitacionDto.IdPiso);
            Assert.Equal(habitacion.IdCategoria, habitacionDto.IdCategoria);
        }
        
        [Fact]
        public async Task GetById_WithInvalidId_ReturnsFailure()
        {
            // Act
            var result = await _service.GetById(0);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la habitación no es válido", result.Message);
        }

        [Fact]
        public async Task GetById_WithNonExistentHabitacion_ReturnsSuccess()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetEntityByIdAsync(99))
                .ReturnsAsync((Habitacion)null);

            // Act
            var result = await _service.GetById(99);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("No se encontró la habitación", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetById_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetEntityByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener la habitación con id 1: Database error", result.Message);
        }

        #endregion

        #region Save Tests

        [Fact]
        public async Task Save_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateHabitacionDTo 
            { 
                Numero = "101", 
                Detalle = "Habitación simple", 
                Precio = 100, 
                IdEstadoHabitacion = 1, 
                IdPiso = 1, 
                IdCategoria = 1 
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);
            
            _mockRepository.Setup(r => r.SaveEntityAsync(It.IsAny<Habitacion>()))
                .ReturnsAsync(OperationResult.Success(new Habitacion { IdHabitacion = 1, Numero = dto.Numero }));

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.SaveEntityAsync(It.Is<Habitacion>(h => 
                h.Numero == dto.Numero && 
                h.Detalle == dto.Detalle && 
                h.Precio == dto.Precio && 
                h.IdEstadoHabitacion == dto.IdEstadoHabitacion && 
                h.IdPiso == dto.IdPiso && 
                h.IdCategoria == dto.IdCategoria && 
                h.Estado == true)), Times.Once);
        }

        [Fact]
        public async Task Save_WithInvalidDto_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo();
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La validación del DTO ha fallado"));

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La validación del DTO ha fallado", result.Message);
            _mockRepository.Verify(r => r.SaveEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Save_WithDuplicateNumero_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo { Numero = "101" };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe una habitación con el número {dto.Numero}", result.Message);
            _mockRepository.Verify(r => r.SaveEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Save_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo { Numero = "101" };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);
            
            _mockRepository.Setup(r => r.SaveEntityAsync(It.IsAny<Habitacion>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar la habitación: Database error", result.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new UpdateHabitacionDto 
            { 
                IdHabitacion = 1,
                Numero = "101", 
                Detalle = "Habitación actualizada", 
                Precio = 150, 
                IdEstadoHabitacion = 2, 
                IdPiso = 2, 
                IdCategoria = 2 
            };
            
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "100", 
                Detalle = "Habitación original",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                Estado = true
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
            
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);
            
            _mockRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()))
                .ReturnsAsync(OperationResult.Success(existingHabitacion));

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.Is<Habitacion>(h => 
                h.IdHabitacion == dto.IdHabitacion)), Times.Once);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateHabitacionDto { IdHabitacion = 0 };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la habitación no es válido", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Update_WithInvalidDto_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateHabitacionDto { IdHabitacion = 1 };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La validación del DTO ha fallado"));

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La validación del DTO ha fallado", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Update_WithNonExistentHabitacion_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateHabitacionDto { IdHabitacion = 99 };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync((Habitacion)null);

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La habitación a actualizar no existe", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Update_WithDuplicateNumero_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateHabitacionDto 
            { 
                IdHabitacion = 1,
                Numero = "102"
            };
            
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "101"
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
            
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal($"Ya existe otra habitación con el número {dto.Numero}", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateHabitacionDto 
            { 
                IdHabitacion = 1,
                Numero = "101"
            };
            
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "101"
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
            
            _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Habitacion, bool>>>()))
                .ReturnsAsync(false);
            
            _mockRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"Error al actualizar la habitación con ID {dto.IdHabitacion}: Database error", result.Message);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 1 };
            
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "101",
                Estado = true
            };
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
            
            _mockRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()))
                .ReturnsAsync(OperationResult.Success(existingHabitacion));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitación eliminada correctamente", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.Is<Habitacion>(h => 
                h.IdHabitacion == dto.IdHabitacion && 
                h.Estado == false)), Times.Once);
        }

        [Fact]
        public async Task Remove_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 0 };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la habitación no es válido", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Remove_WithNonExistentHabitacion_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 99 };
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync((Habitacion)null);

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La habitación no existe", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }

        [Fact]
        public async Task Remove_WhenUpdateFails_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 1 };
            
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "101",
                Estado = true
            };
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
            
            _mockRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()))
                .ReturnsAsync(OperationResult.Failure("Error al actualizar"));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error al actualizar", result.Message);
        }

        [Fact]
        public async Task Remove_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 1 };
            
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"Error al eliminar la habitación con ID {dto.IdHabitacion}: Database error", result.Message);
        }

        [Fact]
        public async Task Remove_WhenHasResvervas_ReturnsFailure()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 1 };
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "101",
                Estado = true
            };
    
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
    
            _mockReservationRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La habitación tiene reservas activas", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()), Times.Never);
        }
        
        [Fact]
        public async Task Remove_WhenNoReservas_ReturnsSuccess()
        {
            // Arrange
            var dto = new DeleteHabitacionDto { IdHabitacion = 1 };
            var existingHabitacion = new Habitacion 
            { 
                IdHabitacion = 1, 
                Numero = "101",
                Estado = true
            };
    
            _mockRepository.Setup(r => r.GetEntityByIdAsync(dto.IdHabitacion))
                .ReturnsAsync(existingHabitacion);
    
            _mockReservationRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
                .ReturnsAsync(false);
    
            _mockRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<Habitacion>()))
                .ReturnsAsync(OperationResult.Success(existingHabitacion));

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitación eliminada correctamente", result.Message);
            _mockRepository.Verify(r => r.UpdateEntityAsync(It.Is<Habitacion>(h => 
                h.IdHabitacion == dto.IdHabitacion && 
                h.Estado == false)), Times.Once);
        }
        
        #endregion

        #region GetByPiso Tests

        [Fact]
        public async Task GetByPiso_WithValidId_ReturnsHabitaciones()
        {
            // Arrange
            var pisoId = 1;
            var habitaciones = new List<Habitacion>
            {
                new Habitacion { 
                    IdHabitacion = 1, 
                    Numero = "101", 
                    Detalle = "Habitación Simple",
                    Precio = 100.0m,
                    IdEstadoHabitacion = 1,
                    IdPiso = pisoId,
                    IdCategoria = 1,
                    Estado = true 
                },
                new Habitacion { 
                    IdHabitacion = 2, 
                    Numero = "102", 
                    Detalle = "Habitación Doble",
                    Precio = 150.0m,
                    IdEstadoHabitacion = 1,
                    IdPiso = pisoId,
                    IdCategoria = 2,
                    Estado = true 
                }
            };
    
            _mockRepository.Setup(r => r.GetByPisoAsync(pisoId))
                .ReturnsAsync(OperationResult.Success(habitaciones));

            // Act
            var result = await _service.GetByPiso(pisoId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitaciones obtenidas correctamente", result.Message);
    
            var resultDtos = Assert.IsType<List<HabitacionDto>>(result.Data);
            Assert.Equal(habitaciones.Count, resultDtos.Count);
    
            for (int i = 0; i < habitaciones.Count; i++)
            {
                Assert.Equal(habitaciones[i].Numero, resultDtos[i].Numero);
                Assert.Equal(habitaciones[i].Detalle, resultDtos[i].Detalle);
                Assert.Equal(habitaciones[i].Precio, resultDtos[i].Precio);
                Assert.Equal(habitaciones[i].IdEstadoHabitacion, resultDtos[i].IdEstadoHabitacion);
                Assert.Equal(habitaciones[i].IdPiso, resultDtos[i].IdPiso);
                Assert.Equal(habitaciones[i].IdCategoria, resultDtos[i].IdCategoria);
            }
        }

        [Fact]
        public async Task GetByPiso_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var pisoId = 0;

            // Act
            var result = await _service.GetByPiso(pisoId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID del piso debe ser mayor que 0", result.Message);
        }

        [Fact]
        public async Task GetByPiso_WithNoHabitacionesFound_ReturnsSuccess()
        {
            // Arrange
            var pisoId = 99;
            
            _mockRepository.Setup(r => r.GetByPisoAsync(pisoId))
                .ReturnsAsync(OperationResult.Success(new List<Habitacion>()));

            // Act
            var result = await _service.GetByPiso(pisoId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("No se encontraron habitaciones registradas", result.Message);
        }

        [Fact]
        public async Task GetByPiso_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var pisoId = 1;
            
            _mockRepository.Setup(r => r.GetByPisoAsync(pisoId))
                .ReturnsAsync(OperationResult.Failure("Error en el repositorio"));

            // Act
            var result = await _service.GetByPiso(pisoId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error en el repositorio", result.Message);
        }

        [Fact]
        public async Task GetByPiso_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var pisoId = 1;
            
            _mockRepository.Setup(r => r.GetByPisoAsync(pisoId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetByPiso(pisoId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"Error al obtener las habitaciones del piso {pisoId}: Database error", result.Message);
        }

        #endregion

        #region GetByNumero Tests

        [Fact]
        public async Task GetByNumero_WithValidNumero_ReturnsHabitacion()
        {
            // Arrange
            var numero = "101";
            var habitacion = new Habitacion { 
                IdHabitacion = 1, 
                Numero = numero, 
                Detalle = "Habitación Simple",
                Precio = 100.0m,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                Estado = true 
            };
    
            _mockRepository.Setup(r => r.GetByNumeroAsync(numero))
                .ReturnsAsync(OperationResult.Success(habitacion));

            // Act
            var result = await _service.GetByNumero(numero);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitación obtenida correctamente", result.Message);
    
            var habitacionDto = Assert.IsType<HabitacionDto>(result.Data);
    
            Assert.Equal(habitacion.Numero, habitacionDto.Numero);
            Assert.Equal(habitacion.Detalle, habitacionDto.Detalle);
            Assert.Equal(habitacion.Precio, habitacionDto.Precio);
            Assert.Equal(habitacion.IdEstadoHabitacion, habitacionDto.IdEstadoHabitacion);
            Assert.Equal(habitacion.IdPiso, habitacionDto.IdPiso);
            Assert.Equal(habitacion.IdCategoria, habitacionDto.IdCategoria);
        }

        [Fact]
        public async Task GetByNumero_WithEmptyNumero_ReturnsFailure()
        {
            // Arrange
            string numero = "";

            // Act
            var result = await _service.GetByNumero(numero);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El número de la habitación no puede estar vacío", result.Message);
        }

        [Fact]
        public async Task GetByNumero_WithNullNumero_ReturnsFailure()
        {
            // Arrange
            string numero = null;

            // Act
            var result = await _service.GetByNumero(numero);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El número de la habitación no puede estar vacío", result.Message);
        }

        [Fact]
        public async Task GetByNumero_WithNonExistentNumero_ReturnsSuccess()
        {
            // Arrange
            var numero = "999";
            
            _mockRepository.Setup(r => r.GetByNumeroAsync(numero))
                .ReturnsAsync(OperationResult.Success(null));

            // Act
            var result = await _service.GetByNumero(numero);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("No se encontró la habitación", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByNumero_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var numero = "101";
            
            _mockRepository.Setup(r => r.GetByNumeroAsync(numero))
                .ReturnsAsync(OperationResult.Failure("Error en el repositorio"));

            // Act
            var result = await _service.GetByNumero(numero);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error en el repositorio", result.Message);
        }

        [Fact]
        public async Task GetByNumero_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var numero = "101";
            
            _mockRepository.Setup(r => r.GetByNumeroAsync(numero))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetByNumero(numero);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"Error al obtener la habitación con número {numero}: Database error", result.Message);
        }

        #endregion

        #region GetByCategoria Tests

        [Fact]
        public async Task GetByCategoria_WithValidCategoria_ReturnsHabitaciones()
        {
            // Arrange
            string categoria = "Premium";
            var habitaciones = new List<Habitacion>
            {
                new Habitacion { 
                    IdHabitacion = 1, 
                    Numero = "101", 
                    Detalle = "Habitación Premium", 
                    Precio = 200.0m,
                    IdEstadoHabitacion = 1,
                    IdPiso = 1,
                    IdCategoria = 1,
                    Estado = true 
                },
                new Habitacion { 
                    IdHabitacion = 2, 
                    Numero = "102", 
                    Detalle = "Habitación Premium", 
                    Precio = 200.0m,
                    IdEstadoHabitacion = 1,
                    IdPiso = 1,
                    IdCategoria = 1,
                    Estado = true 
                }
            };
    
            var operationResult = OperationResult.Success(habitaciones, "Habitaciones obtenidas correctamente");
    
            _mockRepository.Setup(r => r.GetByCategoriaAsync(categoria))
                .ReturnsAsync(operationResult);

            // Act
            var result = await _service.GetByCategoria(categoria);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Habitaciones obtenidas correctamente", result.Message);
    
            var resultDtos = Assert.IsType<List<HabitacionDto>>(result.Data);
            Assert.Equal(habitaciones.Count, resultDtos.Count);
    
            for (int i = 0; i < habitaciones.Count; i++)
            {
                Assert.Equal(habitaciones[i].Numero, resultDtos[i].Numero);
                Assert.Equal(habitaciones[i].Detalle, resultDtos[i].Detalle);
                Assert.Equal(habitaciones[i].Precio, resultDtos[i].Precio);
                Assert.Equal(habitaciones[i].IdEstadoHabitacion, resultDtos[i].IdEstadoHabitacion);
                Assert.Equal(habitaciones[i].IdPiso, resultDtos[i].IdPiso);
                Assert.Equal(habitaciones[i].IdCategoria, resultDtos[i].IdCategoria);
            }
        }
        [Fact]
        public async Task GetByCategoria_WithEmptyCategoria_ReturnsFailure()
        {
            // Arrange
            string categoria = "";

            // Act
            var result = await _service.GetByCategoria(categoria);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La categoría de la habitación no puede estar vacía", result.Message);
        }

        [Fact]
        public async Task GetByCategoria_WithNullCategoria_ReturnsFailure()
        {
            // Arrange
            string categoria = null;

            // Act
            var result = await _service.GetByCategoria(categoria);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La categoría de la habitación no puede estar vacía", result.Message);
        }

        [Fact]
        public async Task GetByCategoria_WithNoHabitacionesFound_ReturnsSuccess()
        {
            // Arrange
            var categoria = "NoExistente";
            
            _mockRepository.Setup(r => r.GetByCategoriaAsync(categoria))
                .ReturnsAsync(OperationResult.Success(new List<Habitacion>()));

            // Act
            var result = await _service.GetByCategoria(categoria);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"No se encontraron habitaciones en la categoría '{categoria}'", result.Message);
            Assert.Empty((List<HabitacionDto>)result.Data);
        }

        [Fact]
        public async Task GetByCategoria_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            var categoria = "Simple";
            
            _mockRepository.Setup(r => r.GetByCategoriaAsync(categoria))
                .ReturnsAsync(OperationResult.Failure("Error en el repositorio"));

            // Act
            var result = await _service.GetByCategoria(categoria);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error en el repositorio", result.Message);
        }

        [Fact]
        public async Task GetByCategoria_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var categoria = "Simple";
            
            _mockRepository.Setup(r => r.GetByCategoriaAsync(categoria))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetByCategoria(categoria);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"Error al obtener las habitaciones de la categoría {categoria}: Database error", result.Message);
        }

        #endregion

        #region GetInfoHabitacionesAsync Tests

        [Fact]
        public async Task GetInfoHabitacionesAsync_WithValidResult_ReturnsSuccess()
        {
            // Arrange
            var infoHabitaciones = new List<object> 
            {
                new 
                { 
                    IdHabitacion = 1, 
                    Numero = "101",
                    PrecioPorNoche = 100.0M,
                    DescripcionCategoria = "Simple"
                }
            };
            
            _mockRepository.Setup(r => r.GetInfoHabitacionesAsync())
                .ReturnsAsync(OperationResult.Success(infoHabitaciones));

            // Act
            var result = await _service.GetInfoHabitacionesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Información de las habitaciones obtenida correctamente", result.Message);
            Assert.Same(infoHabitaciones, result.Data);
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_WithNoInfoFound_ReturnsSuccess()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetInfoHabitacionesAsync())
                .ReturnsAsync(OperationResult.Success(null));

            // Act
            var result = await _service.GetInfoHabitacionesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("No se encontró la información de las habitaciones", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_WhenRepositoryReturnsFailure_ReturnsFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetInfoHabitacionesAsync())
                .ReturnsAsync(OperationResult.Failure("Error en el repositorio"));

            // Act
            var result = await _service.GetInfoHabitacionesAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Error en el repositorio", result.Message);
        }

        [Fact]
        public async Task GetInfoHabitacionesAsync_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetInfoHabitacionesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetInfoHabitacionesAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener la información de las habitaciones: Database error", result.Message);
        }

        #endregion
    }
}