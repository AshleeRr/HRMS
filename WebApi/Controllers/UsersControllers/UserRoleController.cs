using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using WebApi.Models;
using WebApi.Models.UsersModels.UserModels;
using WebApi.Models.UsersModels.UserRoleModels;

namespace WebApi.Controllers.UsersControllers
{
    public class UserRoleController : Controller
    {
        // GET: UserRoleController
        public async Task<IActionResult> Index()
        {
            List<UserRoleModel> roles = new List<UserRoleModel>();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:7175/api/UserRole/roles");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<OperationResult>(jsonString);

                        if (result?.IsSuccess == true && result.Data != null)
                        {
                            roles = JsonConvert.DeserializeObject<List<UserRoleModel>>(result.Data.ToString());
                        }

                    };
                }
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
            UserRoleByIdModel role = new UserRoleByIdModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/UserRole/role/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"JSON recibido: {jsonString}");

                        try
                        {
                            // Deserialización directa
                            role = JsonConvert.DeserializeObject<UserRoleByIdModel>(jsonString);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = $"Error al deserializar los datos: {ex.Message}";
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error de la API: {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            if (role == null)
            {
                return RedirectToAction("Index");
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
        public async Task<IActionResult> Create(UserRoleSaveModel model)
        {
            OperationResult op = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync<UserRoleSaveModel>("https://localhost:7175/api/UserRole/role", model);
                    if (response.IsSuccessStatusCode)
                    {
                        op = await response.Content.ReadFromJsonAsync<OperationResult>();
                        Console.WriteLine($"JSON recibido: {op}");

                    }
                    else
                    {
                        ViewBag.Error = "Error al crear el rol de usuario";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRoleController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            UserRoleUpdateModel rol = new UserRoleUpdateModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/UserRole/role/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"JSON recibido: {jsonString}");

                        // Deserializar directamente a UserRoleUpdateModel
                        rol = JsonConvert.DeserializeObject<UserRoleUpdateModel>(jsonString);
                    }
                    else
                    {
                        ViewBag.Message = $"Error de la API: {response.ReasonPhrase}";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error inesperado: {ex.Message}";
                return View();
            }

            if (rol == null)
            {
                ViewBag.Message = "No se encontró información para este rol.";
                return View();
            }

            return View(rol);
        }


        // POST: UserRoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRoleUpdateModel model)
        {
            OperationResult op = new OperationResult();

            try
            {
                using (var client = new HttpClient())
                {
                    // Crear modelo intermedio
                    var apiModel = new UserRoleUpdateApiModel
                    {
                        IdUserRole = model.IdRolUsuario,
                        Nombre = model.RolNombre, // Mapeo manual
                        Descripcion = model.Descripcion,
                        FechaCreacion = model.FechaCreacion,
                        Estado = model.Estado
                    };

                    var response = await client.PutAsJsonAsync($"https://localhost:7175/api/UserRole/role/{model.IdRolUsuario}", apiModel);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        op = JsonConvert.DeserializeObject<OperationResult>(jsonString);

                        if (op.IsSuccess)
                            return RedirectToAction(nameof(Index));
                        else
                            ViewBag.Error = $"API Error: {op.Message}";
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"Failed to update role: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado: {ex.Message}";
            }

            return View(model);
        }
    }
}
