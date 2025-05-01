using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFG.Models;

namespace TFG.Controllers
{
    public class GestionUsuarioController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ILogger<GestionUsuarioController> logger;

        public GestionUsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ILogger<GestionUsuarioController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        private string ObtenerClaim(ClaimsPrincipal principal, string tipoClaim)
        {
            return principal.HasClaim(c => c.Type == tipoClaim) ? principal.FindFirstValue(tipoClaim) : null;
        }
        private string GenerarPassword(int longitudMaxima = 20)
        {
            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var resultado = new char[longitudMaxima];

            for (int i = 0; i < longitudMaxima; i++)
            {
                resultado[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(resultado);
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
                    var usuarioaux = userManager.FindByEmailAsync(usuario.mailusername);
                    if (usuarioaux.Result == null)
                    {
                        ModelState.AddModelError(string.Empty, "El correo electrónico no está registrado.");
                        return View(usuario);
                    }
                    else
                    {
                        usuario.mailusername = usuarioaux.Result.NombreUsuario;
                    }
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
        public async Task<IActionResult> AltaUsuario(RegistrarUsuarioViewModel VMusuario)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario() { NombreUsuario = VMusuario.NombreUsuario, Nombre = VMusuario.Nombre, Apellido = VMusuario.Apellido, Correo = VMusuario.Correo, Contrasena = VMusuario.Contrasena, Telefono = VMusuario.Telefono, Pais = VMusuario.Pais, F_Nacimiento = VMusuario.F_Nacimiento, GooglePlusCode = VMusuario.GooglePlusCode };
                var result = await userManager.CreateAsync(usuario, password: usuario.Contrasena);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuario, "USUARIO");
                    /*if (TempData["LoginProvider"] != null && TempData["ProviderKey"] != null)
                    {
                        var login = new UserLoginInfo(TempData["LoginProvider"].ToString(), TempData["ProviderKey"].ToString(), TempData["ProviderDisplayName"].ToString());

                        TempData.Remove("LoginProvider");
                        TempData.Remove("ProviderKey");
                        TempData.Remove("ProviderDisplayName");

                        var resultado = await userManager.AddLoginAsync(usuario, login);
                        if (resultado.Succeeded)
                        {
                            await signInManager.SignInAsync(usuario, isPersistent: false, login.LoginProvider);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return View(VMusuario);
                        }
                    }*/
                    await signInManager.SignInAsync(usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(VMusuario);
                }
            }
            return View(VMusuario);
        }

        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> ModificarUsuario(int id)
        {
            if (User.FindFirst("IdUsuario").Value == id.ToString())
            {

                var usuario = new Usuario();
                //comprobar que el usuario id sea el mismo que el de la cookie o sesion
                //despues buscar al usuario en funcion del id proporcionado
                //usuario = await userManager.GetUserAsync(ClaimsPrincipal.Current);
                usuario = await userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    return RedirectToAction("Error", "Shared");
                }

                var model = new ModificarUsuarioViewModel
                {
                    NombreUsuario = usuario.NombreUsuario,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    Pais = usuario.Pais,
                    F_Nacimiento = usuario.F_Nacimiento,
                    GooglePlusCode = usuario.GooglePlusCode
                };

                return View(model);
            }
            else
            {
                var mensaje = "Acceso denegado a la vista de modificación de usuario." + id;
                return RedirectToAction("ModificarUsuario", routeValues: new { mensaje });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModificarUsuario(ModificarUsuarioViewModel VMusuario)
        {
            if (!ModelState.IsValid)
            {
                return View(VMusuario);
            }

            var usuario = await userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return NotFound();
            }

            // Actualizamos los campos del usuario
            usuario.NombreUsuario = VMusuario.NombreUsuario;
            usuario.Nombre = VMusuario.Nombre;
            usuario.Apellido = VMusuario.Apellido;
            usuario.Correo = VMusuario.Correo;
            usuario.NombreUsuario = VMusuario.NombreUsuario; // Muy importante si se usa UserName para login
            usuario.Telefono = VMusuario.Telefono;
            usuario.Pais = VMusuario.Pais;
            usuario.F_Nacimiento = VMusuario.F_Nacimiento;
            usuario.GooglePlusCode = VMusuario.GooglePlusCode;

            // Si hay una nueva contraseña, la cambiamos
            // falla al cambiar la contraseña. TODO
            if (!string.IsNullOrWhiteSpace(VMusuario.Contrasena))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(usuario);
                var resultPassword = await userManager.ResetPasswordAsync(usuario, token, VMusuario.Contrasena);
                if (!resultPassword.Succeeded)
                {
                    foreach (var error in resultPassword.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(VMusuario);
                }
            }

            var result = await userManager.UpdateAsync(usuario);

            if (result.Succeeded)
            {

                logger.LogInformation("Se pudo modificar  usuario: " + VMusuario.NombreUsuario + " / " + VMusuario.Correo);
                TempData["Mensaje"] = "Datos actualizados correctamente.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogInformation("No se pudo modificar usuario: " + VMusuario.NombreUsuario + " / " + VMusuario.Correo);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(VMusuario);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            if (User.FindFirst("IdUsuario").Value == id.ToString())
            {
                var usuario = new Usuario();
                //comprobar que el usuario id sea el mismo que el de la cookie o sesion
                //despues buscar al usuario en funcion del id proporcionado
                //usuario = await userManager.GetUserAsync(ClaimsPrincipal.Current);
                usuario = await userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    var mensaje = "Usuario no encontrado." + id;
                    return RedirectToAction("ModificarUsuario", routeValues: new { mensaje });
                }
                // Eliminar el usuario
                var result = await userManager.DeleteAsync(usuario);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var mensaje = "Acceso denegado a la vista de modificación de usuario." + id;
                return RedirectToAction("ModificarUsuario", routeValues: new { mensaje });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)
        {
            var urlRedireccion = Url.Action("RecogerDatosUsuarioExterno", values: new { urlRetorno });
            var propiedades = signInManager
                .ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);

            if (proveedor == "Google")
                propiedades.Items["prompt"] = "select_account"; //Es necesario siempre elegir una cuenta

            return new ChallengeResult(proveedor, propiedades);
        }


