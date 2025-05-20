// Services/ModerationService.cs
using Microsoft.Extensions.Configuration; // Para leer la API Key desde la configuración
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Para logging
using System;
using GenerativeAI; // Para Exception, InvalidOperationException

namespace TFG.Services
{
    // Define la interfaz del servicio de moderación.
    // Esto es útil para la inyección de dependencias y la facilidad de testing.
    public interface IModeracionService
    {
        Task<ModeracionResultado> ModerarComentario(string mensaje);
    }

    // Clase que representa el resultado de la moderación.
    public class ModeracionResultado
    {
        public bool EsApropiado { get; set; }
        public string MensajeError { get; set; } = string.Empty;
        // Opcional: Podrías añadir más detalles si la API de IA los proporciona,
        // como categorías de toxicidad (ej. "TOXICITY", "FLIRTATION").
        // public List<string> CategoriasInapropiadas { get; set; }
    }

    // Implementación del servicio de moderación que interactúa con la API de Google Gemini.
    public class ModeracionService : IModeracionService
    {
        private readonly GenerativeModel _model; // Objeto para interactuar con el modelo Gemini
        private readonly ILogger<ModeracionService> _logger; // Para registrar eventos y errores

        // Constructor del servicio.
        // Recibe IConfiguration para acceder a la API Key y ILogger para logging.
        public ModeracionService(IConfiguration configuration, ILogger<ModeracionService> logger)
        {
            _logger = logger;
            // Lee la API Key del archivo appsettings.json (o variables de entorno, etc.)
            var apiKey = configuration["GoogleAIApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("La clave de API de Google AI ('GoogleAIApiKey') no está configurada en appsettings.json. La moderación de IA no funcionará.");
                // Es crucial lanzar una excepción o manejar este caso para evitar que la aplicación continúe sin una clave válida.
                throw new InvalidOperationException("La clave de API de Google AI no está configurada.");
            }

            // Inicializa el modelo Gemini.
            // Puedes elegir entre "gemini-pro" (más robusto para tareas complejas)
            // o "gemini-1.5-flash" (más rápido y económico para tareas de clasificación como esta).
            _model = new GenerativeModel(apiKey: apiKey, model: "gemini-1.5-flash");
        }

        // Método principal para moderar un comentario.
        public async Task<ModeracionResultado> ModerarComentario(string mensaje)
        {
            // Validar que el mensaje no esté vacío
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return new ModeracionResultado { EsApropiado = false, MensajeError = "El comentario no puede estar vacío." };
            }

            try
            {
                // Este es el 'prompt' que se le envía a Gemini.
                // Es crucial ser muy claro y específico sobre lo que esperas como respuesta.
                // Le pedimos que responda solo con "APROPIADO" o "INAPROPIADO" para facilitar el parsing.
                var prompt = $@"Evalúa el siguiente comentario de usuario para determinar si contiene contenido inapropiado.
Considera lo siguiente como inapropiado:
- Lenguaje ofensivo, vulgar o soez (incluyendo 'leet speak' o variaciones como 'g1l1po11as', 'm a m a h u e v o s', 'sexo duro', 'mongolo').
- Incitación al odio, discriminación o violencia.
- Acoso, amenazas o intimidación.
- Contenido sexualmente explícito o sugerente.
- Spam o promociones no solicitadas.
- Información personal (números de teléfono, direcciones, correos electrónicos, nombres completos no relevantes).
- Cualquier otro contenido que sea perjudicial, ilegal o contrario a las normas de una comunidad de juegos.

Comentario a evaluar: ""{mensaje}""

Responde estrictamente con la palabra 'APROPIADO' si el comentario es completamente seguro y aceptable.
Si detectas CUALQUIER FORMA de contenido problemático, responde estrictamente con la palabra 'INAPROPIADO'.
No incluyas ninguna explicación, justificación o texto adicional. Solo la palabra 'APROPIADO' o 'INAPROPIADO'.

Ejemplos adicionales para un mejor entendimiento:
Comentario: ""Qué buen día para jugar!""
Respuesta: APROPIADO

Comentario: ""Ese juego es una mierda, los devs son unos estúpidos.""
Respuesta: INAPROPIADO

Comentario: ""Mi correo es hola@ejemplo.com""
Respuesta: INAPROPIADO

Comentario: ""La nueva actualización es una basura, ojalá la arreglen.""
Respuesta: INAPROPIADO

Comentario: ""Te voy a romper la cara por lo que dijiste.""
Respuesta: INAPROPIADO
";

                // Envía la solicitud a la API de Gemini.
                var response = await _model.GenerateContentAsync(prompt);

                // Extrae el texto de la respuesta.
                // Se accede a Candidates[0] porque generalmente solo hay una respuesta.
                // Content.Parts[0].Text obtiene el texto de la primera parte del contenido.
                var textResult = response.Candidates[0].Content.Parts[0].Text.Trim().ToUpperInvariant();

                // Registra la respuesta de Gemini para depuración (aparecerá en la consola del servidor).
                _logger.LogInformation($"Moderación de Gemini para el mensaje '{mensaje}': '{textResult}'");

                // Evalúa el resultado de Gemini.
                if (textResult.Contains("INAPROPIADO"))
                {
                    return new ModeracionResultado { EsApropiado = false, MensajeError = "El comentario contiene contenido inapropiado y no puede ser publicado." };
                }
                else if (textResult.Contains("APROPIADO"))
                {
                    return new ModeracionResultado { EsApropiado = true };
                }
                else
                {
                    // Si Gemini no devuelve 'APROPIADO' o 'INAPROPIADO' (lo cual es raro si el prompt es bueno),
                    // por seguridad, consideramos el comentario como inapropiado y registramos una advertencia.
                    _logger.LogWarning($"La respuesta de Gemini no fue clara para el mensaje '{mensaje}': '{textResult}'. Bloqueando por defecto.");
                    return new ModeracionResultado { EsApropiado = false, MensajeError = "No se pudo verificar el contenido del comentario. Inténtalo de nuevo." };
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción que ocurra durante la comunicación con la API de Gemini.
                _logger.LogError(ex, "Error al comunicarse con la API de Google Gemini para moderación del comentario: '{Mensaje}'.", mensaje);
                // En caso de error (ej. problemas de red, cuota excedida, clave API inválida),
                // es una buena práctica de seguridad bloquear el comentario por defecto
                // para evitar que contenido potencialmente inapropiado pase sin revisión.
                return new ModeracionResultado { EsApropiado = false, MensajeError = "Problema con el sistema de moderación de contenido. Inténtalo de nuevo." };
            }
        }
    }
}