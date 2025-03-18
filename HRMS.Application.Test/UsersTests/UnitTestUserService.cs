using HRMS.Application.DTOs.UserDTOs;
using HRMS.Application.Services.UsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Moq;

namespace HRMS.Application.Test.UsersTests
{
    public class UnitTestUserService
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;
        private readonly Mock<IValidator<SaveUserClientDTO>> _mockValidator;
        private readonly Mock<ILoggingServices> _mockLoggingServices;
        public UnitTestUserService()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockValidator = new Mock<IValidator<SaveUserClientDTO>>();
            _mockLoggingServices = new Mock<ILoggingServices>();
            _userService = new UserService(_mockUserRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
        }
        [Fact]
        public async Task GetAll_ShoulReturnSuccess_WhensExist()
        {
            //arrange
            var users = new List<User> { new User { IdRolUsuario = 1, Estado = true, FechaCreacion = DateTime.Now, Clave = "123qweAAass#@", 
                Correo = "prueba@gmail.com", Documento = "11111111111", IdUsuario = 1, NombreCompleto = "nombre de prueba", TipoDocumento = "cedula" }};
            _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            //act
            var result = await _userService.GetAll();
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<List<User>>(result.Data);
        }
        [Fact]
        public async Task GetAll_ShoulReturnSuccess_WhensUsersDoesntExist()
        {
            //arrange
            _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            //act
            var result = await _userService.GetAll();
            var expectedMessage = "No hay usuarios registrados";
            //assert
            Assert.True(!result.IsSuccess);
            Assert.NotNull(result);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task GetById_ShoulReturnFailure_WhenIdIsMinorThanOne()
        {
            //arrange
            int invalidId = 0;
            var service = new UserService(_mockUserRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
            //act
            var result = await service.GetById(invalidId);
            var expectedMessage = "El id del usuario debe ser mayor que 0";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task GetById_ShoulReturnSuccess_WhenExists()
        {
            //arrange
            var user = new User { IdRolUsuario = 1, Estado = true, FechaCreacion = DateTime.Now, Clave = "123qweAAass#@",
                Correo = "prueba@gmail.com", Documento = "11111111111", IdUsuario = 1, NombreCompleto = "nombre de prueba", TipoDocumento = "cedula" };

            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
            //act
            var result = await _userService.GetById(1);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<User>(result.Data);
        }
        [Fact]
        public async Task GetById_ShoulReturnFailure_WhenUserDoesntExist()
        {
            //arrange
            int invalidId = 985;
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(invalidId)).ReturnsAsync((User)null);
            //act
            var result = await _userService.GetById(invalidId);
            var expectedMessage = "El usuario no existe";
            //assert
            Assert.True(!result.IsSuccess);
            Assert.Null(result);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Save_ShouldReturnSuccess_WhenValidIsSaved()
        {
            //arrange
            var dto = new SaveUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####", IdUserRole = 1, UserID = 1};
            var oP = new OperationResult { IsSuccess = true };
            _mockValidator.Setup(v => v.Validate(It.IsAny<SaveUserClientDTO>())).Returns(oP);
            _mockUserRepository.Setup(r => r.SaveEntityAsync(It.IsAny<User>())).ReturnsAsync(oP);
            var service = new UserService(_mockUserRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);

            //act
            var result = await service.Save(dto);
            var expectedMessage = "Usuario actualizado correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Save_ShouldReturnFailure_WhenDtoIsNotValid()
        {
            //arrange
            var dto = new SaveUserClientDTO { TipoDocumento = "", NombreCompleto = "", Documento = "", Correo = "", Clave = ""};
            var oP = new OperationResult { IsSuccess = false };
            _mockValidator.Setup(v => v.Validate(It.IsAny<SaveUserClientDTO>())).Returns(oP);

            //act
            var result = await _userService.Save(dto);
            var expectedMessage = "Error validando los campos para guardar";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Remove_ShouldReturnSuccess_WhenIsDeleted()
        {
            //arrange
            var user = new User
            {
                IdRolUsuario = 1,
                Estado = true,
                FechaCreacion = DateTime.Now,
                Clave = "123qweAAass#@",
                Correo = "prueba@gmail.com",
                Documento = "11111111111",
                IdUsuario = 1,
                NombreCompleto = "nombre de prueba",
                TipoDocumento = "cedula"
            };
            var service = new UserService(_mockUserRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
            var oP = new OperationResult { IsSuccess = true };

            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateEntityAsync(user)).ReturnsAsync(oP);

            //act
            var result = await _userService.Remove(new RemoveUserClientDTO { Id = 1 });
            var expectedMessage = "Usuario eliminado correctamente";

            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        
    }
}
