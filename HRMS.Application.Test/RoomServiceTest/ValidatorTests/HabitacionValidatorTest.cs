using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.ValidatorTests
{
    public class HabitacionDtoValidatorTests
    {
        private readonly Mock<IValidator<CreateHabitacionDTo>> _mockValidator;
        
        public HabitacionDtoValidatorTests()
        {
            _mockValidator = new Mock<IValidator<CreateHabitacionDTo>>();
        }

        #region CreateHabitacionDTo Validation Tests
        
        [Fact]
        public void Validate_CreateWithValidDto_ReturnsSuccess()
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
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void Validate_CreateWithEmptyNumero_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "",
                Detalle = "Habitación simple",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El número de habitación es requerido"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El número de habitación es requerido", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithNullNumero_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = null,
                Detalle = "Habitación simple",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El número de habitación es requerido"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El número de habitación es requerido", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithTooLongNumero_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = new string('1', 51),
                Detalle = "Habitación simple",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El número de habitación no puede exceder los 50 caracteres"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El número de habitación no puede exceder los 50 caracteres", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithEmptyDetalle_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "101",
                Detalle = "",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El detalle de la habitación es requerido"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El detalle de la habitación es requerido", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithTooLongDetalle_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "101",
                Detalle = new string('a', 501), 
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El detalle de la habitación no puede exceder los 500 caracteres"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El detalle de la habitación no puede exceder los 500 caracteres", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithNegativePrecio_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "101",
                Detalle = "Habitación simple",
                Precio = -1,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El precio debe ser mayor o igual a cero"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio debe ser mayor o igual a cero", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithZeroPiso_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "101",
                Detalle = "Habitación simple",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 0,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID del piso debe ser mayor que cero"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID del piso debe ser mayor que cero", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithZeroCategoria_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "101",
                Detalle = "Habitación simple",
                Precio = 100,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 0
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la categoría debe ser mayor que cero"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la categoría debe ser mayor que cero", result.Message);
        }
        
        [Fact]
        public void Validate_CreateWithZeroEstadoHabitacion_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateHabitacionDTo
            {
                Numero = "101",
                Detalle = "Habitación simple",
                Precio = 100,
                IdEstadoHabitacion = 0,
                IdPiso = 1,
                IdCategoria = 1
            };
            
            _mockValidator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID del estado de habitación debe ser mayor que cero"));
                
            // Act
            var result = _mockValidator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID del estado de habitación debe ser mayor que cero", result.Message);
        }
        
        #endregion
        
        #region UpdateHabitacionDto Validation Tests
        
        [Fact]
        public void Validate_UpdateWithValidDto_ReturnsSuccess()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateHabitacionDto>>();
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
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void Validate_UpdateWithZeroId_ReturnsFailure()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateHabitacionDto>>();
            var dto = new UpdateHabitacionDto
            {
                IdHabitacion = 0,
                Numero = "101"
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la habitación debe ser mayor que cero"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la habitación debe ser mayor que cero", result.Message);
        }
        
        [Fact]
        public void Validate_UpdateWithInvalidNumero_ReturnsFailure()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateHabitacionDto>>();
            var dto = new UpdateHabitacionDto
            {
                IdHabitacion = 1,
                Numero = new string('1', 51) 
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El número de habitación no puede exceder los 50 caracteres"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El número de habitación no puede exceder los 50 caracteres", result.Message);
        }
        
        [Fact]
        public void Validate_UpdateWithNegativePrecio_ReturnsFailure()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateHabitacionDto>>();
            var dto = new UpdateHabitacionDto
            {
                IdHabitacion = 1,
                Precio = -1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El precio debe ser mayor o igual a cero"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio debe ser mayor o igual a cero", result.Message);
        }
        
        #endregion
        
        #region DeleteHabitacionDto Validation Tests
        
        [Fact]
        public void Validate_DeleteWithValidDto_ReturnsSuccess()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteHabitacionDto>>();
            var dto = new DeleteHabitacionDto
            {
                IdHabitacion = 1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void Validate_DeleteWithZeroId_ReturnsFailure()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteHabitacionDto>>();
            var dto = new DeleteHabitacionDto
            {
                IdHabitacion = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la habitación debe ser mayor que cero"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la habitación debe ser mayor que cero", result.Message);
        }
        
        [Fact]
        public void Validate_DeleteWithNegativeId_ReturnsFailure()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteHabitacionDto>>();
            var dto = new DeleteHabitacionDto
            {
                IdHabitacion = -1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la habitación debe ser mayor que cero"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la habitación debe ser mayor que cero", result.Message);
        }
        
        #endregion
    }
}