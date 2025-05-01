using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TFG.Models;
using TFG.Services;

namespace TFG.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJuegoService juegoService;
    private readonly IMisionService misionService;
    private readonly IItemService itemService;
    private readonly ITrucoService trucoService;

    public HomeController(ILogger<HomeController> logger, IJuegoService juegoService, IMisionService misionService, IItemService itemService, ITrucoService trucoService)
    {
        _logger = logger;
        this.juegoService = juegoService;
        this.misionService = misionService;
        this.itemService = itemService;
        this.trucoService = trucoService;
    }

    public async Task<IActionResult> Index()
    {
        // Aquí puedes agregar la lógica para obtener los juegos y pasarlos a la vista
        var elemento = await juegoService.ListarJuegos();
        ViewBag.tipoelemento = "Juego";
        return View(elemento);
    }

    public async Task<IActionResult> Juego(int id)
    {
        var elemento = await juegoService.ObtenerJuegoPorIdAsync(id);
        ViewBag.tipoelemento = "Juego";
        return View(elemento);
    }

    public async Task<IActionResult> Misiones(int id)
    {
        var elemento = await juegoService.ObtenerQuestsPorJuegoAsync(id);
        ViewBag.tipoelemento = "Mision";
        return View("Index", elemento);
    }

    public async Task<IActionResult> Items(int id)
    {
        var elemento = await juegoService.ObtenerItemsPorJuegoAsync(id);
        ViewBag.tipoelemento = "Item";
        return View("Index", elemento);
    }

    public async Task<IActionResult> Mision(int id)
    {
        var elemento = await misionService.ObtenerMisionesPorIdAsync(id);
        ViewBag.tipoelemento = "Mision";
        return View("Mision", elemento);
    }

    public async Task<IActionResult> Item(int id)
     {
         var elemento = await itemService.ObtenerItemPorIdAsync(id);
         ViewBag.tipoelemento = "Item";
         return View("Item", elemento);
     }

     public async Task<IActionResult> Trucos(int id)
     {
         var elemento = await trucoService.ObtenerTrucoPorIdAsync(id);
         ViewBag.tipoelemento = "Trucos";
         return View("Trucos", elemento);
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
