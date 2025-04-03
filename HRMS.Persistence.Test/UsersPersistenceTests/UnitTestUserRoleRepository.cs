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
    public class UnitTestUserRoleRepository
    {
        private readonly DbContextOptions<HRMSContext> _optionsDB;
        private readonly Mock<ILoggingServices> _mockLogger;
        private readonly Mock<IValidator<UserRole>> _mockValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;


        public UnitTestUserRoleRepository()
        {
            _optionsDB = new DbContextOptionsBuilder<HRMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILoggingServices>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockValidator = new Mock<IValidator<UserRole>>();

        }
        /*
        [Fact]
        public async Task GetAllAsync_ShouldReturnsActiveUserRoles_WhenRolesExists()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.UserRoles.AddRange(
                    new UserRole { IdRolUsuario = 1, Estado = true },
                    new UserRole { IdRolUsuario = 2, Estado = false }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetAllAsync(u => u.Estado == true);

                // assert
                Assert.Single(result.Data);
            }
        }

        [Fact]
        public async Task SaveEntityAsync_ShouldReturnsFailure_WhenValidationFails()
        {
            // arrange
            var userRole = new UserRole { RolNombre = "Administrador" };
            _mockValidator.Setup(v => v.Validate(userRole)).Returns(new OperationResult { IsSuccess = false, Message = "Error validation" });

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);
                // act
                var result = await repo.SaveEntityAsync(userRole);
                var expectedMessage = "Error validando los campos del rol para guardar";
                // assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task GetRoleByNameAsync_ShouldReturnsRole_WhenRoleExists()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.UserRoles.Add(new UserRole { RolNombre = "Administrador", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetRoleByNameAsync("Administrador");

                // assert
                Assert.NotNull(result);
                Assert.Equal("Administrador", result.RolNombre);
            }
        }

        [Fact]
        public async Task GetRoleByNameAsync_ShouldReturnsNull_WhenRoleDoesNotExist()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetRoleByNameAsync("prueba");

                // assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldReturnDailure_WhenFailsValidation()
        {
            // arrange
            var userRole = new UserRole { IdRolUsuario = 1, RolNombre = "pr" };
            _mockValidator.Setup(v => v.Validate(userRole)).Returns(new OperationResult { IsSuccess = false, Message = "Error validating role" });

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.UpdateEntityAsync(userRole);
                var expectedMessage = "Error validando los campos del rol para actualizar";
                // assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnRole_WhenRoleExists()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.UserRoles.Add(new UserRole { IdRolUsuario = 1, RolNombre = "Admin", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetEntityByIdAsync(1);

                // assert
                Assert.NotNull(result);
                Assert.Equal(1, result.IdRolUsuario);
            }
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnsNull_WhenRoleDoesNotExist()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetEntityByIdAsync(999);

                // assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetUsersByUserRoleIdAsync_ShouldReturnsUsers_WhenRoleIsBeingUsed()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.UserRoles.Add(new UserRole { IdRolUsuario = 1, Descripcion = "Admin", Estado = true });
                context.Users.Add(new User { IdUsuario = 1, IdRolUsuario = 1, NombreCompleto = "Susana", Correo = "suana@gmail.com" });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetUsersByUserRoleIdAsync(1);

                // assert
                Assert.True(result.IsSuccess);
                Assert.NotEmpty(result.Data);
            }
        }

        [Fact]
        public async Task GetUsersByUserRoleIdAsync_ShouldReturnsFailure_WhenUserRoleIsNotBeingUsed()
        {
            // arrange
            using (var context = new HRMSContext(_optionsDB))
            {
                context.UserRoles.Add(new UserRole { IdRolUsuario = 1, Descripcion = "Admin", Estado = true });
                await context.SaveChangesAsync();
            }

            using (var context = new HRMSContext(_optionsDB))
            {
                var repo = new UserRoleRepository(context, _mockLogger.Object, _mockConfiguration.Object, _mockValidator.Object);

                // act
                var result = await repo.GetUsersByUserRoleIdAsync(1);
                var expectedMessage = "No se encontraron usuarios con este rol";
                // assert
                Assert.False(result.IsSuccess);
                Assert.Equal(expectedMessage, result.Message);
            }
        }
        */
    }
}
