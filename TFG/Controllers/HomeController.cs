using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TFG.Models;

namespace TFG.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }














    /*

    public async Task<IActionResult> ListaPlantilla(int juegoId)
    {
        var juego = await juegoService.ObtenerJuegoPorIdAsync(juegoId);
        if (juego == null)
        {
            return NotFound();
        }

        var quests = await juegoService.ObtenerQuestsPorJuegoAsync(juegoId);
        var items = await juegoService.ObtenerItemsPorJuegoAsync(juegoId);
        var truco = await juegoService.ObtenerTrucoPorJuegoAsync(juegoId);

        var viewModel = new JuegoViewModel
        {
            Id = juego.Id,
            IdElem = juego.IdElem,
            Nombre = juego.Nombre,
            Descripcion = juego.Descripcion,
            NombreJuego = juego.Nombre,
            Quests = quests,
            Items = items,
            Truco = truco
        };

        return View("ListaPlantilla", viewModel);
    }
    */
}
