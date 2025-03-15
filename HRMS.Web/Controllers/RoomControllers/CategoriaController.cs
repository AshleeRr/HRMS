using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoriaController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        // GET: CategoriaController
        public async Task <IActionResult> Index()
        {
            var result = await categoryService.GetAll();
            if (result.IsSuccess)
            {
                List<CategoriaDto> categoryList = (List<CategoriaDto>)result.Data;
                return View(categoryList);
            }
            return View();
        }

        // GET: CategoriaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoriaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(CreateCategoriaDto dto)
        {
            try
            {
                var result = await categoryService.Save(dto);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoriaController/Edit/5
        public async Task <IActionResult> Edit(int id)
        {
            try
            {
                var result = await categoryService.GetById(id);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message ?? "El categoria no fue encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var categoria = result.Data as CategoriaDto;
                if (categoria == null)
                {
                    TempData["ErrorMessage"] = "Error al obtener las categorias.";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateCategoriaDto()
                {
                    IdCategoria = categoria.IdCategoria,
                    Descripcion = categoria.Descripcion,
                    IdServicio = categoria.IdServicio,
                    Capacidad = categoria.Capacidad
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }        
        }

        // POST: CategoriaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(UpdateCategoriaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var result = await categoryService.Update(dto);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Categoria actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "No se pudo actualizar la categoria.");
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(dto);
            }
        }

        // GET: CategoriaController/Delete/5


        // POST: CategoriaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Delete(int id)
        {
            try
            {
                var delete = new DeleteCategoriaDto() { IdCategoria = id };
                var result = await categoryService.Remove(delete);

                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Categoria eliminado correctamente.";
                    return Json(new { success = true, message = "Categoria eliminado correctamente." });
                }
                else
                {
                    string errorMessage = result?.Message ?? "No se pudo eliminar la categoria.";
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}
