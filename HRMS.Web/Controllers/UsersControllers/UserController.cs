 using HRMS.Application.DTOs.UsersDTOs.ClientDTOs;
using HRMS.Application.DTOs.UsersDTOs.UserDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.UsersControllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
		public UserController(IUserService userService, IClientService clientService)
        {
			_userService = userService;
            _clientService = clientService;
        }
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAll();
            if (result.IsSuccess)
            {
                var userList = (List<UserViewDTO>)result.Data;
                return View(userList);
			}
			return View();
		}


		// GET: UserController/Details/5
		public async Task<IActionResult> Details(int id)
        {
            var result = await _userService.GetById(id);
            if (result.IsSuccess)
            {
                var user = (UserViewDTO)result.Data;
                return View(user);
            }
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveUserDTO dto)
        {
            try
            {
                var user = await _userService.Save(dto);
                if (dto.IdUserRole == 1)
                {
                    var createdUser = user.Data;
                    var userID = createdUser.IdUser;
                    var client = new SaveClientDTO
                    {
                        IdUsuario = userID,
                        NombreCompleto = dto.NombreCompleto,
                        Clave = dto.Clave,
                        Correo = dto.Correo,
                        TipoDocumento = dto.TipoDocumento,
                        Documento = dto.Documento,
                        ChangeTime = dto.ChangeTime,
                        UserID = dto.UserID
                    };
                    var result = await _clientService.Save(client);
                    if (result.IsSuccess)
                        return RedirectToAction(nameof(Index));
                    return View();
                }

                if (user.IsSuccess)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _userService.GetById(id);
            if (result.IsSuccess)
            {
                var user = (UserViewDTO)result.Data;
                return View(user);
            }
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserClientDTO dto)
        {
            try
            {
                var user = await _userService.Update(dto);
                if (dto.IdUserRole == 1)
                {
                    var client = await _clientService.GetClientByUserIdAsync(dto.IdUsuario);
                    if(client != null)
                    {
                        var result = await _clientService.Update(dto);
                        if (result.IsSuccess)
                            return RedirectToAction(nameof(Index));
                        return View();
                    }
                }

                if (user.IsSuccess)
                     return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
