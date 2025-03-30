using HRMS.Application.DTOs.UsersDTOs.ClientDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.UsersControllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }
        // GET: ClientController
        public async Task<IActionResult> Index()
        {
            var result = await _clientService.GetAll();
            if (result.IsSuccess)
            {
                var clientsList = (List<ClientViewDTO>)result.Data;
                return View(clientsList);

            }
            return View();
        }

        // GET: ClientController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _clientService.GetById(id);
            if (result.IsSuccess)
            {
                var client = (ClientViewDTO)result.Data;
                return View(client);

            }
            return View();
        }
        /*
        // GET: ClientController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClientController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/

    }
}
