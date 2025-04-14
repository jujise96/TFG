using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFG.Models;

namespace TFG.Controllers
{
    public class GestionUsuarioController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly  SignInManager<Usuario> signInManager;

        public GestionUsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        public IActionResult InicioSesion()
        {
            return View();
        }
        
        public IActionResult AltaUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AltaUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                await userManager.CreateAsync(usuario, password : usuario.Contrasena);
                return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }
    }
}
