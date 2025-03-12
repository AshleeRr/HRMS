using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.DTOs.RoomManagementDto.Validations;

namespace HRMS.Application.Test.RoomServiceTest.ValidatorTests
{
    public class CategoryServiceValidatorTests
    {
        private readonly CategoryServiceValidator _validator;

        public CategoryServiceValidatorTests()
        {
            _validator = new CategoryServiceValidator();
        }

        [Fact]
        public void Validate_ValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Categoría válida",
                IdServicio = 1,
                Capacidad = 2
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(string.IsNullOrEmpty(result.Message));
        }

        [Fact]
        public void Validate_DescriptionTooLong_ShouldReturnFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = new string('A', 51),
                IdServicio = 1,
                Capacidad = 2
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La descripcion categoria no puede superar los 50 caracteres", result.Message);
        }

        [Fact]
        public void Validate_DescriptionTooShort_ShouldReturnFailure()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "AB",
                IdServicio = 1,
                Capacidad = 2
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La descripcion categoria debe tener al menos 3 caracteres", result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_EmptyDescription_ShouldReturnFailure(string descripcion)
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = descripcion,
                IdServicio = 1,
                Capacidad = 2
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La descripcion no puede estar vacia", result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidServicioId_ShouldReturnFailure(short idServicio)
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Categoría válida",
                IdServicio = idServicio,
                Capacidad = 2
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El id del servicio debe ser mayor a 0", result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidCapacidad_ShouldReturnFailure(int capacidad)
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "Categoría válida",
                IdServicio = 1,
                Capacidad = capacidad
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("la capacidad debe ser mayor a 0", result.Message);
        }

        [Fact]
        public void Validate_MultipleErrors_ShouldReturnAllErrorMessages()
        {
            // Arrange
            var dto = new CreateCategoriaDto
            {
                Descripcion = "AB",
                IdServicio = 0,
                Capacidad = 0
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La descripcion categoria debe tener al menos 3 caracteres", result.Message);
            Assert.Contains("El id del servicio debe ser mayor a 0", result.Message);
            Assert.Contains("la capacidad debe ser mayor a 0", result.Message);
        }
    }
}