using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.UsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HRMS.Persistence.Test.UsersPersistenceTests
{
    public class UnitTestClientRepository
    {
        private readonly DbContextOptions<HRMSContext> _dbOptions;
        private readonly Mock<IValidator<Client>> _validatorMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILoggingServices> _loggerMock;

        public UnitTestClientRepository()
        {
            _dbOptions = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _validatorMock = new Mock<IValidator<Client>>();
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILoggingServices>();
        }
        /*
        [Fact]
        public async Task GetClientByEmailAsync_ShouldReturnClient_WhenEmailExists()
        {
            //arrange
            var email = "prueba@gmail.com";
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Clients.Add(new Client { Correo = email });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object );

                //act
                var result = await repo.GetClientByEmailAsync(email);

                //assert
                Assert.NotNull(result);
                Assert.Equal(email, result.Correo);
            }
        }

        [Fact]
        public async Task GetClientByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            //arrange
            var email = "prueba@gmail.com";
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.GetClientByEmailAsync(email);

                //assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetClientByDocumentAsync_ShouldReturnClient_WhenDocumentExists()
        {
            //arrange
            var document = "1234567890";
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Clients.Add(new Client { Documento = document });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.GetClientByDocumentAsync(document);

                //assert
                Assert.NotNull(result);
                Assert.Equal(document, result.Documento);
            }
        }

        [Fact]
        public async Task GetClientsByTypeDocumentAsync_ShouldReturnClients_WhenTypeDocumentExists()
        {
            //arrange
            var tipoDocumento = "cedula";
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Clients.AddRange(
                    new Client { TipoDocumento = tipoDocumento },
                    new Client { TipoDocumento = tipoDocumento });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.GetClientsByTypeDocumentAsync(tipoDocumento);

                //assert
                Assert.NotEmpty(result);
                Assert.All(result, c => Assert.Equal(tipoDocumento, c.TipoDocumento));
            }
        }

        [Fact]
        public async Task GetClientByUserIdAsync_ShouldReturnClient_WhenIdExists()
        {
            //arrange
            var userId = 1;
            using (var context = new HRMSContext(_dbOptions))
            {
                context.Clients.Add(new Client { IdUsuario = userId });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.GetClientByUserIdAsync(userId);

                //assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.IdUsuario);
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            //arrange
            var clientId = 999;
            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.GetEntityByIdAsync(clientId);

                //assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ValidClient_ShouldReturnSuccess()
        {
            //arrange
            var client = new Client { Correo = "prueba@gmail.com", Documento = "123456", NombreCompleto = "nmbrecompleto" };
            _validatorMock.Setup(v => v.Validate(client)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.SaveEntityAsync(client);
                var message = "Cliente guardado correctamente";
                //assert
                Assert.True(result.IsSuccess);
                Assert.Equal(message, result.Message);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_InvalidClient_ShouldReturnsFailure()
        {
            //arrange
            var client = new Client { Correo = "prueba@gmail.com", Documento = "123456", NombreCompleto = "pr" };
            _validatorMock.Setup(v => v.Validate(client)).Returns(new OperationResult { IsSuccess = false, Message = "Error de validación" });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.SaveEntityAsync(client);
                var expectedMessage = "Error de validación";
                //assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ValidClient_ShouldReturnsSuccess()
        {
            //arrange
            var client = new Client { IdUsuario = 1, Correo = "prueba1@gmail.com" };
            _validatorMock.Setup(v => v.Validate(client)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                context.Clients.Add(new Client { IdUsuario = 1, Correo = "prueba@gmail.com" });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);

                //act
                var result = await repo.UpdateEntityAsync(client);
                var expectedMessage = "Cliente actualizado correctamente";
                //assert
                Assert.True(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldReturnsFailure_WhenClientIsNotFound()
        {
            //arrange
            var client = new Client { IdUsuario = 999, Correo = "prueba@icloud.com" };
            _validatorMock.Setup(v => v.Validate(client)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_dbOptions))
            {
                var repo = new ClientRepository(context, _loggerMock.Object, _configMock.Object, _validatorMock.Object);
                //act
                var result = await repo.UpdateEntityAsync(client);
                var expectedMessage = "Este cliente no existe";
                //assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }*/
    }
}