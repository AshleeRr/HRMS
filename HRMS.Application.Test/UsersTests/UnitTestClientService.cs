using HRMS.Application.DTOs.ClientDTOs;
using HRMS.Application.Services.UsersServices;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Moq;
using HRMS.Application.DTOs.UserDTOs;

namespace HRMS.Application.Test.UsersTests
{
    public class UnitTestClientService
    {
        private readonly Mock<IClientRepository> _mockClientRepository;
        private readonly ClientService _clientService;
        private readonly Mock<IValidator<SaveClientDTO>> _mockValidator;
        private readonly Mock<ILoggingServices> _mockLoggingServices;
        public UnitTestClientService()
        {
            _mockClientRepository = new Mock<IClientRepository>();
            _mockValidator = new Mock<IValidator<SaveClientDTO>>();
            _mockLoggingServices = new Mock<ILoggingServices>();
            _clientService = new ClientService(_mockClientRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
        }
        [Fact]
        public async Task GetAll_ShoulReturnSuccess_WhenClientsExist()
        {
            //arrange
            var clients = new List<Client> { new Client { IdCliente = 1, Estado = true, FechaCreacion = DateTime.Now, Clave = "123qweAAass#@",
                Correo = "pruebaemail@gmail.com", Documento = "11111111111", IdUsuario = 1, NombreCompleto = "nombre", TipoDocumento = "cedula" }};
            _mockClientRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(clients);
            //act
            var result = await _clientService.GetAll();
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<List<Client>>(result.Data);
        }
        [Fact]
        public async Task GetAll_ShoulReturnFailure_WhenClientsDoesNotExist()
        {
            //arrange
            _mockClientRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Client>());
            //act
            var result = await _clientService.GetAll();
            var expectedMessage = "No hay clientes registrados";
            //assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task GetById_ShoulReturnFailure_WhenIdIsMinorThanOne()
        {
            //arrange
            var service = new ClientService(_mockClientRepository.Object, _mockValidator.Object, _mockLoggingServices.Object);
            //act
            var result = await service.GetById(0);
            var message = "El id debe ser mayor que 0";
            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(message, result.Message);
        }
        [Fact]
        public async Task GetById_ShoulReturnSuccess_WhenClientExists()
        {
            //arrange
            var client = new Client
            {
                IdCliente = 1,
                Estado = true,
                FechaCreacion = DateTime.Now,
                Clave = "123qweAAass#@",
                Correo = "pruebacliente@gmail.com",
                Documento = "11111111111",
                IdUsuario = 1,
                NombreCompleto = "nombre",
                TipoDocumento = "cedula"
            };

            _mockClientRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(client);
            //act
            var result = await _clientService.GetById(1);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<Client>(result.Data);
        }
        [Fact]
        public async Task GetById_ShoulReturnFailure_WhenClientDoesNotExist()
        {
            //arrange
            int invalidId = 999;
            _mockClientRepository.Setup(r => r.GetEntityByIdAsync(invalidId)).ReturnsAsync((Client)null);
            //act
            var result = await _clientService.GetById(invalidId);
            //assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task Save_ShouldReturnSuccess_WhenClientIsSavedSuccesfully()
        {
            //arrange
            var dto = new SaveClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "cliente", Documento = "ABC123456", Correo = "pruebacliente1555@gmail.com", Clave = "123AAAAqqq####", IdUserRole = 1, IdUsuario = 1};
            var oP = new OperationResult { IsSuccess = true };
            _mockValidator.Setup(v => v.Validate(It.IsAny<SaveClientDTO>())).Returns(oP);
            _mockClientRepository.Setup(r => r.SaveEntityAsync(It.IsAny<Client>())).ReturnsAsync(oP);

            //act
            var result = await _clientService.Save(dto);
            var expectedMessage = "Cliente guardado correctamente";
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task Remove_ShouldReturnSuccess_WhenClientIsSuccesfullyDeleted()
        {
            //arrange
            var client = new Client
            {
                IdCliente = 1,
                Estado = true,
                FechaCreacion = DateTime.Now,
                Clave = "123qweAAass#@",
                Correo = "pruebacliente@gmail.com",
                Documento = "11111111111",
                IdUsuario = 1,
                NombreCompleto = "nombre",
                TipoDocumento = "cedula"
            };
            var oP = new OperationResult { IsSuccess = true };

            _mockClientRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(client);
            _mockClientRepository.Setup(r => r.UpdateEntityAsync(client)).ReturnsAsync(oP);

            //act
            var result = await _clientService.Remove(new RemoveUserClientDTO { Id = 1 });
            var expectedMessage = "Cliente eliminado correctamente";

            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newEmail = "pruebacliente@gmail.com";
            //act
            var result = await _clientService.UpdateCorreoAsync(invalidId, newEmail);
            var expectedMessage = "El id debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenNuevoCorreoIsNullOrEmpty()
        {
            //arrange
            int id = 9;
            string invalidEmail = "";
            //act
            var result = await _clientService.UpdateCorreoAsync(id, invalidEmail);
            var expectedMessage = "El campo: nuevo correo no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateCorreoAsync_ShouldReturnError_WhenNuevoCorreoIsAlreadyInUse()
        {
            //arrange
            _mockClientRepository.Setup(r => r.GetClientByEmailAsync("pruebacliente77@gmail.com")).ReturnsAsync(new Client { IdCliente = 1});
            //act
            var result = await _clientService.UpdateCorreoAsync(1, "pruebacliente77@gmail.com");
            //assert
            Assert.False(result.IsSuccess);

        }

        [Fact]
        public async Task UpdateNombreCompletoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newName = "nuevo nombre";
            //act
            var result = await _clientService.UpdateNombreCompletoAsync(invalidId, newName);
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
            var result = await _clientService.UpdateNombreCompletoAsync(id, invalidName);
            var expectedMessage = "El campo: nuevo nombre completo no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string typeDocument = "Cedula";
            string document = "11111111111";
            //act
            var result = await _clientService.UpdateTipoDocumentoAndDocumentoAsync(invalidId, typeDocument, document);
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
            var result = await _clientService.UpdateTipoDocumentoAndDocumentoAsync(id, typeDocument, invalidDocument);
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
            string document = "11111888811";
            //act
            var result = await _clientService.UpdateTipoDocumentoAndDocumentoAsync(id, invalidTypeDoc, document);
            var expectedMessage = "El campo: tipo documento no puede estar vacio.";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateTipoDocumentoAndDocumentoAsync_ShouldReturnError_WhenDocumentoIsAlreadyInUse()
        {
            //arrange
            var document = "ZAZ144444";
            _mockClientRepository.Setup(r => r.GetClientByDocumentAsync(document)).ReturnsAsync(new Client { IdUsuario = 3 });
            //act
            var result = await _clientService.UpdateTipoDocumentoAndDocumentoAsync(1, "pasaporte", "BLB148884");
            //assert
            Assert.False(result.IsSuccess);

        }
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            //arrange
            var dto = new UpdateUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####"};
            _mockClientRepository.Setup(r => r.GetClientByEmailAsync(dto.Correo)).ReturnsAsync(new Client());
            //act
            var result = await _clientService.Update(dto);
            //asserts
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenDocumentAlreadyExists()
        {
            //arrange
            var dto = new UpdateUserClientDTO { ChangeTime = DateTime.Now, TipoDocumento = "pasaporte", NombreCompleto = "preuba", Documento = "AAA123456", Correo = "prueba@gmail.com", Clave = "122AAAAqqq####" };
           
            _mockClientRepository.Setup(r => r.GetClientByDocumentAsync(dto.Documento)).ReturnsAsync(new Client());
            //act
            var result = await _clientService.Update(dto);
            //asserts
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenClientIsUpdatedSuccesfully()
        {
            _mockClientRepository.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(new Client { IdUsuario = 1 });
            //act
            var result = await _clientService.Update(new UpdateUserClientDTO { IdUsuario = 1 });
            var expectedMessage = "Cliente actualizado correctamente";
            //asserts
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdatePasswordAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            //arrange
            int invalidId = 0;
            string newPassword = "AAAAAAAAbbbaaa12312312#@#@";
            //act
            var result = await _clientService.UpdatePasswordAsync(invalidId, newPassword);
            var expectedMessage = "El id debe ser mayor que 0";

            //assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

    }
}
