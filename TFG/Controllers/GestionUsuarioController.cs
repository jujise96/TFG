using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> InicioSesion(IniciarSesionViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.mailusername.Contains("@"))
                {
                    usuario.mailusername = userManager.FindByEmailAsync(usuario.mailusername).Result.NombreUsuario;
                }
                var result = await signInManager.PasswordSignInAsync(usuario.mailusername, usuario.Contrasena, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                    return View(usuario);
                }
                
            }
            return View(usuario);
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
                var result = await userManager.CreateAsync(usuario, password : usuario.Contrasena);
                if (result.Succeeded) {
                    await signInManager.SignInAsync(usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(usuario);
                }
            }
                return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
