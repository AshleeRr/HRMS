using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Models.UsersModels.ClientModels;

namespace WebApi.Controllers.UsersControllers
{
    public class ClientController : Controller
    {
        // GET: ClientController
        public async Task<IActionResult> Index()
        {
            List<ClientModel> clients = new List<ClientModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:7175/api/Client/clients");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        clients = JsonConvert.DeserializeObject<List<ClientModel>>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return View(clients);
        }

        // GET: ClientController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ClientModel cliente = new ClientModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/Client/client/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        cliente = JsonConvert.DeserializeObject<ClientModel>(jsonString);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return View(cliente);
        }

    }
}
