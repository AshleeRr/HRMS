using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Interfaces.IUsersServices;
using WebApi.Models.UsersModels;

namespace WebApi.Controllers.UsersControllers
{
    public class ClientController : Controller
    {
        private readonly IApiClient _client;
        private readonly IClientRepository _clientRepository;
        public ClientController(IApiClient client, IClientRepository clientRepository)
        {
            _client = client;
            _clientRepository = clientRepository;
        }
        // GET: ClientController
        public async Task<IActionResult> Index()
        {
            List<ClientModel> clients = new List<ClientModel>();
            try
            {
                clients = (await _clientRepository.GetAllAsync()).ToList();
                if (!clients.Any())
                {
                    TempData["Error"] = "Error cargando los datos";
                    return View(new List<ClientModel>());
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
                if (id <= 0)
                {
                    TempData["Error"] = "ID no válido.";
                    return RedirectToAction("Index");
                }
                else
                {
                    cliente = await _clientRepository.GetByIdAsync(id);
                }
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return View(cliente);
        }
         public async Task<IActionResult> Filter(int filterType, string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
            {
                TempData["Error"] = "Debe ingresar un filtro válido.";
                return RedirectToAction("Index");
            }
            List<ClientModel> clientes = new List<ClientModel>();
            switch (filterType)
            {
                case 1:
                    var clientById = await _clientRepository.GetByIdAsync(int.Parse(filterValue));
                    if (clientById != null)clientes.Add(clientById);
                    break;
                case 2:
                    var clientByDocument = await _clientRepository.GetClientByDocument(filterValue);
                    if (clientByDocument != null) clientes.Add(clientByDocument);
                    break;
                case 3:
                    var clientByEmail = await _clientRepository.GetClientByEmail(filterValue);
                    if (clientByEmail != null)clientes.Add(clientByEmail);
                    break;
                case 4:
                    clientes = (await _clientRepository.GetClientsByDocumentType(filterValue)).ToList();
                    break;
                default:
                    TempData["Error"] = "Filtro no válido.";
                    return RedirectToAction("Index");
            }
            return View("Index", clientes);
        }
    }
}
