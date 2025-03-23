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
        public async Task GetAll_ShoulReturnSuccess_WhenExist()
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
        public async Task GetAll_ShoulReturnSuccess_WhensUsersDoesNotExist()
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
            
            //act
            var result = await _userService.GetById(invalidId);
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
        public async Task GetById_ShoulReturnFailure_WhenUserDoesNotExist()
        {
            //arrange
            int invalidId = 985;
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(invalidId)).ReturnsAsync((User)null);
            //act
            var result = await _userService.GetById(invalidId);
            //assert
            Assert.False(result.IsSuccess);
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
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newEmail = "prueba1555@gmail.com";
            //act
            var result = await _userService.UpdateCorreoAsync(invalidId, newEmail);
            var messgaeExpected = "El id del usuario debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(messgaeExpected, result.Message);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenNuevoCorreoIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidEmail = null;
            //act
            var result = await _userService.UpdateCorreoAsync(id, invalidEmail);

            //assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenNuevoCorreoIsAlreadyInUse()
        {
            //arrange
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync("prueba77@gmail.com")).ReturnsAsync(new User { IdUsuario = 1 });
            //act
            var result = await _userService.UpdateCorreoAsync(1, "pruebacliente77@gmail.com");
            //assert
            Assert.False(result.IsSuccess);

        }
        [Fact]
        public async Task UpdateNombreCompletoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newName = "pr";
            //act
            var result = await _userService.UpdateNombreCompletoAsync(invalidId, newName);
            var expectedMessage = "El id del usuario debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateNombreCompletoAsync_ShouldReturnError_WhenNuevoNombreIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidName = null;
            //act
            var result = await _userService.UpdateCorreoAsync(id, invalidName);

            //assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string typeDocument = "Passporte";
            string document = "ABB123456";
            //act
            var result = await _userService.UpdateTipoDocumentoAndDocumentoAsync(invalidId, typeDocument, document);
            var expectedMessage = "El id del usuario debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenDocumentoIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidDocument = null;
            string typeDocument = "cedula";
            //act
            var result = await _userService.UpdateTipoDocumentoAndDocumentoAsync(id, typeDocument, invalidDocument);

            //assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenTipoDocumentoIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidTypeDoc = null;
            string document = "11111111111";
            //act
            var result = await _userService.UpdateTipoDocumentoAndDocumentoAsync(id, invalidTypeDoc, document);

            //assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenDocumentoIsAlreadyInUse()
        {
            //arrange
            var document = "ZAZ144444";
            _mockUserRepository.Setup(r => r.GetUserByDocumentAsync(document)).ReturnsAsync(new User { IdUsuario = 3 });
            //act
            var result = await _userService.UpdateTipoDocumentoAndDocumentoAsync(1, "pasaporte", "BLB148884");
            //assert
            Assert.False(result.IsSuccess);

        }
        [Fact]
        public async Task UpdateUserRoleToUserAsync_ShouldReturnFailure_WhenIdUserIsNotValid()
        {
            //arrange
            int idUser = 0;
            int idUserRole = 4;
            //act
            var result = await _userService.UpdateUserRoleToUserAsync(idUser, idUserRole);
            var expectedMessage = "El id del usuario debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateUserRoleToUserAsync_ShouldReturnFailure_WhenIdUserRoleIsNotValid()
        {
            //arrange
            int idUser = 8;
            int idUserRole = -1;
            //act
            var result = await _userService.UpdateUserRoleToUserAsync(idUser, idUserRole);
            var expectedMessage = "El id del usuario debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateUserRoleToUserAsync_ShouldReturnFailure_WhenUserIsNotFound()
        {
            //assert
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);
            //act
            var result = await _userService.UpdateUserRoleToUserAsync(1, 2);
            //assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdateUserRoleToUserAsync_ShouldReturnSuccess_WhenRoleIsUpdatedSuccesfullyToAnUser()
        {
            //assert
            int idUser = 1;
            int idUserRole = 3;
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(idUser)).ReturnsAsync(new User { IdUsuario = 1, IdRolUsuario = 2 });
            //act
            var result = await _userService.UpdateUserRoleToUserAsync(idUser, idUserRole);
            var expectedMessage = "Rol de usuario actualizado al usuario correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.NotNull(result);
        }
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            //arrange
            var dto = new UpdateUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####" };
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(dto.Correo)).ReturnsAsync(new User());
            //act
            var result = await _userService.Update(dto);
            //asserts
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenDocumentAlreadyExists()
        {
            //arrange
            var dto = new UpdateUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####" };
            _mockUserRepository.Setup(r => r.GetUserByDocumentAsync(dto.Correo)).ReturnsAsync(new User());
            //act
            var result = await _userService.Update(dto);
            //asserts
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task UpdatePasswordAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newPassword = "AAAAAAAAAAAaaa12312312#@#@";
            //act
            var result = await _userService.UpdatePasswordAsync(invalidId, newPassword);
            var expectedMessage = "El id del usuario debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdatePasswordAsync_ShouldReturnSuccess_WhenPasswordIsUpdatedSuccesfully()
        {
            ///arrange
            string validPassword = "12323qweqASDAS#@";
            _mockUserRepository.Setup(mr => mr.GetEntityByIdAsync(1)).ReturnsAsync(new User { IdUsuario = 1 });

            //act
            var expectedMessage = "Clave actualizada correctamente";
            var result = await _userService.UpdatePasswordAsync(1, validPassword);
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

    }
}
