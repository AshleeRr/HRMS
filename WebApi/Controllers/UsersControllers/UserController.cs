using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Interfaces.IUsersServices;
using WebApi.Models;
using WebApi.Models.UsersModels;
using WebApi.Models.UsersModels.Validations;

namespace WebApi.Controllers.UsersControllers
{
    public class UserController : Controller
    {
        private readonly IApiClient _client;
        private readonly UserValidations _validator;
        private readonly IUserRepository _userRepository;
        public UserController(IApiClient client, UserValidations validator, IUserRepository userRepository)
        {
            _client = client;
            _validator = validator;
            _userRepository = userRepository;
        }
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            List<UserModel> usuarios = new List<UserModel>();
            try
            {
                usuarios = (await _userRepository.GetAllAsync()).ToList();

            }
            catch(Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            return View(usuarios);
        }

        // GET: UserController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            UserModel usuario = new UserModel();
            try
            {
                usuario = await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            return View(usuario);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model)
        {
            OperationResult op = new OperationResult();
            try
            {
                if (!IsValidModel(model)) return RedirectToAction("Index");
                op = await _userRepository.CreateAsync(model);
                return RedirectToAction(nameof(Index));

            }
            catch(Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View();
            }
        }

        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            UserModel usuario = new UserModel();

            try
            {
                
                usuario = await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error inesperado: {ex.Message}";
            }

            return View(usuario);
        }


        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserModel model)
        {
            OperationResult op = new OperationResult();
            try
            {
                if (!IsValidateId(id) || !IsValidModel(model))
                    return RedirectToAction("Index");

                    op = await _userRepository.UpdateAsync(id, model);
                    return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            try
            {
                var usuario = await _userRepository.GetByIdAsync(id);
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            if (!IsValidateId(id)) return RedirectToAction("Index");
            try
            {
                var usuario = await _userRepository.DeleteAsync(id);
                if (usuario.IsSuccess)return RedirectToAction(nameof(Index));
                
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
            List<UserModel> usuarios = new List<UserModel>();
            switch (filterType)
            {
                case 1:
                    var userById = await _userRepository.GetByIdAsync(int.Parse(filterValue));
                    if (userById != null)usuarios.Add(userById);
                    break;
                case 2:
                    usuarios = await _userRepository.GetUsersByName(filterValue);
                    break;
                case 3:
                    var userByEmail = await _userRepository.GetUserByEmailAsync(filterValue);
                    if (userByEmail != null)usuarios.Add(userByEmail);
                    break;
                case 4:
                    usuarios = (await _userRepository.GetUsersByTypeDocumentAsync(filterValue)).ToList();
                    break;
                case 5:
                    var userByDoc = await _userRepository.GetUserByDocumentAsync(filterValue);
                    if (userByDoc != null) usuarios.Add(userByDoc);
                    break;
                default:
                    TempData["Error"] = "Filtro no válido.";
                    return RedirectToAction("Index");
            }
            return View("Index", usuarios);
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
        private bool IsValidModel(UserModel model)
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
