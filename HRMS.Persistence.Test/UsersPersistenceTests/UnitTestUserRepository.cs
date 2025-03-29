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
    public class UnitTestUserRepository
    {
        private readonly DbContextOptions<HRMSContext> _optionsDB;
        private readonly Mock<ILoggingServices> _mockLogger;
        private readonly Mock<IValidator<User>> _mockValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;


        public UnitTestUserRepository()
        {
            _optionsDB = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILoggingServices>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockValidator = new Mock<IValidator<User>>();

        }

        [Fact]

        public async Task GetUsersByNameAsync_ShouldThrowArgumentException_WhenNombreCompletoIsNullOrEmpty()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.Add(new User { NombreCompleto = "prueba", Clave = "prUEba123#@aaa", Correo = "prueba@gmail.com", Documento = "40219814962", Estado = true, FechaCreacion = DateTime.Now, IdRolUsuario = 1, IdUsuario = 1, TipoDocumento = "pruebaDoc" });
                context.Users.Add(new User { NombreCompleto = "prueba2", Clave = "prUEba123#@aaa", Correo = "prueba@gmail.com", Documento = "40219814962", Estado = true, FechaCreacion = DateTime.Now, IdRolUsuario = 1, IdUsuario = 1, TipoDocumento = "pruebaDoc" });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act 
                var result = await repo.GetUsersByNameAsync(" ");
                // assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetUsersByNameAsync_ShouldReturnEmptyList_WhenUsersAreNotFound()
        {
            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act 
                var result = await repo.GetUsersByNameAsync("prueba");
                // assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetUsersByNameAsync_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.Add(new User { NombreCompleto = "prueba", Clave = "prUEba123#@aaa", Correo = "prueba@gmail.com", Documento = "40219814962", Estado = true, FechaCreacion = DateTime.Now, IdRolUsuario = 1, IdUsuario = 1, TipoDocumento = "pruebaDoc" });
                context.Users.Add(new User { NombreCompleto = "prueba2", Clave = "prUEba123#@aaa", Correo = "prueba@gmail.com", Documento = "40219814962", Estado = true, FechaCreacion = DateTime.Now, IdRolUsuario = 1, IdUsuario = 1, TipoDocumento = "pruebaDoc" });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act 
                var result = await repo.GetUsersByNameAsync("prueba");
                // assert
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // arrange;

            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.Add(new User { IdUsuario = 1, Correo = "prueba@gmail.com", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object); ;

                // act
                var result = await repo.GetUserByEmailAsync("prueba@gmail.com");

                // assert
                Assert.NotNull(result);
                Assert.Equal("prueba@gmail.com", result.Correo);
            }
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetUserByEmailAsync("juana@example.com");

                // assert
                Assert.Null(result);
            }
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.AddRange(
                    new User { IdUsuario = 1, Estado = true },
                    new User { IdUsuario = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetAllAsync(u => u.Estado == true);

                // assert
                Assert.Single(result.Data);
            }
        }
        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.Add(new User { IdUsuario = 1, NombreCompleto = "nombre" });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetEntityByIdAsync(1);

                // assert
                Assert.NotNull(result);
                Assert.Equal(1, result.IdUsuario);
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetEntityByIdAsync(9999);

                // assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ShouldReturnSuccess_WhenUserIsValid()
        {
            // arrange
            var user = new User { IdUsuario = 1, NombreCompleto = "prueba", Correo = "prueba@gmail.com", Estado = true };

            _mockValidator.Setup(v => v.Validate(user)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.SaveEntityAsync(user);
                var expectedMessage = "Usuario guardado correctamente";
                // assert
                Assert.True(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ShouldReturnFailure_WhenUserIsNotValid()
        {
            // arrange
            var user = new User { IdUsuario = 1, NombreCompleto = "pr" };

            _mockValidator.Setup(v => v.Validate(user)).Returns(new OperationResult { IsSuccess = false, Message = "Validation error" });

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.SaveEntityAsync(user);
                var expectedMessage = "Validation error";
                // assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }



        [Fact]
        public async Task UpdateEntityAsync_ShouldReturnSuccess_WhenUserExists()
        {
            // arrange
            var existingUser = new User { IdUsuario = 1, NombreCompleto = "nombre1", Correo = "prueba1@gmail.com" };

            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.Add(existingUser);
                await context.SaveChangesAsync();
            }

            var updatedUser = new User { IdUsuario = 1, NombreCompleto = "Nombre2", Correo = "prueba2@gmail.com" };

            _mockValidator.Setup(v => v.Validate(updatedUser)).Returns(new OperationResult { IsSuccess = true });

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.UpdateEntityAsync(updatedUser);
                var expectedMessage = "Usuario actualizado correctamente";
                // assert
                Assert.True(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // arrange
            var updatedUser = new User { IdUsuario = 99, NombreCompleto = "pruebaUser", Correo = "juan@icloud.com" };

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.UpdateEntityAsync(updatedUser);
                var expectedMessage = "Este usuario no existe";
                // assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task GetUserByDocumentAsync_ShouldReturnUser_WhenDocumentExists()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.Add(new User { IdUsuario = 1, Documento = "123456789987", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetUserByDocumentAsync("123456789987");
                // assert
                Assert.NotNull(result);
                Assert.Equal("123456789987", result.Documento);
            }
        }


        [Fact]
        public async Task GetUsersByTypeDocumentAsync_ShouldReturnUsers_WhenTypeDocumentExists()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.Users.AddRange(
                    new User { IdUsuario = 1, TipoDocumento = "DNI", Estado = true },
                    new User { IdUsuario = 2, TipoDocumento = "DNI", Estado = true }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetUsersByTypeDocumentAsync("DNI");

                // assert
                Assert.Equal(2, result.Count);
                Assert.All(result, u => Assert.Equal("DNI", u.TipoDocumento));
            }
        }


    }
}
