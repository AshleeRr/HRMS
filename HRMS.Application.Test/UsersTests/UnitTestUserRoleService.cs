using HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs;
using HRMS.Application.Services.UsersServices;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Moq;


namespace HRMS.Application.Test.UsersTests
{
    public class UnitTestUserRoleService
    {
        private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
        private readonly UserRoleService _userRoleService;
        private readonly Mock<IValidator<SaveUserRoleDTO>> _mockValidator;
        private readonly Mock<ILoggingServices> _mockLoggingServices;
        private readonly Mock<IUserRepository> _mockUserRepository;
        public UnitTestUserRoleService()
        {
            _mockUserRoleRepository = new Mock<IUserRoleRepository>();
            _mockValidator = new Mock<IValidator<SaveUserRoleDTO>>();
            _mockLoggingServices = new Mock<ILoggingServices>();
            _userRoleService = new UserRoleService(_mockUserRoleRepository.Object, _mockValidator.Object, _mockLoggingServices.Object, _mockUserRepository.Object);
        }/*
        [Fact]
        public async Task GetAll_ShoulReturnSuccess_WhenRolesExist() 
        {
            //arrange
            var userRoles = new List<UserRole> { new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" } };
            _mockUserRoleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(userRoles);
            //act
            var result = await _userRoleService.GetAll();
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<List<UserRole>>(result.Data);
        }
        [Fact]
        public async Task GetAll_ShoulReturnFailure_WhenRolesDoesntExist()
        {
            //arrange
            _mockUserRoleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserRole>());
            //act
            var result = await _userRoleService.GetAll();
            var expectedMessage = "No hay roles de usuario registrados";
            //assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task GetById_ShoulReturnFailure_WhenIdIsMinorThanOne()
        {
            //arrange
            var service = new UserRoleService(_mockUserRoleRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
            //act
            var result = await service.GetById(0);
            var messageExpected = "El id debe ser mayor que 0";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(messageExpected, result.Message);
        }
        [Fact]
        public async Task GetById_ShoulReturnSuccess_WhenRoleExists()
        {
            //arrange
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            _mockUserRoleRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(userRole);
            //act
            var result = await _userRoleService.GetById(1);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<UserRole>(result.Data);
        }
        [Fact]
        public async Task Save_ShouldReturnSuccess_WhenValidRoleIsSaved() 
        {
            //arrange
            var dto = new SaveUserRoleDTO { Descripcion = "prueba", RolNombre = "prueba" };
            var oP = new OperationResult { IsSuccess = true };
            _mockValidator.Setup(v => v.Validate(It.IsAny<SaveUserRoleDTO>())).Returns(oP);
            _mockUserRoleRepository.Setup(r => r.SaveEntityAsync(It.IsAny<UserRole>())).ReturnsAsync(oP);
            var service = new UserRoleService(_mockUserRoleRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);

            //act
            var result = await service.Save(dto);
            var expectedMessage = "Rol de usuario guardado correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenUserRoleIsInUse()
        {
            //arrange
            var service = new UserRoleService(_mockUserRoleRepository.Object,_mockValidator.Object, _mockLoggingServices.Object);
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            _mockUserRoleRepository.Setup(r => r.GetUsersByUserRoleIdAsync(1))
              .ReturnsAsync(new OperationResult
              {
                  IsSuccess = false,
                  Data = new List<User> { new User() } 
              });

            //act
            var result = await _userRoleService.Remove(new RemoveUserRoleDTO { IdUserRole = 1 });
            var expectedMessage = "Este rol está siendo utilizado por usuarios. No se puede eliminar";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Update_ShoulReturnFailure_WhenIdIsMinorThanOne()
        {
            //arrange
            var dto = new UpdateUserRoleDTO { IdUserRole = 0, Descripcion = "desc", Nombre = "nombre" };
            //act
            var result = await _userRoleService.Update(dto);
            var expectedMessage = "El id debe ser mayor que 0";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenRoleIsUpdated()
        {
            //arrange
            var dto = new UpdateUserRoleDTO { Descripcion = "", RolNombre= "", IdRolUsuario = 1, ChangeTime= DateTime.Now, UserID=1};
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            var service = new UserRoleService(_mockUserRoleRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
            var oP = new OperationResult { IsSuccess = true };

            _mockUserRoleRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(userRole);
            _mockUserRoleRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<UserRole>())).ReturnsAsync(oP);

            //act
            var result = await _userRoleService.Update(new UpdateUserRoleDTO { IdUserRole = 1 });
            var expectedMessage = "Rol de usuario actualizado correctamente";

            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateDescriptionAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newDescription = "prueba update";
            //act
            var result = await _userRoleService.UpdateDescriptionAsync(invalidId, newDescription);
            var expectedMessage = "El id debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateDescriptionAsync_ShouldReturnError_WhenDescriptionIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidDesc = null;
            //act
            var result = await _userRoleService.UpdateDescriptionAsync(id, invalidDesc);
            var expectedMessage = "El campo: nueva descripcion no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateDescriptionAsync_ShouldReturnSuccess_WhenDescriptionIsUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            string newDesc = "desc prueba";
            _mockUserRoleRepository.Setup(r => r.GetEntityByIdAsync(2)).ReturnsAsync(userRole);
            var oP = new OperationResult { IsSuccess = true };
            _mockUserRoleRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<UserRole>())).ReturnsAsync(oP);

            //act
            var result = await _userRoleService.UpdateDescriptionAsync(id, newDesc);
            var expectedMessage = "Se actualizó la descripción del rol de usuario";

            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateDescriptionAsync_ShouldReturnFailure_WhenDescriptionIsNotUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            string newDesc = "desc prueba";
            _mockUserRoleRepository.Setup(r => r.GetEntityByIdAsync(2)).ReturnsAsync(userRole);
            var oP = new OperationResult { IsSuccess = false };
            _mockUserRoleRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<UserRole>())).ReturnsAsync(oP);

            //act
            var result = await _userRoleService.UpdateDescriptionAsync(id, newDesc);
            var expectedMessage = "Error actualizando la descripcion del rol de usuario";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateNameAsync_ShouldReturnError_WhenIdIsInvalid()
        {

            //arrange
            int invalidId = 0;
            string newName = "prueba update";
            //act
            var result = await _userRoleService.UpdateNameAsync(invalidId, newName);
            var expectedMessage = "El id debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateNameAsync_ShouldReturnError_WhenNameIsNullOrEmpty()
        {
            //arrange
            int id = 2;
            string invalidName = null;
            //act
            var result = await _userRoleService.UpdateNameAsync(id, invalidName);
            var expectedMessage = "El campo: nuevo nombre no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateNameAsync_ShouldReturnSuccess_WhenNameIsUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            string newName = "nombre de prueba";
            _mockUserRoleRepository.Setup(r => r.GetEntityByIdAsync(2)).ReturnsAsync(userRole);
            var oP = new OperationResult { IsSuccess = true };
            _mockUserRoleRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<UserRole>())).ReturnsAsync(oP);

            //act
            var result = await _userRoleService.UpdateNameAsync(id, newName);
            var expectedMessage = "Se actualizó el nombre del rol de usuario";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateNameAsync_ShouldReturnFailure_WhenNameIsNotUpdatedSuccesfully()
        {
            //arrange
            int id = 2;
            var userRole = new UserRole { IdRolUsuario = 1, Descripcion = "prueba", Estado = true, FechaCreacion = DateTime.Now, RolNombre = "nombre prueba" };
            string newName = "nombre de prueba";
            _mockUserRoleRepository.Setup(r => r.GetEntityByIdAsync(id)).ReturnsAsync(userRole);
            var oP = new OperationResult { IsSuccess = false };
            _mockUserRoleRepository.Setup(r => r.UpdateEntityAsync(It.IsAny<UserRole>())).ReturnsAsync(oP);

            //act
            var result = await _userRoleService.UpdateNameAsync(id, newName);
            var expectedMessage = "Error actualizando el nombre del rol de usuario";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }*/

    }
}
