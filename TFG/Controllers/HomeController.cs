using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<Usuario> userManager;
    private readonly SignInManager<Usuario> signinmanager;

    public HomeController(ILogger<HomeController> logger, IJuegoService juegoService, IMisionService misionService, IItemService itemService, ITrucoService trucoService, SignInManager<Usuario> signinmanager, UserManager<Usuario> userManager)
    {
        _logger = logger;
        this.juegoService = juegoService;
        this.misionService = misionService;
        this.itemService = itemService;
        this.trucoService = trucoService;
        this.signinmanager = signinmanager;
        this.userManager = userManager;
    }

    public IActionResult Mensaje(string mensaje = "")
    {
        ViewBag.Mensaje = mensaje;
        return View();
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
        ViewBag.idJuego = id;
        return View(elemento);
    }

    public async Task<IActionResult> Misiones(int id)
    {

        var elementos = await juegoService.ObtenerQuestsPorJuegoAsync(id);
        if (signinmanager.IsSignedIn(User))
        {
            var usuario = await userManager.GetUserAsync(User); // Correctly fetch the user object
            if (usuario != null)
            {
                var elementoscompletados = await misionService.ObtenerQuestsPorJuegoyUsuario(id, usuario.Id); // Use the correct instance of misionService
                foreach (var elemento in elementos)
                {
                    elemento.idJuego = id;
                    if (elementoscompletados.Any(c => c.MisionId == elemento.Id))//si el id de elemento.id esta en elementoscompletados.MisionID
                    {
                        elemento.completada = true;
                    }
                    else
                    {
                        elemento.completada = false;
                    }
                }
            }
        }

        ViewBag.tipoelemento = "Mision";
        ViewBag.idJuego = id;
        return View("Index", elementos);
    }

    public async Task<IActionResult> Items(int id)
    {
        var elementos = await juegoService.ObtenerItemsPorJuegoAsync(id);
        if (signinmanager.IsSignedIn(User))
        {
            var usuario = await userManager.GetUserAsync(User); // Correctly fetch the user object
            if (usuario != null)
            {
                var elementoscompletados = await itemService.ObtenerItemPorJuegoyUsuario(id, usuario.Id); // Use the correct instance of misionService
                foreach (var elemento in elementos)
                {
                    elemento.idJuego = id;
                    if (elementoscompletados.Any(c => c.Id == elemento.Id))//si el id de elemento.id esta en elementoscompletados.MisionID
                    {
                        elemento.completada = true;
                    }
                    else
                    {
                        elemento.completada = false;
                    }
                }
            }
        }
        ViewBag.tipoelemento = "Item";
        ViewBag.idJuego = id;
        return View("Index", elementos);
    }

    public async Task<IActionResult> Mision(int id, int idJuego)
    {
        var elemento = await misionService.ObtenerMisionesPorIdAsync(id);
        var misionvm = new MisionViewModel
        {
            Id = elemento.Id,
            idJuego = elemento.JuegoId,
            Nombre = elemento.Nombre,
            Descripcion = elemento.Descripcion,
            Imagen = elemento.Imagen,
            StartTrigger = elemento.StartTrigger,
            Bugs = elemento.Bugs,
            TipoQuest = elemento.TipoQuest,
            IdElem = elemento.IdElem,
            Completada = false
        };
        if (signinmanager.IsSignedIn(User))
        {
            var usuario = await userManager.GetUserAsync(User); // Correctly fetch the user object
            if (usuario != null)
            {
                var elementoscompletados = await misionService.ObtenerQuestsPorJuegoyUsuario(idJuego, usuario.Id); // Use the correct instance of misionService

                if (elementoscompletados.Any(c => c.MisionId == elemento.Id))//si el id de elemento.id esta en elementoscompletados.MisionID
                {
                    misionvm.Completada = true;
                }
            }
        }

        return View("Mision", misionvm);
    }

    public async Task<IActionResult> Item(int id, int idJuego)
    {
        var elemento = await itemService.ObtenerItemPorIdAsync(id);
        var itemvm = new ItemViewModel
        {
            Id = elemento.Id,
            JuegoId = elemento.JuegoId,
            Nombre = elemento.Nombre,
            Descripcion = elemento.Descripcion,
            Imagen = elemento.Imagen,
            Bugs = elemento.Bugs,
            IdElem = elemento.IdElem,
            TipoItem = elemento.TipoItem,
            Completada = false
        };
        if (signinmanager.IsSignedIn(User))
        {
            var usuario = await userManager.GetUserAsync(User); // Correctly fetch the user object
            if (usuario != null)
            {
                var elementoscompletados = await itemService.ObtenerItemPorJuegoyUsuario(idJuego, usuario.Id); // Use the correct instance of misionService

                if (elementoscompletados.Any(c => c.Id == elemento.Id))//si el id de elemento.id esta en elementoscompletados.MisionID
                {
                    itemvm.Completada = true;
                }
            }
        }
        ViewBag.tipoelemento = "Item";
        return View("Item", itemvm);
    }

    public async Task<IActionResult> Trucos(int id)
    {
        var elemento = await trucoService.ObtenerTrucoPorIdAsync(id);
        ViewBag.tipoelemento = "Trucos";
        if (elemento is null)
        {
            return RedirectToAction("Mensaje", new { mensaje = "No se ha encontrado ningun truco" });
        }
        return View("Trucos", elemento);
    }

    [HttpPost]
    public async Task<IActionResult> CrearJuego(JuegoViewModel juegovm)
    {
        var juego = new Juego()
        {
            IdElem = juegovm.IdElem,
            Nombre = juegovm.Nombre,
            Descripcion = juegovm.Descripcion,
            Imagen = juegovm.Imagen,
            Bugs = juegovm.Bugs
        };

        if (await juegoService.crearjuego(juego))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha creado el juego" });
        }

        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar crear el juego" });

    }

    public async Task<IActionResult> CrearMision(MisionViewModel misionvm)
    {
        var mision = new Mision()
        {
            Id = misionvm.Id,
            IdElem = misionvm.IdElem,
            JuegoId = misionvm.idJuego,

            Nombre = misionvm.Nombre,
            Descripcion = misionvm.Descripcion,
            Imagen = misionvm.Imagen,
            StartTrigger = misionvm.StartTrigger,
            Bugs = misionvm.Bugs,
            TipoQuest = misionvm.TipoQuest
        };

        if (await misionService.crearmision(mision))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha creado la mision" });
        }

        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar crear la mision asegurese de que el Id del juego existe" });
    }

    public async Task<IActionResult> CrearItem(ItemViewModel itemvm)
    {
        var item = new Item() {
            Id = itemvm.Id,
            IdElem = itemvm.IdElem,
            JuegoId = itemvm.JuegoId,
            Nombre = itemvm.Nombre,
            Descripcion = itemvm.Descripcion,
            Imagen = itemvm.Imagen,
            Bugs = itemvm.Bugs,
            TipoItem = itemvm.TipoItem
        };
        if (await itemService.CrearItem(item))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha creado el item" });
        }
        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar crear el item" });
    }

    public async Task<IActionResult> eliminarelemento(string tipo, int idelemento, int idjuego)
    {
        var idJuego = int.Parse(HttpContext.Request.Form["idjuego"]);
        var idElemento = int.Parse(HttpContext.Request.Form["idelemento"]);
        var tipoElemento = HttpContext.Request.Form["tipo"];
        if (tipoElemento == "Mision")
        {
            if (await misionService.EliminarMision(idElemento, idjuego))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "No se ha podido eliminar la mision" });
            }
        }
        else if (tipoElemento == "Item")
        {
            if (await itemService.EliminarItem(idElemento, idjuego))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "No se ha podido eliminar el item" });
            }
        }
        else if (tipoElemento == "Juego")
        {
            if (await juegoService.EliminarJuego(idElemento))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "No se ha podido eliminar el juego" });
            }
        }
        else if (tipoElemento == "Truco")
        {
            if (await trucoService.EliminarTruco(idElemento, idjuego))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "No se ha podido eliminar el truco" });
            }
        }
        return RedirectToAction("Mensaje", new { mensaje = "Tipo de elemento no válido" });

    }

    [HttpPost]
    public async Task<IActionResult> CrearElemento(string tipo, string idJuego)
    {
        if (tipo == "Juego")
        {
            var elemento = new JuegoViewModel();
            return await Task.FromResult(View("CRUD/CrearJuego", elemento));
        }
        else if (tipo == "Mision")
        {
            var elemento = new MisionViewModel();
            elemento.idJuego = int.Parse(idJuego);
            return await Task.FromResult(View("CRUD/CrearMision", elemento));
        }
        else if (tipo == "Item")
        {
            var elemento = new ItemViewModel();
            elemento.JuegoId = int.Parse(idJuego);
            return await Task.FromResult(View("CRUD/CrearItem", elemento));
        }
        else if (tipo == "Truco")
        {
            var elemento = new Truco();
            elemento.JuegoId = int.Parse(idJuego);
            return await Task.FromResult(View("CRUD/CrearTruco", elemento));
        }
        else
        {
            return RedirectToAction("Mensaje", new { mensaje = "Tipo de elemento no válido" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ModificarElemento(string tipo, int idelemento)
    {
        try
        {


            if (tipo == "Juego")
            {
                var elemento = await juegoService.ObtenerJuegoPorIdAsync(idelemento);
                return await Task.FromResult(View("CRUD/ModificarJuego", elemento));
            }
            else if (tipo == "Mision")
            {
                var elemento = await misionService.ObtenerMisionesPorIdAsync(idelemento);
                return await Task.FromResult(View("CRUD/ModificarMision", elemento));
            }
            else if (tipo == "Item")
            {
                var elemento = await itemService.ObtenerItemPorIdAsync(idelemento);
                return await Task.FromResult(View("CRUD/ModificarItem", elemento));
            }
            else if (tipo == "Truco")
            {
                var elemento = await trucoService.ObtenerTrucoPorIdAsync(idelemento);
                return await Task.FromResult(View("CRUD/ModificarTruco", elemento));
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "Tipo de elemento no válido" });
            }
        }
        catch
        {
            return RedirectToAction("Mensaje", new { mensaje = "Error al intentar modificar el elemento" });
        }
    }


    [HttpPost]
    public async Task<IActionResult> ModificarJuego(JuegoViewModel juegovm)
    {
        var juego = new Juego()
        {
            Id = juegovm.Id,
            IdElem = juegovm.IdElem,
            Nombre = juegovm.Nombre,
            Descripcion = juegovm.Descripcion,
            Imagen = juegovm.Imagen,
            Bugs = juegovm.Bugs
        };
        if (await juegoService.ModificarJuego(juego))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha modificado el juego" });
        }
        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar modificar el juego" });
    }

    [HttpPost]
    public async Task<IActionResult> ModificarMision(MisionViewModel misionvm)
    {
        var mision = new Mision()
        {
            Id = misionvm.Id,
            JuegoId = misionvm.idJuego,
            IdElem = misionvm.IdElem,
            Nombre = misionvm.Nombre,
            Descripcion = misionvm.Descripcion,
            Imagen = misionvm.Imagen,
            StartTrigger = misionvm.StartTrigger,
            Bugs = misionvm.Bugs,
            TipoQuest = misionvm.TipoQuest

        };
        if (await misionService.ModificarMision(mision))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha modificado la mision" });
        }
        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar modificar la mision" });
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

}
