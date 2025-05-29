using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TFG.Models;
using TFG.Repositories;
using TFG.Services;

namespace TFG.Controllers
{
    public class GestionUsuarioController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ILogger<GestionUsuarioController> logger;
        private readonly IRepositorioLoginExterno repositorioLoginExterno;
        private readonly IMailService mailService;
        private readonly IMisionService misionService;
        private readonly IItemService itemService;

        public GestionUsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ILogger<GestionUsuarioController> logger, IMailService mailService, IMisionService misionService, IItemService itemService, IRepositorioLoginExterno repositorioLoginExterno)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.mailService = mailService;
            this.misionService = misionService;
            this.itemService = itemService;
            this.repositorioLoginExterno = repositorioLoginExterno;
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
                var result = await signInManager.PasswordSignInAsync(usuario.mailusername, usuario.Contrasena, isPersistent: true, lockoutOnFailure: false);
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
                var usuario = new Usuario() { NombreUsuario = VMusuario.NombreUsuario, Nombre = VMusuario.Nombre, Apellido = VMusuario.Apellido, Correo = VMusuario.Correo, Contrasena = VMusuario.Contrasena, Telefono = VMusuario.Telefono, Pais = VMusuario.Pais, F_Nacimiento = VMusuario.F_Nacimiento, GooglePlusCode = VMusuario.GooglePlusCode, PerfilPic = VMusuario.PerfilPic };
                var result = await userManager.CreateAsync(usuario, password: usuario.Contrasena);
                if (result.Succeeded)
                {                    
                    if (usuario.Correo == "jujise96@gmail.com")
                    {
                        await userManager.AddToRoleAsync(usuario, "Admin");
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(usuario, "Usuario");
                    }
                        await signInManager.SignInAsync(usuario, isPersistent: true);
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
                    GooglePlusCode = usuario.GooglePlusCode,
                    loginexternos = (ICollection<LoginExterno>)await repositorioLoginExterno.ListadoLogins(usuario.Id),
                    PerfilPic = usuario.PerfilPic
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
            usuario.PerfilPic = VMusuario.PerfilPic;

            var result = await userManager.UpdateAsync(usuario);

            if (result.Succeeded)
            {

                logger.LogInformation("Se pudo modificar  usuario: " + VMusuario.NombreUsuario + " / " + VMusuario.Correo);
                TempData["Mensaje"] = "Datos actualizados correctamente.";
                await signInManager.SignInAsync(usuario, isPersistent: true);
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

            //comprobamos si el correo ya esta registrado
            var usuarioExistente = await userManager.FindByEmailAsync(registroVM.Correo);
            if (usuarioExistente != null)
            {
                registroVM.Id = usuarioExistente.Id;
                var resultadoinsercionlogin = await userManager.AddLoginAsync(usuarioExistente, info);
                if(resultadoinsercionlogin.Succeeded)
                {
                    await signInManager.SignInAsync(usuarioExistente, isPersistent: true, info.LoginProvider);
                    return LocalRedirect(urlRetorno);
                }
                else
                {
                    mensaje = "Error al añadir el login externo al usuario existente";
                    return RedirectToAction("InicioSesion", routeValues: new { mensaje });
                }
            }


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

        public IActionResult PassForgoten(string info = "")
        {
            ViewBag.Mensaje = info;
            if (User.Identity.IsAuthenticated)
            {
                var passforgotenviewmodel = new PassForgotenViewModel();
                passforgotenviewmodel.Correo = User.FindFirst(ClaimTypes.Email)?.Value;
                return View(passforgotenviewmodel);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PassForgoten(PassForgotenViewModel passforgotenvm)
        {
            var mensaje = "De existir una cuenta con el correo electronico: " + passforgotenvm.Correo + " se ha enviado un correo con instrucciones para recuperar su contraseña";
            ViewBag.Mensaje = mensaje;
            ModelState.Clear();
            var usuario = await userManager.FindByEmailAsync(passforgotenvm.Correo);
            if (usuario is not null)
            {
                const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var bytes = new byte[6];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(bytes);

                var resultado = new StringBuilder(6);
                foreach (var b in bytes)
                {
                    resultado.Append(caracteres[b % caracteres.Length]);
                }
                var codigo = resultado.ToString();
                await mailService.SendEmailAsync(usuario.Correo, usuario.NombreUsuario, codigo);
                return RedirectToAction("RecuperarContrasena", "GestionUsuario", routeValues: new { codigo });
            }
            return View();
        }

        public IActionResult RecuperarContrasena(string codigo = null)
        {
            if (codigo is null)
            {
                var mensaje = "Ha habido un problema al generar su codigo";
                return RedirectToAction("Mensaje", "Home", routeValues: new { mensaje });
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var modelo = new RecuperarContraseñaViewModel();
            modelo.Correo = email;
            modelo.Codigo = codigo;
            return View(modelo);
            //tempdata["nombredevariable"] codigo todo
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContrasena(RecuperarContraseñaViewModel recuperarContraseñaVM)
        {
            var usuario = await userManager.FindByEmailAsync(recuperarContraseñaVM.Correo);
            var mensaje = "";
            if (usuario is null)
            {
                mensaje = "Ha habido un problema al localizar su usuario";
                return RedirectToAction("Mensaje", "Home", routeValues: new { mensaje });
            }

            if (recuperarContraseñaVM.Codigo != recuperarContraseñaVM.IntentoCodigo)
            {
                mensaje = "El codigo de verificación no coincide con el indicado";
                return RedirectToAction("Mensaje", "Home", routeValues: new { mensaje });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultado = await userManager.ResetPasswordAsync(usuario, token, recuperarContraseñaVM.Contrasena);
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(recuperarContraseñaVM);
            }

            mensaje = "Ha habido un problema al actualizar su contraseña";
            if (resultado.Succeeded)
            {
                mensaje = "La clave se ha cambiado con exito";
            }
            return RedirectToAction("Mensaje", "Home", routeValues: new { mensaje });
        }

        [HttpPost]
        public async Task<IActionResult> MarcarCompletadaConModelo([FromBody] ItemCheckRequest modelo)
        {
            var idusuario = int.Parse(User.FindFirst("IdUsuario").Value);
            if (modelo.tipo == "Mision")
            {
                // Aquí puedes implementar la lógica para marcar la misión como completada
                // Por ejemplo, actualizar el estado de la misión en la base de datos
                if (modelo.completada)
                {
                    await misionService.CompletarMision(modelo.Id, idusuario);
                }
                else
                {
                    await misionService.DescompletarMision(modelo.Id, idusuario);
                }

            }
            else if (modelo.tipo == "Item")
            {
                // Aquí puedes implementar la lógica para marcar el item como completado
                // Por ejemplo, actualizar el estado del item en la base de datos
                if (modelo.completada)
                {
                    await itemService.CompletarItem(modelo.Id, idusuario);
                }
                else
                {
                    await itemService.DescompletarItem(modelo.Id, idusuario);
                }
            }
            else
            {
                return BadRequest("Tipo no válido");
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarcarCompletada(int juegoId, int misionId, bool check)
        {
            var idusuario = int.Parse(User.FindFirst("IdUsuario").Value);
            if (check)
            {
                await misionService.DescompletarMision(misionId, idusuario);
                check = false;                
            }
            else
            {
                await misionService.CompletarMision(misionId, idusuario);
                check = true;                
            }
            return RedirectToAction("Mision", "Home", new { id = misionId, idJuego = juegoId });
            //return RedirectToAction("Misiones", "Home", new { id = juegoId });
        }


        [HttpPost]
        public async Task<IActionResult> MarcarItemCompletado(int juegoId, int itemId, bool check)
        {
            var idusuario = int.Parse(User.FindFirst("IdUsuario").Value);
            if (check)
            {
                await itemService.DescompletarItem(itemId, idusuario);
                check = false;
            }
            else
            {
                await itemService.CompletarItem(itemId, idusuario);
                check = true;
            }
            return RedirectToAction("Item", "Home", new { id = itemId, idJuego = juegoId });
            //return RedirectToAction("Misiones", "Home", new { id = juegoId });
        }


    }

}
