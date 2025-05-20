using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TFG.Models;
using TFG.Services;

namespace TFG.Controllers;

public class ComentarioController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJuegoService juegoService;
    private readonly IMisionService misionService;
    private readonly IItemService itemService;
    private readonly ITrucoService trucoService;
    private readonly UserManager<Usuario> userManager;
    private readonly SignInManager<Usuario> signinmanager;
    private readonly IComentarioService _comentarioService;


    public ComentarioController(ILogger<HomeController> logger, IJuegoService juegoService, IMisionService misionService, IItemService itemService, ITrucoService trucoService, SignInManager<Usuario> signinmanager, UserManager<Usuario> userManager, IComentarioService comentarioService)
    {
        _logger = logger;
        this.juegoService = juegoService;
        this.misionService = misionService;
        this.itemService = itemService;
        this.trucoService = trucoService;
        this.signinmanager = signinmanager;
        this.userManager = userManager;
        _comentarioService = comentarioService;
    }

    public IActionResult Mensaje(string mensaje = "")
    {
        ViewBag.Mensaje = mensaje;
        return View();

    }


    [HttpGet]
    public async Task<IActionResult> MostrarComentarios(string tipoEntidad, int entidadId)
    {
        if (!Enum.TryParse<TipoEntidad>(tipoEntidad, true, out var tipo))
        {
            return BadRequest("Tipo de entidad no válido.");
        }

        if (entidadId <= 0)
        {
            return BadRequest("ID de entidad no válido.");
        }

        var comentarios = await _comentarioService.ObtenerComentariosPorEntidad(tipo, entidadId);
        var comentariosViewModel = ConstruirCajaDeComentariosViewModel(tipo, entidadId, comentarios);

        return PartialView("CajaComentarios/_CajaDeComentarios", comentariosViewModel);
    }

    private CajaDeComentariosViewModel ConstruirCajaDeComentariosViewModel(TipoEntidad tipoEntidad, int entidadId, IEnumerable<ComentarioViewModel> comentarios)
    {
        var viewModel = new CajaDeComentariosViewModel
        {
            TipoEntidad = tipoEntidad.ToString(), // Convertir el enum a string para el ViewModel
            EntidadId = entidadId,
            Comentarios = new List<ComentarioViewModel>(),
            TotalComentarios = comentarios.Count(),
            PaginaActual = 1, // Ejemplo
            TotalPaginas = 1 // Ejemplo
        };

        var comentariosPrincipales = comentarios.Where(c => c.ComentarioPadreId == null).ToList();

        foreach (var comentarioPrincipal in comentariosPrincipales)
        {
            viewModel.Comentarios.Add(ConvertirAViewModelConRespuestas(comentarioPrincipal, comentarios));
        }

        return viewModel;
    }

    private ComentarioViewModel ConvertirAViewModelConRespuestas(ComentarioViewModel comentario, IEnumerable<ComentarioViewModel> todosLosComentarios)
    {
        var comentarioViewModel = new ComentarioViewModel
        {
            Id = comentario.Id,
            Mensaje = comentario.Mensaje,
            FechaCreacion = comentario.FechaCreacion,
            UserId = comentario.UserId,
            NombreUsuario = comentario.NombreUsuario,
            ComentarioPadreId = comentario.ComentarioPadreId,
            Respuestas = new List<ComentarioViewModel>()
        };


        var respuestas = todosLosComentarios.Where(c => c.ComentarioPadreId == comentario.Id).ToList();
        foreach (var respuesta in respuestas)
        {
            comentarioViewModel.Respuestas.Add(ConvertirAViewModelConRespuestas(respuesta, todosLosComentarios));
        }

        return comentarioViewModel;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearComentario(string tipoEntidad, int entidadId, string mensaje, int? comentarioPadreId, int juegoId)
    {
        if (!Enum.TryParse<TipoEntidad>(tipoEntidad, true, out var tipo))
        {
            return BadRequest("Tipo de entidad no válido.");
        }

        if (entidadId <= 0 || string.IsNullOrWhiteSpace(mensaje))
        {
            return BadRequest("Datos de comentario no válidos.");
        }

        //var userId = userManager?.GetUserId(User);
        var userId = -1;
        var usuario = await userManager.GetUserAsync(User);
        if (usuario != null)
        {
            userId = usuario.Id;
        }

        var nuevoComentario = new ComentarioViewModel
        {
            JuegoId = juegoId, // Asigna el ID del juego correspondiente
            TipoEntidad = tipo,
            EntidadId = entidadId,
            Mensaje = mensaje,
            ComentarioPadreId = comentarioPadreId,
            FechaCreacion = DateTime.UtcNow,
            UserId = userId,
            NombreUsuario = usuario.NombreUsuario

        };

        bool resultado = await _comentarioService.GuardarComentario(nuevoComentario);

        if (resultado)
        {
            // Podrías devolver un PartialView con el nuevo comentario para actualizar la lista dinámicamente
            var comentarioViewModel = ConvertirAViewModelConRespuestas(nuevoComentario, new List<ComentarioViewModel> { nuevoComentario }); // Solo el nuevo
            return PartialView("CajaComentarios/_ComentarioIndividual", comentarioViewModel);
            // O podrías simplemente devolver un Ok() y recargar la sección de comentarios con JS
            // return Ok();
        }
        else
        {
            return StatusCode(500, "Error al guardar el comentario.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> EliminarComentario(int comentarioId)
    {
        var comentario = await _comentarioService.ObtenerComentariosPorId(comentarioId);

        if (comentario == null)
        {
            // Usa NotFound() para un 404 si el recurso no existe.
            // Redirigir a "Mensaje" no es ideal para una llamada AJAX.
            return NotFound();
        }

        var usuarioActual = await userManager.GetUserAsync(User);

        if (usuarioActual == null) // Aunque [Authorize] debería manejar esto, es buena práctica.
        {
            return Forbid(); // 403 Forbidden
        }

        var esAdmin = await userManager.IsInRoleAsync(usuarioActual, "Admin");

        // Verifica si el usuario actual es el creador o es administrador
        if (comentario.NombreUsuario == usuarioActual.NombreUsuario || esAdmin) // Usa usuarioActual.UserName para comparar con el nombre del usuario logueado
        {
            try
            {
                if (await _comentarioService.EliminarComentario(comentario.Id))
                {
                    // *** CAMBIO AQUÍ: Devuelve un simple OK (status 200) ***
                    return Ok();
                }
                else
                {
                    // Si el servicio reporta un error interno, devuelve un 500
                    return StatusCode(500, "La base de datos reportó un error al intentar eliminar el comentario.");
                }
            }
            catch (Exception ex) // Captura excepciones para un mejor diagnóstico
            {
                _logger.LogError(ex, "Error al eliminar el comentario con ID {ComentarioId}", comentario.Id);
                return StatusCode(500, "Ocurrió un error inesperado al eliminar el comentario.");
            }
        }
        else
        {
            // Si no tiene permisos, devuelve un 403 Forbidden
            return Forbid();
        }
    }

}
