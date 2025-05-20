document.addEventListener('DOMContentLoaded', function () {

    // Función para obtener el AntiForgeryToken
    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        if (tokenInput) {
            return tokenInput.value;
        }
        console.error("No se encontró el AntiForgeryToken en el DOM.");
        return null;
    }

    // Función para mostrar mensajes de alerta
    function mostrarAlerta(elementoMensaje, mensaje, tipo, duracion = 3000) {
        elementoMensaje.innerHTML = `<div class="alert alert-${tipo}">${mensaje}</div>`;
        setTimeout(() => {
            elementoMensaje.innerHTML = '';
        }, duracion);
    }

    // Función principal para cargar comentarios
    async function cargarComentarios(tipoEntidad, entidadId, comentariosDiv) {
        comentariosDiv.innerHTML = '<p>Cargando comentarios...</p>';
        try {
            const response = await fetch(`/Comentario/MostrarComentarios?tipoEntidad=${tipoEntidad}&entidadId=${entidadId}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.text();
            comentariosDiv.innerHTML = data;
            // Después de cargar el HTML, actualizar el total de comentarios
            const totalComentariosSpan = document.getElementById(`total-comentarios-${tipoEntidad}-${entidadId}`);
            if (totalComentariosSpan) {
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = data;
                const comentarioCards = tempDiv.querySelectorAll('.comentario-individual[data-comentario-id]');
                totalComentariosSpan.textContent = comentarioCards.length;
            }
        } catch (error) {
            console.error(`Error al cargar los comentarios para ${tipoEntidad} ${entidadId}:`, error);
            comentariosDiv.innerHTML = '<p class="alert alert-danger">Error al cargar los comentarios.</p>';
        }
    }

    // Delegación de eventos para la sección de comentarios
    document.querySelectorAll('[id^="seccion-de-comentarios"]').forEach(comentariosWrapper => {
        const tipoEntidad = comentariosWrapper.dataset.tipoEntidad;
        const entidadId = comentariosWrapper.dataset.entidadId;

        if (!tipoEntidad || !entidadId) {
            console.error("Faltan atributos data-tipo-entidad o data-entidad-id en el div de comentarios.");
            return;
        }

        // Cargar los comentarios iniciales al cargar la página para cada sección
        cargarComentarios(tipoEntidad, entidadId, comentariosWrapper);

        comentariosWrapper.addEventListener('click', async function (event) {
            const target = event.target;

            // --- Botón Enviar Nuevo Comentario Principal ---
            if (target.classList.contains('btn-enviar-nuevo-comentario')) {
                const form = target.closest('.form-nuevo-comentario');
                const mensajeTextarea = form.querySelector('.mensaje-nuevo-comentario');
                const mensajeRespuestaDiv = form.querySelector('.mensaje-respuesta-comentario');

                const mensaje = mensajeTextarea.value.trim();
                const currentTipoEntidad = form.dataset.tipoEntidad;
                const currentEntidadId = form.dataset.entidadId;

                if (mensaje === "") {
                    mostrarAlerta(mensajeRespuestaDiv, 'Por favor, escribe un comentario.', 'warning');
                    return;
                }

                const antiForgeryToken = getAntiForgeryToken();
                if (!antiForgeryToken) {
                    mostrarAlerta(mensajeRespuestaDiv, 'Error de seguridad: No se pudo obtener el token de verificación.', 'danger');
                    return;
                }

                try {
                    const response = await fetch('/Comentario/CrearComentario', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': antiForgeryToken
                        },
                        body: `tipoEntidad=${currentTipoEntidad}&entidadId=${currentEntidadId}&mensaje=${encodeURIComponent(mensaje)}&comentarioPadreId=&juegoId=${currentTipoEntidad === 'Juego' ? currentEntidadId : ''}`
                    });

                    if (response.ok) {
                        mensajeTextarea.value = "";
                        mostrarAlerta(mensajeRespuestaDiv, 'Comentario enviado.', 'success');
                        await cargarComentarios(currentTipoEntidad, currentEntidadId, comentariosWrapper); // Recargar la sección completa
                    } else {
                        const errorText = await response.text();
                        console.error('Error al enviar el comentario:', response.status, response.statusText, errorText);
                        mostrarAlerta(mensajeRespuestaDiv, `Error al enviar el comentario: ${errorText || 'Inténtalo de nuevo.'}`, 'danger');
                    }
                } catch (error) {
                    console.error('Error de red o procesamiento al enviar comentario:', error);
                    mostrarAlerta(mensajeRespuestaDiv, 'Ocurrió un error inesperado al enviar el comentario.', 'danger');
                }
            }

            // --- Botón Responder ---
            if (target.classList.contains('btn-responder')) {
                const comentarioId = target.dataset.comentarioId;
                const respuestaForm = document.getElementById(`respuesta-form-${comentarioId}`);
                if (respuestaForm) {
                    respuestaForm.classList.toggle('d-none'); // Toggle para mostrar/ocultar
                    const textarea = respuestaForm.querySelector('.mensaje-respuesta-textarea');
                    if (!respuestaForm.classList.contains('d-none')) {
                        textarea.focus(); // Poner el foco en el textarea si se muestra
                    } else {
                        textarea.value = ''; // Limpiar si se oculta
                        respuestaForm.querySelector('.mensaje-respuesta-alerta').innerHTML = ''; // Limpiar alertas
                    }
                }
            }

            // --- Botón Cancelar Respuesta ---
            if (target.classList.contains('btn-cancelar-respuesta')) {
                const respuestaForm = target.closest('.form-respuesta');
                if (respuestaForm) {
                    respuestaForm.classList.add('d-none');
                    respuestaForm.querySelector('.mensaje-respuesta-textarea').value = '';
                    respuestaForm.querySelector('.mensaje-respuesta-alerta').innerHTML = '';
                }
            }

            // --- Botón Enviar Respuesta ---
            if (target.classList.contains('btn-enviar-respuesta')) {
                const padreId = target.dataset.comentarioPadreId;
                const respuestaForm = target.closest('.form-respuesta');
                const mensajeTextarea = respuestaForm.querySelector('.mensaje-respuesta-textarea');
                const mensajeRespuestaDiv = respuestaForm.querySelector('.mensaje-respuesta-alerta');
                const comentarioIndividual = target.closest('.comentario-individual'); // Para obtener el contexto

                const currentTipoEntidad = comentarioIndividual.dataset.tipoEntidad;
                const currentEntidadId = comentarioIndividual.dataset.entidadId;

                const mensaje = mensajeTextarea.value.trim();

                if (mensaje === "") {
                    mostrarAlerta(mensajeRespuestaDiv, 'Por favor, escribe tu respuesta.', 'warning');
                    return;
                }

                const antiForgeryToken = getAntiForgeryToken();
                if (!antiForgeryToken) {
                    mostrarAlerta(mensajeRespuestaDiv, 'Error de seguridad: No se pudo obtener el token de verificación.', 'danger');
                    return;
                }

                try {
                    const response = await fetch('/Comentario/CrearComentario', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': antiForgeryToken
                        },
                        body: `tipoEntidad=${currentTipoEntidad}&entidadId=${currentEntidadId}&mensaje=${encodeURIComponent(mensaje)}&comentarioPadreId=${padreId}&juegoId=${currentTipoEntidad === 'Juego' ? currentEntidadId : ''}`
                    });

                    if (response.ok) {
                        mensajeTextarea.value = "";
                        mostrarAlerta(mensajeRespuestaDiv, 'Respuesta enviada.', 'success');
                        // Recargar la sección completa para ver la nueva respuesta
                        await cargarComentarios(currentTipoEntidad, currentEntidadId, comentariosWrapper);
                    } else {
                        const errorText = await response.text();
                        console.error('Error al enviar la respuesta:', response.status, response.statusText, errorText);
                        mostrarAlerta(mensajeRespuestaDiv, `Error al enviar la respuesta: ${errorText || 'Inténtalo de nuevo.'}`, 'danger');
                    }
                } catch (error) {
                    console.error('Error de red o procesamiento al enviar respuesta:', error);
                    mostrarAlerta(mensajeRespuestaDiv, 'Ocurrió un error inesperado al enviar la respuesta.', 'danger');
                }
            }

            // --- Botón Eliminar Comentario ---
            if (target.classList.contains('btn-eliminar-comentario')) {
                const comentarioId = target.dataset.comentarioId;
                const comentarioIndividual = target.closest('.comentario-individual'); // Para obtener el contexto

                const currentTipoEntidad = comentarioIndividual.dataset.tipoEntidad;
                const currentEntidadId = comentarioIndividual.dataset.entidadId;


                if (confirm('¿Estás seguro de que quieres eliminar este comentario y todas sus respuestas?')) {
                    const antiForgeryToken = getAntiForgeryToken();
                    if (!antiForgeryToken) {
                        alert('Error de seguridad: No se pudo obtener el token de verificación.');
                        return;
                    }

                    try {
                        const response = await fetch('/Comentario/EliminarComentario', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded',
                                'RequestVerificationToken': antiForgeryToken
                            },
                            body: `comentarioId=${comentarioId}`
                        });

                        if (response.ok) {
                            alert('Comentario eliminado exitosamente.'); // Usar alert simple aquí para confirmación rápida
                            await cargarComentarios(currentTipoEntidad, currentEntidadId, comentariosWrapper); // Recargar
                        } else if (response.status === 403) {
                            alert('No tienes permiso para eliminar este comentario.');
                        } else if (response.status === 404) {
                            alert('El comentario que intentas eliminar no fue encontrado.');
                        } else {
                            const errorText = await response.text();
                            console.error('Error al eliminar el comentario:', response.status, response.statusText, errorText);
                            alert(`Error al eliminar el comentario: ${errorText || 'Inténtalo de nuevo.'}`);
                        }
                    } catch (error) {
                        console.error('Error de red o procesamiento al eliminar comentario:', error);
                        alert('Ocurrió un error inesperado. Por favor, inténtalo de nuevo.');
                    }
                }
            }

            // --- Lógica para mostrar/ocultar respuestas ---
            if (target.classList.contains('btn-mostrar-respuestas')) {
                const comentarioId = target.dataset.comentarioId;
                const respuestasContainer = document.getElementById(`respuestas-de-${comentarioId}`);
                if (respuestasContainer) {
                    respuestasContainer.classList.toggle('d-none');
                    const numRespuestas = respuestasContainer.querySelectorAll('li.media').length;
                    target.textContent = respuestasContainer.classList.contains('d-none')
                        ? `Ver ${numRespuestas} respuestas`
                        : `Ocultar respuestas`;
                }
            }
        });
    });
});