        [AllowAnonymous]
        public async Task<IActionResult> RecogerDatosUsuarioExterno(string urlRetorno = null, string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");

            var mensaje = "";

            if (remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("InicioSesion", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                mensaje = "Error cargando la data de login externo";
                return RedirectToAction("InicioSesion", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            // Ya existe la cuenta 
            if (resultadoLoginExterno.Succeeded)
                return LocalRedirect(urlRetorno);

            //Recogemos los datos que queramos del login externo y preparamos el ViewModel para el register
            string email = ObtenerClaim(info.Principal, ClaimTypes.Email);

            if (email == null)
            {
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("InicioSesion", routeValues: new { mensaje });
            }

            var nombre = ObtenerClaim(info.Principal, ClaimTypes.GivenName);
            if (nombre == null)
            {
                mensaje = "Error leyendo el nombre del usuario del proveedor";
                return RedirectToAction("InicioSesion", routeValues: new { mensaje });
            }

            var apellidos = ObtenerClaim(info.Principal, ClaimTypes.Surname);
            /*if (apellidos == null)
            {
                mensaje = "Error leyendo los apellidos del usuario del proveedor";
                return RedirectToAction("InicioSesion", routeValues: new { mensaje });
            }*/
            var Contraseña = GenerarPassword();
            if (Contraseña == null)
            {
                mensaje = "Error Generando la contraseña";
                return RedirectToAction("InicioSesion", routeValues: new { mensaje });
            }

            var registroVM = new Usuario
            {
                NombreUsuario = email,//quitar el @ del email TODO
                Correo = email,
                Nombre = nombre,
                Apellido = apellidos,
                Contrasena = Contraseña
                //F_CreacionUsuario = DateTime.Now // No se puede obtener la fecha de nacimiento del proveedor
            };



            /*//Guardamos temporalmente estos datos para a futuro guardarlos en el usuarioLoginExterno
            TempData["LoginProvider"] = info.LoginProvider;
            TempData["ProviderKey"] = info.ProviderKey;
            TempData["ProviderDisplayName"] = info.ProviderDisplayName;*/

            var resultado = await userManager.CreateAsync(registroVM);
            if (resultado.Succeeded)
            {
                var resultadoLogin = await userManager.AddLoginAsync(registroVM, info);
                if (resultado.Succeeded)
                {
                    await signInManager.SignInAsync(registroVM, isPersistent: false, info.LoginProvider);
                    return LocalRedirect(urlRetorno);
                }
            }
            return View("AltaUsuario", registroVM);

        }
    }
}
