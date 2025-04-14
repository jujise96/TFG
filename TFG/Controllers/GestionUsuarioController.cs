using Microsoft.AspNetCore.Mvc;

namespace TFG.Controllers
{
    public class GestionUsuarioController : Controller
    {
        public IActionResult InicioSesion()
        {
            return View();
        }

        public IActionResult AltaUsuario()
        {
            return View();
        }
    }
}
