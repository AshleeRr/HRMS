using HRMS.Application.DTOs.UsersDTOs.UserRoleDTOs;
using HRMS.Application.Interfaces.IUsersServices;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.UsersControllers
{
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        public UserRoleController(IUserRoleService userRoleService) 
        {
            _userRoleService = userRoleService;
        }
        // GET: UserRoleController
        public async Task<IActionResult> Index()
        {
            var result = await _userRoleService.GetAll();
            if(result.IsSuccess)
            {
                var userRolesList = (List<UserRoleViewDTO>)result.Data;
                return View(userRolesList);
            }
            return View();
        }

        // GET: UserRoleController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _userRoleService.GetById(id);
            if(result.IsSuccess)
            {
                var userRole = (UserRoleViewDTO)result.Data;
                return View(userRole);
            }
            return View();
        }

        // GET: UserRoleController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: UserRoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveUserRoleDTO dto)
        {
            try
            {
                var result = await _userRoleService.Save(dto);
                if(result.IsSuccess)
					return RedirectToAction(nameof(Index));


				return View();
			}
            catch
            {
                return View();
            }
        }

        // GET: UserRoleController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
			var result = await _userRoleService.GetById(id);
			if (result.IsSuccess)
			{
				var userRole = (UserRoleViewDTO)result.Data;
				return View(userRole);
			}
			return View();
		}

        // POST: UserRoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserRoleDTO dto)
        {
            try
            {
                var result = await _userRoleService.Update(dto);
                if (result.IsSuccess)
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
