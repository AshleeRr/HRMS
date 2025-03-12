using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using Moq;

namespace HRMS.Application.Test.RoomServiceTest.ValidatorTests
{
    public class TarifaDtoValidatorTests
    {
        #region CreateTarifaDto Tests
        
        [Fact]
        public void CreateTarifaDto_ValidValues_IsValid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void CreateTarifaDto_StartDateBeforeToday_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(-1), 
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La fecha de inicio tiene que ser mayor a la fecha actual"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La fecha de inicio tiene que ser mayor a la fecha actual", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_EndDateBeforeStartDate_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(2),
                FechaFin = DateTime.Now.AddDays(1), 
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La fecha de inicio debe ser menor a la fecha de fin"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La fecha de inicio debe ser menor a la fecha de fin", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_NonPositivePrice_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 0, 
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El precio por noche debe ser mayor que 0"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio por noche debe ser mayor que 0", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_NegativePrice_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = -50,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El precio por noche debe ser mayor que 0"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El precio por noche debe ser mayor que 0", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_NonPositiveCategoryId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 0,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El id de categoría debe ser mayor que 0"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El id de categoría debe ser mayor que 0", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_NegativeDiscount_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "Tarifa de prueba",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = -5 
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El descuento no puede ser negativo"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El descuento no puede ser negativo", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_EmptyDescription_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción no puede estar vacía"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede estar vacía", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_NullDescription_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = null,
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción no puede estar vacía"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede estar vacía", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_TooShortDescription_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = "AB",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción debe tener al menos 3 caracteres"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción debe tener al menos 3 caracteres", result.Message);
        }
        
        [Fact]
        public void CreateTarifaDto_TooLongDescription_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateTarifaDto>>();
            var dto = new CreateTarifaDto
            {
                IdCategoria = 1,
                Descripcion = new string('A', 256), 
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 100.0m,
                Descuento = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("La descripción no puede tener más de 255 caracteres"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("La descripción no puede tener más de 255 caracteres", result.Message);
        }
        
        #endregion
        
        #region UpdateTarifaDto Tests
        
        [Fact]
        public void UpdateTarifaDto_ValidValues_IsValid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateTarifaDto>>();
            var dto = new UpdateTarifaDto
            {
                IdTarifa = 1,
                IdCategoria = 1,
                Descripcion = "Tarifa actualizada",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 120.0m,
                Descuento = 5
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void UpdateTarifaDto_InvalidId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<UpdateTarifaDto>>();
            var dto = new UpdateTarifaDto
            {
                IdTarifa = 0, 
                IdCategoria = 1,
                Descripcion = "Tarifa actualizada",
                FechaInicio = DateTime.Now.AddDays(1),
                FechaFin = DateTime.Now.AddDays(10),
                PrecioPorNoche = 120.0m,
                Descuento = 5
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la tarifa debe ser mayor que 0"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la tarifa debe ser mayor que 0", result.Message);
        }
        
        #endregion
        
        #region DeleteTarifaDto Tests
        
        [Fact]
        public void DeleteTarifaDto_ValidId_IsValid()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteTarifaDto>>();
            var dto = new DeleteTarifaDto
            {
                IdTarifa = 1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Success());
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void DeleteTarifaDto_ZeroId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteTarifaDto>>();
            var dto = new DeleteTarifaDto
            {
                IdTarifa = 0
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la tarifa debe ser mayor que 0"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la tarifa debe ser mayor que 0", result.Message);
        }
        
        [Fact]
        public void DeleteTarifaDto_NegativeId_IsInvalid()
        {
            // Arrange
            var validator = new Mock<IValidator<DeleteTarifaDto>>();
            var dto = new DeleteTarifaDto
            {
                IdTarifa = -1
            };
            
            validator.Setup(v => v.Validate(dto))
                .Returns(OperationResult.Failure("El ID de la tarifa debe ser mayor que 0"));
                
            // Act
            var result = validator.Object.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID de la tarifa debe ser mayor que 0", result.Message);
        }
        
        #endregion
    }
}