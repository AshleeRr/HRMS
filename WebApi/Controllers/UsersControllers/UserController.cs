using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Models;
using WebApi.Models.UsersModels.UserModels;

namespace WebApi.Controllers.UsersControllers
{
    public class UserController : Controller
    {
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            List<UserModel> usuarios = new List<UserModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:7175/api/User/users");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<OperationResult>(jsonString);
                        if (result?.IsSuccess == true && result.Data != null)
                        {
                            usuarios = JsonConvert.DeserializeObject<List<UserModel>>(result.Data.ToString());
                        }
                    }
                }

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
            UserModel usuario = new UserModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/User/user/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<OperationResult>(jsonString);
                        if (result?.IsSuccess == true && result.Data != null)
                        {
                            usuario = JsonConvert.DeserializeObject<UserModel>(result.Data.ToString());
                        }
                    }
                }

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
        public async Task<IActionResult> Create(UserSaveModel model)
        {
            OperationResult op = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync<UserSaveModel>("https://localhost:7175/api/User/user", model);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        op = await response.Content.ReadFromJsonAsync<OperationResult>();
                    }
                    else
                    {
                        ViewBag.Error = "Error al crear el usuario";
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

        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            UserModel usuario = new UserModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7175/api/User/user/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<OperationResult>(jsonString);

                        if (result != null && result.IsSuccess && result.Data != null)
                        {
                            var dataJson = JsonConvert.SerializeObject(result.Data);
                            usuario = JsonConvert.DeserializeObject<UserModel>(dataJson);
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
                return View();
            }

            return View(usuario);
        }


        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserModel model)
        {
            OperationResult op = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PutAsJsonAsync<UserModel>($"https://localhost:7175/api/User/user/{model.IdUsuario}", model);
                    if (response.IsSuccessStatusCode)
                    {
                        op = await response.Content.ReadFromJsonAsync<OperationResult>();
                    }
                    else
                    {
                        ViewBag.Error = "Error al actualizar el usuario";
                        return View();
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
