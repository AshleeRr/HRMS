using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Models;
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
            UserRoleModel role = new UserRoleModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/UserRole/role/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();

                        try
                        {
                            role = JsonConvert.DeserializeObject<UserRoleModel>(jsonString);
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
                        var jsonString = await response.Content.ReadAsStringAsync();
                        op = await response.Content.ReadFromJsonAsync<OperationResult>();

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
            UserRoleModel rol = new UserRoleModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/UserRole/role/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<OperationResult>(jsonString);

                        if (result != null && result.IsSuccess && result.Data != null)
                        {
                            var dataJson = JsonConvert.SerializeObject(result.Data);
                            Console.WriteLine($"JSON antes de deserializar: {dataJson}");
                            rol = JsonConvert.DeserializeObject<UserRoleModel>(dataJson);
                        }
                        else
                        {
                            ViewBag.Message = "No se pudo procesar la respuesta de la API.";
                            return View();
                        }
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
            }
             return View(rol);
        }


        // POST: UserRoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRoleModel model)
        {
            OperationResult op = new OperationResult();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PutAsJsonAsync<UserRoleModel>($"https://localhost:7175/api/UserRole/role/{model.IdRolUsuario}", model);

                    if (response.IsSuccessStatusCode)
                    {
                        op = await response.Content.ReadFromJsonAsync<OperationResult>();
                    }
                    else
                    {
                        ViewBag.Error = "Error al actualizar el rol";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
