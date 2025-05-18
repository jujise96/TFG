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

    private CajaDeComentariosViewModel ConstruirCajaDeComentariosViewModel(TipoEntidad tipoEntidad, int entidadId, IEnumerable<Comentario> comentarios)
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

    private ComentarioViewModel ConvertirAViewModelConRespuestas(Comentario comentario, IEnumerable<Comentario> todosLosComentarios)
    {
        var comentarioViewModel = new ComentarioViewModel
        {
            Id = comentario.Id,
            Mensaje = comentario.Mensaje,
            FechaCreacion = comentario.FechaCreacion,
            UserId = comentario.UserId,
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
    public async Task<IActionResult> CrearComentario(string tipoEntidad, int entidadId, string mensaje, int? comentarioPadreId)
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
        if (usuario != null) {
            userId=usuario.Id;
        }

        var nuevoComentario = new Comentario
        {
            TipoEntidad = tipo,
            EntidadId = entidadId,
            Mensaje = mensaje,
            ComentarioPadreId = comentarioPadreId,
            FechaCreacion = DateTime.UtcNow,
            UserId = userId
        };

        bool resultado = await _comentarioService.GuardarComentario(nuevoComentario);

        if (resultado)
        {
            // Podrías devolver un PartialView con el nuevo comentario para actualizar la lista dinámicamente
            var comentarioViewModel = ConvertirAViewModelConRespuestas(nuevoComentario, new List<Comentario> { nuevoComentario }); // Solo el nuevo
            return PartialView("CajaComentarios/_ComentarioIndividual", comentarioViewModel);
            // O podrías simplemente devolver un Ok() y recargar la sección de comentarios con JS
            // return Ok();
        }
        else
        {
            return StatusCode(500, "Error al guardar el comentario.");
        }
    }

}
