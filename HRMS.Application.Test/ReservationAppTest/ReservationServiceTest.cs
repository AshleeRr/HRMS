using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.DTOs.Reservation_2023_0731.ReservDtosValidator;
using HRMS.Application.Services.Reservation_2023_0731;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.ReservationValidator;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;
using HRMS.Persistence.Context;
using HRMS.Persistence.Repositories.Reserv;
using HRMS.Persistence.Test.ReservationTests;
using HRMS.Persistence.Test.TestContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Test.ReservationAppTest
{
    public class ReservationServiceTest
    {
        private readonly ReservationService _reservationService;
        private Reservation reservToUpdate;
        private Reservation _reservToCancel;
        private Reservation _reservCanceled;
        private Client clientePlaceholder;
        private int reservTestId;
        public ReservationServiceTest()
        {
            var _dbContextOptions = new DbContextOptionsBuilder<HRMSContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

            var _context = new HRMSContext(_dbContextOptions);
            _context.LoadPlaceHolders();
           
            clientePlaceholder = new Client
            {
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
                PrecioInicial = 100,
                PrecioRestante = 1,
                CostoPenalidad = 0,
                TotalPagado = 99,
                Adelanto = 10,
                Observacion = "Observacion",
                EstadoReserva = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now
            };

            var ReservToCancel = new Reservation
            {

                IdCliente = 1,
                IdHabitacion = 1,
                FechaEntrada = DateTime.Now.AddDays(32),
                FechaSalida = DateTime.Now.AddDays(43),
                PrecioInicial = 1000,
                PrecioRestante = 10,
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

            _context.Reservations.Add(ReservToReadPlaceHolder);
            _context.Reservations.Add(ReservCanceledPlaceHolder);
            _context.Reservations.Add(ReservToCancel);  
            _reservToCancel = ReservToCancel;
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
            var reservationRepository = new ReservationRepository(_context, new FakeLoggingServices(), new ReservationValidator());
            _context.SaveChanges();
            reservTestId = ReservToReadPlaceHolder.IdRecepcion;
            reservToUpdate = ReservToReadPlaceHolder;
            _reservCanceled = ReservCanceledPlaceHolder;
            _reservationService = new ReservationService(reservationRepository, new FakeLoggingServices());
        }


        [Fact]
        public async void GetById_WhenReservIdIsZero_returnsFalse()
        {
            //Arrange
            int idReserv = 0;
            //Act
            var result = await _reservationService.GetById(idReserv);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("El id no se ha especificado", result.Message);
        }

        [Fact]
        public async void CancelReservation_WhenReservIdIsZero_returnsFalse()
        {
            //Arrange
            int idReserv = 0;
            //Act
            var result = await _reservationService.CancelReservation(idReserv);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado la reserva a buscar", result.Message);
        }

        [Fact]
        public async void CancelReservation_WhenReservDoesntExist_returnsFalse()
        {
            //Arrange
            int idReserv = 100;
            //Act
            var result = await _reservationService.CancelReservation(idReserv);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha encontrado la reserva", result.Message);
        }

        [Fact]
        public async void CancelReservation_WhenReservIsNotPendiente_returnsFalse()
        {
            //Arrange
            int idReserv = _reservCanceled.IdRecepcion;
            //Act
            var result = await _reservationService.CancelReservation(idReserv);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Solo se pueden cancelar reservas Pendientes, no confirmadas ni canceladas", result.Message);
        }

        [Fact]
        public async void CancelReservation_WhenReservIsValid_returnsTrue()
        {
            //Arrange
            int idReserv = _reservToCancel.IdRecepcion;
            //Act
            var result = await _reservationService.CancelReservation(idReserv);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            //
        }


        [Fact]
        public async void ConfirmReservation_WhenReservIdIsZerod_returnsFalse()
        {
            //Arrange
            ReservationConfirmDTO dto = new ReservationConfirmDTO
            {
                ReservationId = 0,
                UserID = 1,
            };
            //Act
            var result = await _reservationService.ConfirmReservation(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha especificado la reserva a confirmar", result.Message);
            //
        }

        [Fact]
        public async void ConfirmReservation_WhenReservStateIsNotPendiente_returnsFalse()
        {
            //Arrange
            ReservationConfirmDTO dto = new ReservationConfirmDTO
            {
                ReservationId = _reservCanceled.IdRecepcion,
                UserID = 1,
            };
            //Act
            var result = await _reservationService.ConfirmReservation(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Solo se pueden confirmar reservas Pendientes, no confirmadas ni canceladas", result.Message);
            //
        }

        [Fact]
        public async void ConfirmReservation_WhenReservDosentExist_returnsFalse()
        {
            //Arrange
            ReservationConfirmDTO dto = new ReservationConfirmDTO
            {
                ReservationId = 120,
                UserID = 1,
            };
            //Act
            var result = await _reservationService.ConfirmReservation(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("No se ha encontrado la reserva a confirmar", result.Message);
            //
        }

        [Fact]
        public async void ConfirmReservation_WhenAbonoIsNotValid_returnsFalse()
        {
            //Arrange
            ReservationConfirmDTO dto = new ReservationConfirmDTO
            {
                ReservationId = 1,
                UserID = 1,
                Abono = 0
            };
            //Act
            var result = await _reservationService.ConfirmReservation(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("El abono no coincide con el precio restante", result.Message);
            //
        }

        [Fact]
        public async void ConfirmReservation_WhenReservIsValid_returnsTrue()
        {
            //Arrange
            ReservationConfirmDTO dto = new ReservationConfirmDTO
            {
                ReservationId = 1,
                UserID = 1,
                Abono = 1
            };
            //Act
            var result = await _reservationService.ConfirmReservation(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            //Assert.Contains("El abono no coincide con el precio restante", result.Message);
            Assert.True(result.IsSuccess);
            
            //
        }

        [Fact]
        public async void GetReservationByClientID_WhenReservIdIsZero_returnsFalse()
        {
            //Arrange
            int id = 0;
            //Act
            var result = await _reservationService.GetReservationByClientID(id); 
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("El id no se ha especificado", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void GetReservationByClientID_WhenReservIdIsValid_returnsTrue()
        {
            //Arrange
            int id = 1;
            //Act
            var result = await _reservationService.GetReservationByClientID(id);
            //Assert
           //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            //Assert.Contains("El id no se ha especificado", result.Message);
            Assert.True(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WhenServicesAreNotDisponibleForRoomCategory_returnsFalse()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 1,
                Adelanto = 1,
                ChangeTime = DateTime.Now,
                UserID = 1,
                Services = [1, 2],
                PeopleNumber = 2,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(2),
                Out = DateTime.Now.AddDays(5),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("No todos los servicios se encuentran disponibles para esta categoria de habitación", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WhenRoomCategoryDosentExist_returnsFalse()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 341,
                Adelanto = 1,
                ChangeTime = DateTime.Now,
                UserID = clientePlaceholder.IdCliente,
                Services = [1],
                PeopleNumber = 2,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(2),
                Out = DateTime.Now.AddDays(5),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("No se ha encontrado la categoria de habitación seleccionada", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WhenRoomCategoryHasNotCapacity_returnsFalse()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 1,
                Adelanto = 1,
                ChangeTime = DateTime.Now,
                UserID = clientePlaceholder.IdCliente,
                Services = [1],
                PeopleNumber = 5,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(2),
                Out = DateTime.Now.AddDays(5),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("La cantidad de personas supera la capacidad de la habitación", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WheAreNotRoomsForCategory_returnsFalse()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 1,
                Adelanto = 1,
                ChangeTime = DateTime.Now,
                UserID = clientePlaceholder.IdCliente,
                Services = [1],
                PeopleNumber = 2,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(2),
                Out = DateTime.Now.AddDays(3),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("No hay habitaciones disponibles en la categoría especificada.", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WheAdelantoIsMoreThanTotal_returnsFalse()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 1,
                Adelanto = 100000,
                ChangeTime = DateTime.Now,
                UserID = clientePlaceholder.IdCliente,
                Services = [1],
                PeopleNumber = 2,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(80),
                Out = DateTime.Now.AddDays(90),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("El adelanto no puede ser mayor al total de la reserva", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WheAdelantoIsLessThan30PercentOfTotal_returnsFalse()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 1,
                Adelanto = 1,
                ChangeTime = DateTime.Now,
                UserID = clientePlaceholder.IdCliente,
                Services = [1],
                PeopleNumber = 2,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(80),
                Out = DateTime.Now.AddDays(90),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("El adelanto debe ser al menos el 30% del total de la reserva", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Save_WheDtoIsValid_returnsTrue()
        {
            //Arrange
            ReservationAddDTO dto = new ReservationAddDTO
            {
                RoomCategoryID = 1,
                Adelanto = 4000,
                ChangeTime = DateTime.Now,
                UserID = clientePlaceholder.IdCliente,
                Services = [1],
                PeopleNumber = 2,
                Observations = "Observacion",
                In = DateTime.Now.AddDays(80),
                Out = DateTime.Now.AddDays(90),
            };
            //Act
            var result = await _reservationService.Save(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            //Assert.Contains("El adelanto debe ser al menos el 30% del total de la reserva", result.Message);
            Assert.True(result.IsSuccess);

            //
        }

        [Fact]
        public async void Update_WheReservationDosentExist_returnsFalse()
        {
            //Arrange
            ReservationUpdateDTO dto = new ReservationUpdateDTO
            {
                     ID =100,
                     In = DateTime.Now,
                     Out = DateTime.Now,
                     Observations = "",
                     AbonoPenalidad = 0
            };
            //Act
            var result = await _reservationService.Update(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("No se ha encontrado la reserva a actualizar", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Update_WheReservationIsNotPendiente_returnsFalse()
        {
            //Arrange
            ReservationUpdateDTO dto = new ReservationUpdateDTO
            {
                ID = _reservCanceled.IdRecepcion,
                In = DateTime.Now,
                Out = DateTime.Now,
                Observations = "",
                AbonoPenalidad = 0
            };
            //Act
            var result = await _reservationService.Update(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            Assert.Contains("No se puede modificar cuyo estado no sea pendiente", result.Message);
            Assert.False(result.IsSuccess);

            //
        }

        [Fact]
        public async void Update_WheReservationIsValidToUpdate_returnsTrue()
        {
            //Arrange
            ReservationUpdateDTO dto = new ReservationUpdateDTO
            {
                ID = reservToUpdate.IdRecepcion,
                In = DateTime.Now.AddDays(50),
                Out = DateTime.Now.AddDays(60),
                Observations = "",
                AbonoPenalidad = 0
            };
            //Act
            var result = await _reservationService.Update(dto);
            //Assert
            //Assert.Contains("srfrq", result.Message);
            Assert.IsType<OperationResult>(result);
            //Assert.Contains("No se puede modificar cuyo estado no sea pendiente", result.Message);
            Assert.True(result.IsSuccess);

            //
        }




    }
}
