using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.ValidatorTests
{
    public class EstadoHabitacionDtoTests
    {
        #region CreateEstadoHabitacionDto Tests
        
        [Fact]
        public void CreateEstadoHabitacionDto_ValidValues_IsValid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateEstadoHabitacionDto>>();
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = "Disponible"
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void CreateEstadoHabitacionDto_NullDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateEstadoHabitacionDto>>();
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = null
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }
        
        [Fact]
        public void CreateEstadoHabitacionDto_EmptyDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateEstadoHabitacionDto>>();
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = ""
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }
        
        [Fact]
        public void CreateEstadoHabitacionDto_WhitespaceDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateEstadoHabitacionDto>>();
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = "   "
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }
        
        [Fact]
        public void CreateEstadoHabitacionDto_TooLongDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateEstadoHabitacionDto>>();
            var dto = new CreateEstadoHabitacionDto
            {
                Descripcion = new string('A', 51) 
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción no puede exceder los 50 caracteres."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede exceder los 50 caracteres.", result.Message);
        }
        
        #endregion
        
        #region UpdateEstadoHabitacionDto Tests
        
        [Fact]
        public void UpdateEstadoHabitacionDto_ValidValues_IsValid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateEstadoHabitacionDto>>();
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = "Disponible Actualizado"
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void UpdateEstadoHabitacionDto_ZeroId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateEstadoHabitacionDto>>();
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 0,
                Descripcion = "Disponible"
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("Para actualizar el estado de habitación, el ID debe ser mayor que cero."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para actualizar el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }
        
        [Fact]
        public void UpdateEstadoHabitacionDto_NegativeId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateEstadoHabitacionDto>>();
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = -1,
                Descripcion = "Disponible"
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("Para actualizar el estado de habitación, el ID debe ser mayor que cero."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para actualizar el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }
        
        [Fact]
        public void UpdateEstadoHabitacionDto_NullDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateEstadoHabitacionDto>>();
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = null
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }
        
        [Fact]
        public void UpdateEstadoHabitacionDto_EmptyDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateEstadoHabitacionDto>>();
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = ""
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción del estado de habitación es requerida."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción del estado de habitación es requerida.", result.Message);
        }
        
        [Fact]
        public void UpdateEstadoHabitacionDto_TooLongDescripcion_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateEstadoHabitacionDto>>();
            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1,
                Descripcion = new string('A', 51)
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción no puede exceder los 50 caracteres."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede exceder los 50 caracteres.", result.Message);
        }
        
        #endregion
        
        #region DeleteEstadoHabitacionDto Tests
        
        [Fact]
        public void DeleteEstadoHabitacionDto_ValidId_IsValid()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteEstadoHabitacionDto>>();
            var dto = new DeleteEstadoHabitacionDto
            {
                IdEstadoHabitacion = 1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void DeleteEstadoHabitacionDto_ZeroId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteEstadoHabitacionDto>>();
            var dto = new DeleteEstadoHabitacionDto
            {
                IdEstadoHabitacion = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("Para eliminar el estado de habitación, el ID debe ser mayor que cero."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para eliminar el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }
        
        [Fact]
        public void DeleteEstadoHabitacionDto_NegativeId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteEstadoHabitacionDto>>();
            var dto = new DeleteEstadoHabitacionDto
            {
                IdEstadoHabitacion = -1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("Para eliminar el estado de habitación, el ID debe ser mayor que cero."));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Para eliminar el estado de habitación, el ID debe ser mayor que cero.", result.Message);
        }
        
        #endregion
        
        #region EstadoHabitacionDto Tests
        
        [Fact]
        public void EstadoHabitacionDto_Properties_WorkAsExpected()
        {
            // Arrange
            var descripcion = "Disponible";
            
            // Act
            var dto = new EstadoHabitacionDto
            {
                Descripcion = descripcion
            };
            
            // Assert
            Assert.Equal(descripcion, dto.Descripcion);
        }
        
        #endregion
    }
}