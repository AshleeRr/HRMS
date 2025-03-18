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
            var expectedMessage = "El usuario no existe";
            //assert
            Assert.True(!result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Save_ShouldReturnSuccess_WhenValidIsSaved()
        {
            //arrange
            var dto = new SaveUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####", IdUserRole = 1, UserID = 1 };
            var oP = new OperationResult { IsSuccess = true };
            _mockValidator.Setup(v => v.Validate(It.IsAny<SaveUserClientDTO>())).Returns(oP);
            _mockUserRepository.Setup(r => r.SaveEntityAsync(It.IsAny<User>())).ReturnsAsync(oP);
            
            //act
            var result = await _userService.Save(dto);
            var expectedMessage = "Usuario guardado correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Save_ShouldReturnFailure_WhenDtoIsNotValid()
        {
            //arrange
            var dto = new SaveUserClientDTO { TipoDocumento = "", NombreCompleto = "", Documento = "", Correo = "", Clave = "" };
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
        public async Task Save_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            //arrange
            var dto = new SaveUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####", IdUserRole = 1, UserID = 1 };
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(dto.Correo)).ReturnsAsync(new User());
            //act
            var result = await _userService.Save(dto);
            var expectedMessage = "Este correo ya esta registrado";
            //asserts
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Save_ShouldReturnFailure_WhenDocumentAlreadyExists()
        {
            //arrange
            var dto = new SaveUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####", IdUserRole = 1, UserID = 1 };
            _mockUserRepository.Setup(r => r.GetUserByDocumentAsync(dto.Documento)).ReturnsAsync(new User());
            //act
            var result = await _userService.Save(dto);
            var expectedMessage = "Este documento ya esta registrado";
            //asserts
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
            var expectedMessage = "El id debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenNuevoCorreoIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidEmail = null;
            //act
            var result = await _userService.UpdateCorreoAsync(id, invalidEmail);
            var expectedMessage = "El campo: nuevo correo no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenNuevoCorreoIsAlreadyInUse()
        {
            //arrange
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync("prueba@icloud.com")).ReturnsAsync(new User { IdUsuario = 2 });
            //act
            var result = await _userService.UpdateCorreoAsync(1, "prueba@icloud.com");
            var expectedMessage = "Este correo ya esta registrado";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);

        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnSuccess_WhenCorreoIsUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
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


            string newCorreo = "correo@gmail.com";
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
            var oP = new OperationResult { IsSuccess = true };
            _mockUserRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<User>())).ReturnsAsync(oP);

            //act
            var result = await _userService.UpdateCorreoAsync(id, newCorreo);
            var expectedMessage = "Correo actualizado correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task UpdateNombreCompletoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newName = "pr";
            //act
            var result = await _userService.UpdateNombreCompletoAsync(invalidId, newName);
            var expectedMessage = "El id debe ser mayor que 0";

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
            var expectedMessage = "El campo: nuevo nombre no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateNombreCompletoAsync_ShouldReturnSuccess_WhenNombreIsUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
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


            string newCorreo = "nombre actualizado";
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
            var oP = new OperationResult { IsSuccess = true };
            _mockUserRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<User>())).ReturnsAsync(oP);

            //act
            var result = await _userService.UpdateCorreoAsync(id, newCorreo);
            var expectedMessage = "Nombre actualizado correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
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
            var expectedMessage = "El id debe ser mayor que 0";

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
            var expectedMessage = "El campo: documento no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
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
            var expectedMessage = "El campo: tipo de documento no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenDocumentoIsAlreadyInUse()
        {
            //arrange
            var document = "BBB144444";
            _mockUserRepository.Setup(r => r.GetUserByDocumentAsync(document)).ReturnsAsync(new User { IdUsuario = 3 });
            //act
            var result = await _userService.UpdateTipoDocumentoAndDocumentoAsync(1, "pasaporte", "BBB144444");
            var expectedMessage = "Este documento ya esta registrado";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);

        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnSuccess_WhenTipoDocumentoAndDocumentoAreUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
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
            string newDocument = "123456789";
            string newTypeDoc = "cedula";
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
            var oP = new OperationResult { IsSuccess = true };
            _mockUserRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<User>())).ReturnsAsync(oP);

            //act
            var result = await _userService.UpdateTipoDocumentoAndDocumentoAsync(id, newDocument, newTypeDoc);
            var expectedMessage = "Datos actualizados correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateUserRoleToUserAsync_ShouldReturnFailure_WhenIdUserIsNotValid()
        {
            //arrange
            int idUser = 0;
            int idUserRole = 4;
            //act
            var result = await _userService.UpdateUserRoleToUserAsync(idUser, idUserRole);
            var expectedMessage = "El id debe ser mayor que 0";

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
            var expectedMessage = "El id debe ser mayor que 0";

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
            var expectedMessage = "El usuario no existe";
            var result = await _userService.UpdateUserRoleToUserAsync(1, 2);
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
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
        public async Task Update_ShouldReturnError_WhenUserIsNotFound() {
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);
            //act
            var result = await _userService.Update(new UpdateUserClientDTO { IdUsuario = 1});
            var expectedMessage = "El usuario no existe";
            //asserts
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            //arrange
            var dto = new UpdateUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####", IdUserRole = 1, UserID = 1 };
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(dto.Correo)).ReturnsAsync(new User());
            //act
            var result = await _userService.Update(dto);
            var expectedMessage = "Este correo ya esta registrado";
            //asserts
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenDocumentAlreadyExists()
        {
            //arrange
            var dto = new UpdateUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####", IdUserRole = 1, UserID = 1 };
            _mockUserRepository.Setup(r => r.GetUserByDocumentAsync(dto.Documento)).ReturnsAsync(new User());
            //act
            var result = await _userService.Update(dto);
            var expectedMessage = "Este documento ya esta registrado";
            //asserts
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenUserIsUpdatedSuccesfully() {
            _mockUserRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(new User { IdUsuario = 1});
            //act
            var result = await _userService.Update(new UpdateUserClientDTO { IdUsuario = 1 });
            var expectedMessage = "Usuario actualizado correctamente";
            //asserts
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdatePasswordAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newPassword = "AAAAAAAAAAAaaa12312312#@#@";
            //act
            var result = await _userService.UpdatePasswordAsync(invalidId, newPassword);
            var expectedMessage = "El id debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        public static IEnumerable<object[]> UserInvalidPasswords => new List<object[]>
        {
            new object[]{ null},
            new object[] { new string('B', 51) },
            new object[] { new string('B', 11) },
            new object[] { "bbbbbbbbbbbbb" },
            new object[] { "BBBBBBBBBBBBB" },
            new object[] { "BBbbbbbbbbbbb" },
            new object[] { "BBbbbbbb1234" },
            new object[] { "BBbbbbb##b1234" },
            new object[]{ "BBbbb  ##1234"}
        };

        [Theory]
        [MemberData(nameof(UserInvalidPasswords))]
        public async Task UpdatePasswordAsync_ShouldReturnError_WhenPasswordIsNotValid(string invalidPassword)
        {
            ///arrange
            _mockUserRepository.Setup(mr => mr.GetEntityByIdAsync(1)).ReturnsAsync(new User { IdUsuario = 1 });
            
            //act
            var expectedMessage = "La clave no debe contener espacios. Debe tener al menos 8 caracteres, un número, una letra mayúscula, un caracter especial y una letra minúscula para ser segura";
            var result = await _userService.UpdatePasswordAsync(1, invalidPassword);
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
