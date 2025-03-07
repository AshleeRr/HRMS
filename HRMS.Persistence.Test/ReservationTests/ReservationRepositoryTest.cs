using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Repository;

namespace HRMS.Application.Test.ReservationTests
{
    public class ReservationRepositoryTest
    {
        private IReservationRepository reservationRepository;

        [Fact]
        public void SaveEntityAsync_WhenReservationIsNull_returnsFalse()
        {
            //Arrange
            Reservation reservation = null;

            //Act
            var result = reservationRepository.SaveEntityAsync(reservation).Result;

            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("La reservación no puede ser nula", result.Message);
        }

        [Fact]
        public void SaveEntityAsync_WhenClientIdIsZero_returnsFalse()
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
            var result = reservationRepository.SaveEntityAsync(reservation).Result;
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("El ID del cliente no puede ser cero", result.Message);
        }
    }
}
