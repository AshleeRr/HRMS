using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infraestructure.Notification
{
    public class NotificationServiceFake : INotificationService
    {
        public async Task SendNotification(int userId, string message)
        {
            Console.WriteLine($"Enviando notificación a {userId}: {message}");
        }
    }
}
