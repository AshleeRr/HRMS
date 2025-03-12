using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;

namespace HRMS.Application.Test.RoomServiceTest.ValidatorTests
{
    public class PisoDtoValidatorTests
    {
        private readonly PisoServiceValidator _validator;
        
        public PisoDtoValidatorTests()
        {
            _validator = new PisoServiceValidator();
        }

        #region CreatePisoDto Tests
        
        [Fact]
        public void CreatePisoDto_ValidValues_IsValid()
        {
            // Arrange
            var dto = new CreatePisoDto
            {
                Descripcion = "Primer Piso"
            };
                
            // Act
            var result = _validator.Validate(dto);
            
            // Assert
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public void CreatePisoDto_NullDescripcion_IsInvalid()
        {
            // Arrange
            var dto = new CreatePisoDto
            {
                Descripcion = null
            };
                
            // Act
            var result = _validator.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("no puede estar vacía", result.Message);
        }
        
        [Fact]
        public void CreatePisoDto_EmptyDescripcion_IsInvalid()
        {
            // Arrange
            var dto = new CreatePisoDto
            {
                Descripcion = ""
            };
                
            // Act
            var result = _validator.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("es requerida", result.Message);
        }
        
        [Fact]
        public void CreatePisoDto_TooLongDescripcion_IsInvalid()
        {
            // Arrange
            var dto = new CreatePisoDto
            {
                Descripcion = new string('A', 51) 
            };
                
            // Act
            var result = _validator.Validate(dto);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("no puede tener más de 50 caracteres", result.Message);
        }
        
        #endregion
    }
}