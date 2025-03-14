using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
    public class RoomController : Controller
    {
        private readonly IHabitacionService habitacionService;

        public RoomController(IHabitacionService habitacionService)
        {
            this.habitacionService = habitacionService;
        }

        // GET: RoomController
        public async Task <IActionResult> Index()
        {
            var result=await habitacionService.GetAll();
            if (result.IsSuccess)
            {
                List<HabitacionDto> habitacionList = (List<HabitacionDto>)result.Data;
                return View(habitacionList);
            }
            return View("Index");
        }
        
        // GET: RoomController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(CreateHabitacionDTo dTo)
        {
            try
            {
                var result = await habitacionService.Save(dTo);
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

        // GET: RoomController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateHabitacionDto update)
        {
            try
            {
                var dto = new UpdateHabitacionDto()
                {
                    IdHabitacion = id
                };
                var result = await habitacionService.Update(dto);
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
        
        // POST: RoomController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var delete = new DeleteHabitacionDto { IdHabitacion = id };
                await habitacionService.Remove(delete);
            }
            catch
            {
                return View("Index");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
