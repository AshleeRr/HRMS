using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.ReservationValidator;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;
using HRMS.Models.Models.ReservationModels;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.Reserv;
using HRMS.Persistence.Test.TestContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRMS.Application.Test.ReservationTests
{
    public class ReservationRepositoryTest
    {
        private readonly IReservationRepository reservationRepository;
        private Reservation reservToUpdate;
        private int categoryId;
        private Reservation _reservCanceled;
        private int reservTestId;
        public ReservationRepositoryTest()
        {
            var _dbContextOptions = new DbContextOptionsBuilder<HRMSContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Nombre de la BD en memoria
            .Options;

            var _context = new HRMSContext(_dbContextOptions);
            var CategoryPlaceholder = new Categoria { IdCategoria = 1, Capacidad = 4, Descripcion = "Deluxe", FechaCreacion = DateTime.Now, Estado = true };
            var pisoPlaceholder = new Piso { IdPiso = 1, Descripcion = "Primer Piso" };
            var RoomPlaceholder = new Habitacion
            {
                IdHabitacion = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1,
                Numero = "101",
                Detalle = "Habitacion de lujo",
                Estado = true,
                FechaCreacion = DateTime.Now,
                IdPiso = 1,
            };

            var serv1 = new Servicios
            {
                IdSercicio = 1,
                Nombre = "SPA",
                Descripcion = "Prueba 1",
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            var serv2 = new Servicios
            {
                IdSercicio = 2,
                Nombre = "CINE",
                Descripcion = "Prueba 2",
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            var TarifaPlaceholder = new Tarifas { IdTarifa = 1, IdCategoria = 1, PrecioPorNoche = 1000, Estado = true, Descripcion = "",
                FechaInicio = DateTime.Now.AddDays(2),
                FechaCreacion = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(200),
                Descuento = 0
            };
            var clientePlaceholder = new Client {
                Correo = "fulano@gmail.com",
                Documento = "1314252",
                NombreCompleto = "Fulano de Tal",
                Estado = true,
                TipoDocumento = "Cedula",
                FechaCreacion = DateTime.Now,
            };
            var ReservToReadPlaceHolder = new Reservation
            {

                IdCliente = 1,
                IdHabitacion = 1,
                FechaEntrada = DateTime.Now.AddDays(2),
                FechaSalida = DateTime.Now.AddDays(3),
                PrecioInicial = 1000,
                PrecioRestante = 1,
                CostoPenalidad = 0,
                TotalPagado = 100,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            var ReservCanceledPlaceHolder = new Reservation
            {

                IdCliente = 1,
                IdHabitacion = 1,
                FechaEntrada = DateTime.Now.AddDays(31),
                FechaSalida = DateTime.Now.AddDays(33),
                PrecioInicial = 1000,
                PrecioRestante = 1,
                CostoPenalidad = 0,
                TotalPagado = 100,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Cancelada,
                FechaCreacion = DateTime.Now
            };

            _context.Categorias.Add(CategoryPlaceholder);
            _context.Pisos.Add(pisoPlaceholder);
            _context.Habitaciones.Add(RoomPlaceholder);
            _context.Tarifas.Add(TarifaPlaceholder);
            _context.Clients.Add(clientePlaceholder);
            _context.Servicios.Add(serv1);
            _context.Servicios.Add(serv2);
            _context.Reservations.Add(ReservToReadPlaceHolder);
            _context.Reservations.Add(ReservCanceledPlaceHolder);
            var serviciosPorCategoria1 = new ServicioPorCategoria
            {
                CategoriaID = 1,
                ServicioID = 1,
                Precio = 200
            };
            /*
            var serviciosPorCategoria2 = new ServicioPorCategoria
            {
                CategoriaID = 1,
                ServicioID = serv2.IdSercicio,
                Precio = 200
            };*/
            _context.ServicioPorCategorias.Add(serviciosPorCategoria1);
            reservationRepository = new ReservationRepository(_context, new FakeLoggingServices(), new ReservationValidator());
            _context.SaveChanges();
            reservTestId = ReservToReadPlaceHolder.IdRecepcion;
            reservToUpdate = ReservToReadPlaceHolder;
            _reservCanceled = ReservCanceledPlaceHolder;
            categoryId = CategoryPlaceholder.IdCategoria;
        }

        [Fact]
        public async void SaveEntityAsync_WhenReservationIsNull_returnsFalse()
        {
            //Arrange
            Reservation reservation = null;

            //Act
            var result = await  reservationRepository.SaveEntityAsync(reservation);

            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("La entidad a guardar no puede ser nula", result.Message);
        }

        [Fact]
        public async void SaveEntityAsync_WhenClientIdIsZero_returnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 0,
                IdHabitacion = 1,
                FechaEntrada = DateTime.Now,
                FechaSalida = DateTime.Now.AddDays(1),
                PrecioInicial = 100,
                TotalPagado = 50,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //Act
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("El ID del cliente no puede ser cero", result.Message);
        }


        [Fact]
        public void SaveEntityAsync_WhenRoomIdIsZero_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now,
                FechaSalida = DateTime.Now.AddDays(1),
                PrecioInicial = 100,
                TotalPagado = 50,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = reservationRepository.SaveEntityAsync(reservation).Result;
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El ID de la habitación no puede ser cero", result.Message);
        }

        [Fact]
        public void SaveEntityAsync_WhenEntryDateIsNull_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = null,
                FechaSalida = DateTime.Now.AddDays(1),
                PrecioInicial = 100,
                TotalPagado = 50,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Pendiente,
            };

            var result = reservationRepository.SaveEntityAsync(reservation).Result;
            Assert.False(result.IsSuccess);
            Assert.Contains("La fecha de entrada no puede ser nula", result.Message);
        }

        [Fact]
        public async void SaveEntityAsync_WhenObservationExceeds800Characters_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now,
                FechaSalida = DateTime.Now.AddDays(1),
                PrecioInicial = 100,
                TotalPagado = 50,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La observación es demasiado larga, no puede pasar de 800 caracteres", result.Message);
        }

        [Fact]
        public async void SaveEntityAsync_WhenExitDateIsBeforeEntryDate_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now.AddDays(4),
                FechaSalida = DateTime.Now.AddDays(1),
                PrecioInicial = 100,
                TotalPagado = 50,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La fecha de salida debe ser posterior a la fecha de entrada", result.Message);
        }

        [Fact]
        public async void SaveEntityAsync_WhenEntryDateIsBeforeNow_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now.AddDays(-4),
                FechaSalida = DateTime.Now,
                PrecioInicial = 100,
                TotalPagado = 50,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La fecha de entrada debe ser posterior a la fecha actual", result.Message);
        }


        [Fact]
        public async void SaveEntityAsync_WhenInitialPriceIsInvalid_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now.AddDays(-4),
                FechaSalida = DateTime.Now,
                PrecioInicial = 100000000.00m,
                TotalPagado = 50,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El precio inicial no es una cantidad valida", result.Message);
        }



        [Fact]
        public async void SaveEntityAsync_WhenTotalPaidIsInvalid_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now.AddDays(-4),
                FechaSalida = DateTime.Now,
                PrecioInicial = 100000000.00m,
                TotalPagado = 0,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El Total pagado no es una cantidad valida", result.Message);
        }


        [Fact]
        public async void SaveEntityAsync_WhenPenaltyCostIsInvalid_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now.AddDays(-4),
                FechaSalida = DateTime.Now,
                PrecioInicial = 100000000.00m,
                CostoPenalidad = 100000000.00m,
                TotalPagado = 0,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El costo penalidad no es una cantidad valida", result.Message);
        }

        [Fact]
        public async void SaveEntityAsync_WhenRemainingPriceIsInvalid_ReturnsFalse()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 0,
                FechaEntrada = DateTime.Now.AddDays(-4),
                FechaSalida = DateTime.Now,
                PrecioInicial = 100000000.00m,
                CostoPenalidad = 100000000.00m,
                PrecioRestante = -100000000.00m,
                TotalPagado = 0,
                Observacion = new string('A', 809),
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };
            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("El precio restante no  es una cantidad valida", result.Message);
        }



        [Fact]
        public async void GetDisponibleRoomsOfCategoryInTimeLapse_WhenCategoryIdIsZero_ReturnsFalse()
        {
            //Arrange
            int categoryId = 0;
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(1);
            //ACT
            var result = await reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(start, end, categoryId);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado una categoría.", result.Message);
        }

        [Fact]
        public async void GetDisponibleRoomsOfCategoryInTimeLapse_WhenExitDateIsBeforeEntryDate_ReturnsFalse()
        {
            //Arrange
            int categoryId = 1;
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(-1);
            //ACT
            var result = await reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(start, end, categoryId);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("La fecha de inicio no puede ser mayor a la fecha de fin.", result.Message);
        }


        [Fact]
        public async void GetPricesForServicesinRoomCategory_WhenCategoryIdIsZero_ReturnsFalse()
        {
            //Arrange
            int categoryId = 0;
            IEnumerable<int> servicesIds = new List<int> { 1, 2, 3};
            //ACT
            var result = await reservationRepository.GetPricesForServicesinRoomCategory(categoryId, servicesIds);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado una categoría.", result.Message);
        }


        [Fact]
        public async void GetPricesForServicesinRoomCategory_WhenAreServicesIdsAreNotSpecified_ReturnsFalse()
        {
            //Arrange
            int categoryId = 1;
            IEnumerable<int> servicesIds = new List<int>();
            //ACT
            var result = await reservationRepository.GetPricesForServicesinRoomCategory(categoryId, servicesIds);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se han especificado servicios", result.Message);
        }

        [Fact]
        public async void HasRoomCapacity_WhenCategoryIdIsZero_ReturnsFalse()
        {
            //Arrange
            int categoryId = 0;
            int capacity = 1;
            //ACT
            var result = await reservationRepository.HasRoomCapacity(categoryId, capacity);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Categoria no especificada", result.Message);
        }

        [Fact]
        public async void HasRoomCapacity_WhenCapacitiIsZero_ReturnsFalse()
        {
            //Arrange
            int categoryId = 1;
            int capacity = 0;
            //ACT
            var result = await reservationRepository.HasRoomCapacity(categoryId, capacity);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Numero de personas no valido", result.Message);
        }

        [Fact]
        public async void GetEntityByIdAsync_WhenIdIsZero_ReturnsFalse()
        {
            //Arrange
            int id = 0;
            //ACT
            var result = await reservationRepository.GetEntityByIdAsync(id);
            //Assert
            Assert.Null(result);
        }


        [Fact]
        public async void ExistsAsync_WhenFilterIsNull_ReturnsFalse()
        {
            //Arrange
            Expression<Func<Reservation, bool>> filter = null;
            //ACT
            var result = await reservationRepository.ExistsAsync(filter);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void GetAllAsync_WhenFilterIsNull_ReturnsFalse()
        {
            //Arrange
            Expression<Func<Reservation, bool>> filter = null;
            //ACT
            var result = await reservationRepository.GetAllAsync(filter);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado un filtro", result.Message);
        }

        [Fact]
        public async void GetReservationsByClientId_WhenClientIdIsZero_ReturnsFalse()
        {
            //Arrange
            int clienteId = 0;
            //ACT
            var result = await reservationRepository.GetReservationsByClientId(clienteId);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado un cliente.", result.Message);
        }

        [Fact]
        public async void GetCategoryForReserv_WhenCategoryIdIsZero_ReturnsFalse()
        {
            //Arrange
            int id = 0;
            //ACT
            var result = await reservationRepository.GetCategoryForReserv(id, DateTime.Now, DateTime.Now.AddDays(1));
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Categoria no especificada", result.Message);
        }

        [Fact]
        public async void GetCategoryForReserv_WhenEndDateIsBeforeThanStart_ReturnsFalse()
        {
            //Arrange
            int id = 1;
            //ACT
            var result = await reservationRepository.GetCategoryForReserv(id, DateTime.Now, DateTime.Now.AddDays(-1));
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Fecha de fin no puede ser anterior a la fecha de inicio", result.Message);
        }

        [Fact]
        public async void GetCategoryForReserv_WhenIdIsValid_ReturnsTrue()
        {
            //Arrange
            int id = categoryId;
            //ACT
            var result = await reservationRepository.GetCategoryForReserv(id, DateTime.Now, DateTime.Now.AddDays(1));
            //Assert
          //  Assert.Contains("Fecha de fin no puede ser anterior a la fecha de inicio", result.Message);
            Assert.True(result.IsSuccess);
            //
        }


        [Fact]
        public async void GetCategoryForReservByRoom_WhenRoomIdIsZero_ReturnsFalse()
        {
            //Arrange
            int id = 0;
            //ACT
            var result = await reservationRepository.GetCategoryForReservByRoom(id, DateTime.Now, DateTime.Now.AddDays(1));
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Categoria no especificada", result.Message);
        }

        [Fact]
        public async void GetCategoryForReservByRoom_WhenEndDateIsBeforeThanStart_ReturnsFalse()
        {
            //Arrange
            int id = 1;
            //ACT
            var result = await reservationRepository.GetCategoryForReservByRoom(id, DateTime.Now, DateTime.Now.AddDays(-1));
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Fecha de fin no puede ser anterior a la fecha de inicio", result.Message);
        }

        [Fact]
        public async void GetTotalForServices_WhenReservationIdIsZero_ReturnsFalse()
        {
            //Arrange
            int id = 0;
            //ACT
            var result = await reservationRepository.GetTotalForServices(id);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Reservacion no especificada", result.Message);
        }

        [Fact]
        public async void ExistUser_WhenIdUserIsZero_ReturnsFalse()
        {
            //Arrange
            int id = 0;
            //ACT
            var result = await reservationRepository.ExistUser(id);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado el usuario", result.Message);
        }

        [Fact]
        public async void GetReservationsByClientId_WhenIdIsMoreThanZero_ReturnsTrue()
        {
            //Arrange
            int id = 1;
            //ACT
            var result = await reservationRepository.GetReservationsByClientId(id);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<List<ReservHabitClientModel>>(result.Data);
        }

        [Fact]
        public async void GetDisponibleRoomsOfCategoryInTimeLapse_WhenTheAreNotRoomsOfCategory_ReturnsFalse() 
        {
            //Arrange
            int id = 3;
            //ACT
            var result = await reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), id);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No hay habitaciones disponibles en la categoría especificada.", result.Message);
        }

        [Fact]
        public async void GetDisponibleRoomsOfCategoryInTimeLapse_WhenTheAreRoomsOfCategory_ReturnsTrue()
        {
            //Arrange
            int id = 1;
            //ACT
            var result = await reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(DateTime.Now.AddDays(15), DateTime.Now.AddDays(19), id);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<int>(result.Data);
        }

        [Fact]
        public async void GetDisponibleRoomsOfCategoryInTimeLapse_WhenThereAndExistingReservInTheSameTimeLapse_ReturnsTrue()
        {
            //Arrange
            int id = 1;
            //ACT
            var result = await reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(DateTime.Now.AddDays(2), DateTime.Now.AddDays(19), id, 1);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsType<int>(result.Data);
        }

        
        [Fact]
        public async void GetPricesForServicesinRoomCategory_WhenThereAreNotServicesForCategory_ReturnsFalse()
        {
            //Arrange
            int id = 2;
            IEnumerable<int>  services = new List<int>{
                1,
                2
            };
            //ACT
            var result = await reservationRepository.GetPricesForServicesinRoomCategory(id, services);  
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
        }

        [Fact]
        public async void GetPricesForServicesinRoomCategory_WhenThereAllAreDisponibleServicesForCategory_ReturnsTrue()
        {
            //Arrange
            int id = 1;
            IEnumerable<int> services = new List<int>{
                1
            };
            //ACT
            var result = await reservationRepository.GetPricesForServicesinRoomCategory(id, services);
            //Assert
            //Assert.Contains("eqeq", result.Message);
            Assert.True(result.IsSuccess);

            //Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
        }

        [Fact]
        public async void GetEntityByIdAsync_WhenReservExist_ReturnsReserv()
        {
            //Arrange
            int id = reservTestId;

            //ACT
            var result = await reservationRepository.GetEntityByIdAsync(id);    
            //Assert
            //Assert.Contains("eqeq", result.Message);
            Assert.IsType<Reservation>(result);

            //Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
        }


        [Fact]
        public async void SaveEntityAsync_WhenReservIsValid_ReturnsTrue()
        {
            //Arrange
            Reservation reservation = new Reservation
            {
                IdCliente = 1,
                IdHabitacion = 1,
                FechaEntrada = DateTime.Now.AddDays(20),
                FechaSalida = DateTime.Now.AddDays(25),
                PrecioInicial = 100,
                PrecioRestante = 1,
                CostoPenalidad = 0,
                TotalPagado = 50,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Pendiente,
            };

            //ACT
            var result = await reservationRepository.SaveEntityAsync(reservation);
            //Assert
            //Assert.Contains("eqeq", result.Message);
            Assert.True(result.IsSuccess);

            //Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
        }

        [Fact]
        public async void UpdateEntityAsync_WhenReservIsValid_ReturnsTrue()
        {
            //Arrange
            Reservation reservation = reservToUpdate;
            reservation.Observacion = "Nueva Observacion";

            //ACT
            var result = await reservationRepository.UpdateEntityAsync(reservation);
            //Assert
            //Assert.Contains("eqeq", result.Message);
            Assert.True(result.IsSuccess);

            //Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
        }


        [Fact]
        public async void UpdateEntityAsync_WhenReservIsCanceled_ReturnsTrue()
        {
            //Arrange
            Reservation reservation = _reservCanceled ;
            reservation.Observacion = "Nueva Observacion";

            //ACT
            var result = await reservationRepository.UpdateEntityAsync(reservation);
            //Assert
            
            Assert.False(result.IsSuccess);
            Assert.Contains("Solo puedes actualizar Reservaciones pendientes", result.Message);
            //Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
        }

        







    }
}
