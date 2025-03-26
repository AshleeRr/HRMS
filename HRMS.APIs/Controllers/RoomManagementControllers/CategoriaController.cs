using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers.RoomManagementControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ApiControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriaController(ICategoryService categoryService , ILogger<CategoriaController> logger) : base(logger)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        ///  Obtiene todas las categorias activas
        /// </summary>
        /// <returns>Lista de categorias</returns>
        [HttpGet("GetAllCategorias")]
        [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todas las categorías");
            var result = await _categoryService.GetAll();
            return HandleResponse(result);
        }

        /// <summary>
        /// Obtiene una categoria mediante el parametro del Id
        /// </summary>
        /// <param name="id"></param>
        /// <returnsCategoria></returns>
        [HttpGet("GetCategoriaById{id}")]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Obteniendo Categoria con ID: {Id}", id);
            var validation = ValidateId(id);
            if (validation != null) return validation;
            
            var result = await _categoryService.GetById(id);
            return HandleResponse(result);

        }

        /// <summary>
        /// Create una nueva categoria
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Piso creado</returns>
        [HttpPost("CreateCategoria")]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
        {
            _logger.LogInformation("Creando una nueva categoria  {Descripcion}", dto.Descripcion);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _categoryService.Save(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Error al crear la categoria: {Message}", result.Message);
                return BadRequest(CreateProblemDetails(result.Message, StatusCodes.Status400BadRequest));
            }

            var categoriaDto = (CategoriaDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { id = categoriaDto.IdCategoria }, categoriaDto);
        }

        /// <summary>
        /// Actualiza una categoria mediante el Id,
        /// y su dto 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>Categoria</returns>
        [HttpPut("UpdateCategoriaById{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id != dto.IdCategoria)
            {
                return BadRequest(CreateProblemDetails(
                    "El ID en la URL no coincide con el ID en el cuerpo de la solicitud", 
                    StatusCodes.Status400BadRequest));
            }

            var result = await _categoryService.Update(dto);
            return HandleOperationResult(result);
        }

        /// <summary>
        /// Elimina una categoria mediante su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Categoria eliminada</returns>
        [HttpDelete("DeleteCategoriaById{id}")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminand la categoria ID: {Id}", id);
            
            var validation = ValidateId(id);
            if (validation != null) return validation;
            
            var dto = new DeleteCategoriaDto { IdCategoria = id };
            var result = await _categoryService.Remove(dto);
            
            return HandleOperationResult(result);
        }

        /// <summary>
        /// Busca las categorias que tengan un nombre de servicio asociado
        /// </summary>
        /// <param name="nombreServicio"></param>
        /// <returns>Categoria list</returns>
        [HttpGet("GetCategoriaByNombreServicio/{nombreServicio}")]
        [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByServicio(string nombreServicio)
        {
          _logger.LogInformation("Buscando categorias con nombre de servicio: {nombreServicio}", nombreServicio);
          
          var validation = ValidateString(nombreServicio, "nombreServicio");
          if(validation != null) return validation;
          
          var result = await _categoryService.GetCategoriaByServicio(nombreServicio);
          return HandleResponse(result);
        }

        /// <summary>
        /// Busca una categoria mediante sus descripcion
        /// </summary>
        /// <param name="descripcion"></param>
        /// <returns>Categoria por descripcion</returns>
        [HttpGet("GetCategoriaByDescripcion/{descripcion}")]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServiciosByDescripcion(string descripcion)
        {
            _logger.LogInformation("Buscando categorias con descripción: {descripcion}", descripcion);
            
            var validation = ValidateString(descripcion, "descripcion");
            if(validation != null) return validation;
            
            var result = await _categoryService.GetCategoriaByDescripcion(descripcion);
            return HandleResponse(result);
            
        }

        /// <summary>
        /// Busca categorias con x capacidad y las habitaciones relacionadas con ellas
        /// </summary>
        /// <param name="capacidad"></param>
        /// <returns>Habitaciones </returns>
        [HttpGet("GetHabitacionByCapacidad/{capacidad}")]
        public async Task<IActionResult> GetHabitacionesByCapacidad(int capacidad)
        {
            _logger.LogInformation("Buscando habitacion con capacidad: {capacida}", capacidad);

            if (capacidad <= 0)
            {
                return BadRequest("La capaciddad debe ser mayor a 0");
            }
            
            var result = await _categoryService.GetHabitacionesByCapacidad(capacidad);
            return HandleResponse(result);
        }
    }
}