using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Interfaces.IUsersServices;
using WebApi.Models;
using WebApi.Models.UsersModels;
using WebApi.Models.UsersModels.Validations;

namespace WebApi.Controllers.UsersControllers
{
    public class UserRoleController : Controller
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IApiClient _client;
        private readonly UserRoleValidations _validator;
        public UserRoleController(IUserRoleRepository userRoleRepository, IApiClient client, UserRoleValidations validator)
        {
            _userRoleRepository = userRoleRepository;
            _client = client;
            _validator = validator;
        }
        // GET: UserRoleController
        public async Task<IActionResult> Index()
        {
            List<UserRoleModel> roles = new List<UserRoleModel>();

            try
            {
                roles = (await _userRoleRepository.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return View(roles);
        }

        // GET: UserRoleController/Details/5
        public async Task<IActionResult> Details(int id)
        {

            if(!IsValidateId(id)) return RedirectToAction("Index");
            UserRoleModel role = new UserRoleModel();
            try
            {
                role = await _userRoleRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            return View(role);
        }



        // GET: UserRoleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRoleModel model)
        {
            OperationResult op = new OperationResult();
            try
            {
                if(!IsValidModel(model))return RedirectToAction("Index");
                
                op = await _userRoleRepository.CreateAsync(model);
                return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(model);
            }
        }

        // GET: UserRoleController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            UserRoleModel rol = new UserRoleModel();

            try
            {
                rol = await _userRoleRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            return View(rol);
        }


        // POST: UserRoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserRoleModel model)
        {
            OperationResult op = new OperationResult();

            try
            {
                if (!IsValidateId(id) || !IsValidModel(model))
                    return RedirectToAction("Index");

                op = await _userRoleRepository.UpdateAsync(id, model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: UserRoleController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            try
            {
                var rol = await _userRoleRepository.GetByIdAsync(id);
                return View(rol);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UserRoleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            try
            {
                var rol = await _userRoleRepository.DeleteAsync(id);
                if (rol.IsSuccess)return RedirectToAction(nameof(Index));
                
                else
                TempData["Error"] = "Error al eliminar";

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Filter(int filterType, string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
            {
                TempData["Error"] = "Debe ingresar un filtro válido.";
                return RedirectToAction("Index");
            }
            List<UserRoleModel> roles = new List<UserRoleModel>();
            List<UserModel> usersByRole = new List<UserModel>();
            switch (filterType)
            {
                case 1:
                    var rolById = await _userRoleRepository.GetByIdAsync(int.Parse(filterValue));
                    if (rolById != null)
                        roles.Add(rolById);
                    break;
                case 2:
                    var rolByDesc = await _userRoleRepository.GetRoleByDescription(filterValue);
                    if (rolByDesc != null)
                        roles.Add(rolByDesc);
                    break;
                case 3:
                    var rolByName = await _userRoleRepository.GetRoleByName(filterValue);
                    if (rolByName != null)
                        roles.Add(rolByName);
                    break;
                case 4:
                    usersByRole = (await _userRoleRepository.GetUsersByRole(int.Parse(filterValue))).ToList();
                    break;
                default:
                    TempData["Error"] = "Filtro no válido.";
                    return RedirectToAction("Index");
            }
            if (filterType == 4)
            {
                return View("Index2", usersByRole);
            }
            return View("Index", roles);
        }

        private bool IsValidateId(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID no válido.";
                return false;
            }
            return true;
        }
        private bool IsValidModel(UserRoleModel model)
        {
            var result = _validator.Validate(model);
            if (!result.IsSuccess)
            {
                TempData["Error"] = "Error al validar los campos.";
                return false;
            }
            return true;
        }
    }
}
