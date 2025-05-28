using System.Diagnostics;
using System.Security.Claims;
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

    private readonly IWebHostEnvironment _webHostEnvironment;


    public HomeController(ILogger<HomeController> logger, IJuegoService juegoService, IMisionService misionService, IItemService itemService, ITrucoService trucoService, SignInManager<Usuario> signinmanager, UserManager<Usuario> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        this.juegoService = juegoService;
        this.misionService = misionService;
        this.itemService = itemService;
        this.trucoService = trucoService;
        this.signinmanager = signinmanager;
        this.userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
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
        if (signinmanager.IsSignedIn(User))
        {
            var usuario = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Correctly fetch the user object
            if (usuario is not null)
            {
                foreach (var elem in elemento)
                {
                    var progreso = await juegoService.ProgresoJuego(elem.Id, usuario.Id);
                    elem.Progreso = (int)progreso;
                }
            }
        }
        ViewBag.tipoelemento = "Juego";
        return View(elemento);
    }

    public async Task<IActionResult> Juego(int id)
    {
        var elemento = await juegoService.ObtenerJuegoPorIdAsync(id);
        ViewBag.tipoelemento = "Juego";
        ViewBag.idJuego = id;
        if (signinmanager.IsSignedIn(User))
        {
            var usuario = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Correctly fetch the user object
            if (usuario is not null)
            {
                var progresoMisionDecimal = await misionService.ProgresoMision(elemento.Id, usuario.Id);
                var progresoItemDecimal = await itemService.ProgresoItem(elemento.Id, usuario.Id);

                TempData["progresomision"] = (int)progresoMisionDecimal;
                TempData["progresoitem"] = (int)progresoItemDecimal;
            }
        }
        else
        {
            TempData["progresomision"] = -1;
            TempData["progresoitem"] = -1;
        }


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
            elemento = new Truco()
            {
                Id = -1,
                IdElem = "",
                JuegoId = id,
                Nombre = "",
                Descripcion = "",
                Trucos = ""
            };
        }
        return View("Trucos", elemento);
    }

    [HttpPost]
    public async Task<IActionResult> CrearJuego(JuegoViewModel juegovm)
    {

        // Manejar la subida de la imagen
        if (juegovm.ImagenFile != null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            // Asegúrate de que la carpeta exista
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar un nombre único para el archivo para evitar colisiones
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + juegovm.ImagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await juegovm.ImagenFile.CopyToAsync(fileStream);
            }

            // Guardar la ruta relativa en la base de datos
            juegovm.Imagen = "/images/uploads/" + uniqueFileName;
        }



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
        if (misionvm.ImagenFile != null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            // Asegúrate de que la carpeta exista
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar un nombre único para el archivo para evitar colisiones
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + misionvm.ImagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await misionvm.ImagenFile.CopyToAsync(fileStream);
            }

            // Guardar la ruta relativa en la base de datos
            misionvm.Imagen = "/images/uploads/" + uniqueFileName;
        }

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
        // Manejar la subida de la imagen
        if (itemvm.ImagenFile != null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            // Asegúrate de que la carpeta exista
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar un nombre único para el archivo para evitar colisiones
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemvm.ImagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await itemvm.ImagenFile.CopyToAsync(fileStream);
            }

            // Guardar la ruta relativa en la base de datos
            itemvm.Imagen = "/images/uploads/" + uniqueFileName;
        }

        var item = new Item()
        {
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

    public async Task<IActionResult> CrearTruco(Truco truco)
    {
        if (await trucoService.Creartruco(truco))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha creado el truco" });
        }
        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar crear el truco" });
    }


    private bool EliminarImagenFisica(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return true; // No hay imagen o es nula, no hay nada que eliminar

        // Construye la ruta física completa de la imagen en wwwroot
        // El .TrimStart('/') es importante para manejar URLs que empiezan con '/'
        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));

        try
        {
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
                Console.WriteLine($"Imagen eliminada exitosamente: {imagePath}"); // Para depuración
                return true;
            }
            else
            {
                // La imagen no existe en la ruta física, esto puede ocurrir si la URL en DB
                // ya no corresponde a un archivo real, pero aun así consideramos éxito para la eliminación de DB.
                Console.WriteLine($"Advertencia: La imagen no se encontró en la ruta esperada: {imagePath}");
                return true;
            }
        }
        catch (Exception ex)
        {
            // Registra el error para depuración
            _logger.LogError(ex, $"Error al eliminar la imagen física '{imagePath}'.");
            // Podrías decidir si esto debería impedir la eliminación de la DB o no.
            // Por ahora, si falla la eliminación física, no la impedimos.
            return false; // Retorna false si hubo un error al eliminar físicamente
        }
    }

    public async Task<IActionResult> eliminarelemento(string tipo, int idelemento, int idjuego)
    {
        var idJuego = int.Parse(HttpContext.Request.Form["idjuego"]);
        var idElemento = int.Parse(HttpContext.Request.Form["idelemento"]);
        var tipoElemento = HttpContext.Request.Form["tipo"];
        if (tipoElemento == "Mision")
        {
            var mision = await misionService.ObtenerMisionesPorIdAsync(idElemento);
            var urldeMision = mision.Imagen;

            if (await misionService.EliminarMision(idElemento, idjuego))
            {
                if (urldeMision != null)
                {
                    System.IO.File.Delete("./wwwroot" + urldeMision);
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "No se ha podido eliminar la mision" });
            }
        }
        else if (tipoElemento == "Item")
        {
            var item = await itemService.ObtenerItemPorIdAsync(idElemento);
            var urldeItem = item.Imagen;
            if (await itemService.EliminarItem(idElemento, idjuego))
            {
                if (urldeItem != null)
                {
                    System.IO.File.Delete("./wwwroot" + urldeItem);
                }

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Mensaje", new { mensaje = "No se ha podido eliminar el item" });
            }
        }
        else if (tipoElemento == "Juego")
        {

            var todasLasImagenesHijas = await juegoService.ObtenerUrlsImagenesHijosDeJuego(idElemento);

            var juego = await juegoService.ObtenerJuegoPorIdAsync(idElemento);
            var urlJuego = juego.Imagen;
            if (await juegoService.EliminarJuego(idElemento))
            {
                foreach (var imageUrl in todasLasImagenesHijas)
                {
                    EliminarImagenFisica(imageUrl);
                }
                if (urlJuego != null)
                {
                    System.IO.File.Delete("./wwwroot" + urlJuego);
                }

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

    public async Task<IActionResult> ModificarJuego(JuegoViewModel juegovm)
    {
        // Manejar la subida de la nueva imagen
        if (juegovm.ImagenFile != null) // Si el usuario ha seleccionado un nuevo archivo de imagen
        {
            // Opcional: Eliminar la imagen antigua si existe en el servidor
            if (!string.IsNullOrEmpty(juegovm.Imagen))
            {
                // Construye la ruta física de la imagen antigua en wwwroot
                // Elimina el '/' inicial de la URL para Path.Combine
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, juegovm.Imagen.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Generar un nombre único para el nuevo archivo
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            // Asegúrate de que la carpeta exista, créala si no
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + juegovm.ImagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar el nuevo archivo en el servidor
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await juegovm.ImagenFile.CopyToAsync(fileStream);
            }

            // Actualizar la propiedad Imagen del modelo existente con la nueva URL
            juegovm.Imagen = "/images/uploads/" + uniqueFileName;
        }
        else // Si no se ha subido un nuevo archivo
        {
            // Mantener la imagen existente. La URL de la imagen actual se envía desde el campo hidden en la vista
            juegovm.Imagen = juegovm.Imagen;
        }


        // 4. Llamar al servicio para actualizar el juego en la base de datos
        var juego = new Juego()
        {
            Id = juegovm.Id,
            IdElem = juegovm.IdElem,
            Nombre = juegovm.Nombre,
            Descripcion = juegovm.Descripcion,
            Imagen = juegovm.Imagen,
            Bugs = juegovm.Bugs
        };
        if (await juegoService.ModificarJuego(juego)) // O el nombre del método de tu servicio para actualizar
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha modificado el juego" });
        }
        else
        {
            // Si el servicio devuelve false, hubo un error en la BD
            ModelState.AddModelError("", "Error al intentar modificar el juego en la base de datos.");
            return View(juegovm); // Vuelve a la vista con el ViewModel y el error
        }
    }

    [HttpPost]
    public async Task<IActionResult> ModificarMision(MisionViewModel misionvm)
    {
        // Manejar la subida de la nueva imagen
        if (misionvm.ImagenFile != null) // Si el usuario ha seleccionado un nuevo archivo de imagen
        {
            // Opcional: Eliminar la imagen antigua si existe en el servidor
            if (!string.IsNullOrEmpty(misionvm.Imagen))
            {
                // Construye la ruta física de la imagen antigua en wwwroot
                // Elimina el '/' inicial de la URL para Path.Combine
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, misionvm.Imagen.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Generar un nombre único para el nuevo archivo
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            // Asegúrate de que la carpeta exista, créala si no
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + misionvm.ImagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar el nuevo archivo en el servidor
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await misionvm.ImagenFile.CopyToAsync(fileStream);
            }

            // Actualizar la propiedad Imagen del modelo existente con la nueva URL
            misionvm.Imagen = "/images/uploads/" + uniqueFileName;
        }
        else // Si no se ha subido un nuevo archivo
        {
            // Mantener la imagen existente. La URL de la imagen actual se envía desde el campo hidden en la vista
            misionvm.Imagen = misionvm.Imagen;
        }

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

    [HttpPost]
    public async Task<IActionResult> ModificarItem(ItemViewModel itemvm)
    {
        // Manejar la subida de la nueva imagen
        if (itemvm.ImagenFile != null) // Si el usuario ha seleccionado un nuevo archivo de imagen
        {
            // Opcional: Eliminar la imagen antigua si existe en el servidor
            if (!string.IsNullOrEmpty(itemvm.Imagen))
            {
                // Construye la ruta física de la imagen antigua en wwwroot
                // Elimina el '/' inicial de la URL para Path.Combine
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, itemvm.Imagen.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Generar un nombre único para el nuevo archivo
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            // Asegúrate de que la carpeta exista, créala si no
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemvm.ImagenFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar el nuevo archivo en el servidor
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await itemvm.ImagenFile.CopyToAsync(fileStream);
            }

            // Actualizar la propiedad Imagen del modelo existente con la nueva URL
            itemvm.Imagen = "/images/uploads/" + uniqueFileName;
        }
        else // Si no se ha subido un nuevo archivo
        {
            // Mantener la imagen existente. La URL de la imagen actual se envía desde el campo hidden en la vista
            itemvm.Imagen = itemvm.Imagen;
        }


        var item = new Item()
        {
            Id = itemvm.Id,
            JuegoId = itemvm.JuegoId,
            IdElem = itemvm.IdElem,
            Nombre = itemvm.Nombre,
            Descripcion = itemvm.Descripcion,
            Imagen = itemvm.Imagen,
            Bugs = itemvm.Bugs,
            TipoItem = itemvm.TipoItem
        };
        if (await itemService.ModificarItem(item))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha modificado el item" });
        }
        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar modificar el item" });
    }

    [HttpPost]
    public async Task<IActionResult> ModificarTruco(Truco truco)
    {
        if (await trucoService.ModificarTruco(truco))
        {
            return RedirectToAction("Mensaje", new { mensaje = "Se ha modificado el truco" });
        }
        return RedirectToAction("Mensaje", new { mensaje = "Error al intentar modificar el truco" });
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
