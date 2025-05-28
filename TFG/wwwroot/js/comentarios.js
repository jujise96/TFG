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

    // Función para actualizar los conteos de likes/dislikes en el DOM
    function updateLikeDislikeButtons(comentarioElement, newLikes, newDislikes, userReaction) {
        const likeBtn = comentarioElement.querySelector('.btn-like-comentario');
        const dislikeBtn = comentarioElement.querySelector('.btn-dislike-comentario');
        const likesCountSpan = comentarioElement.querySelector('.likes-count');
        const dislikesCountSpan = comentarioElement.querySelector('.dislikes-count');

        // Actualizar conteos
        if (likesCountSpan) likesCountSpan.textContent = newLikes;
        if (dislikesCountSpan) dislikesCountSpan.textContent = newDislikes;

        // Actualizar clases de los botones
        if (likeBtn) {
            if (userReaction === 1) { // Usuario dio like
                likeBtn.classList.remove('btn-outline-success');
                likeBtn.classList.add('btn-success');
            } else { // Usuario no dio like o dio dislike
                likeBtn.classList.remove('btn-success');
                likeBtn.classList.add('btn-outline-success');
            }
            likeBtn.dataset.currentUserReaction = userReaction !== null ? userReaction.toString() : "null";
        }

        if (dislikeBtn) {
            if (userReaction === 0) { // Usuario dio dislike
                dislikeBtn.classList.remove('btn-outline-danger');
                dislikeBtn.classList.add('btn-danger');
            } else { // Usuario no dio dislike o dio like
                dislikeBtn.classList.remove('btn-danger');
                dislikeBtn.classList.add('btn-outline-danger');
            }
            dislikeBtn.dataset.currentUserReaction = userReaction !== null ? userReaction.toString() : "null";
        }
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
                const parentContainer = target.closest('[data-entidad-juegoid]');

                const mensaje = mensajeTextarea.value.trim();
                const currentTipoEntidad = form.dataset.tipoEntidad;
                const currentEntidadId = form.dataset.entidadId;

                let juegoIdEnviar = '';
                if (currentTipoEntidad === 'Juego') {
                    juegoIdEnviar = currentEntidadId;
                } else {
                    if (comentariosWrapper && comentariosWrapper.dataset.entidadJuegoid) {
                        juegoIdEnviar = comentariosWrapper.dataset.entidadJuegoid;
                    } else {
                        console.warn(`No se encontró data-entidad-JuegoId para la entidad ${currentTipoEntidad} con ID ${currentEntidadId}.`);
                    }
                }

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
                        body: `tipoEntidad=${currentTipoEntidad}&entidadId=${currentEntidadId}&mensaje=${encodeURIComponent(mensaje)}&comentarioPadreId=&juegoId=${juegoIdEnviar}`
                    });

                    if (response.ok) {
                        mensajeTextarea.value = "";
                        mostrarAlerta(mensajeRespuestaDiv, 'Comentario enviado.', 'success');
                        await cargarComentarios(currentTipoEntidad, currentEntidadId, comentariosWrapper);
                    } else {
                        const errorText = await response.text();
                        console.error('Error al enviar el comentario:', response.status, response.statusText, errorText);
                        if (response.status === 400) {
                            mostrarAlerta(mensajeRespuestaDiv, `Error: ${errorText}`, 'danger');
                        } else {
                            mostrarAlerta(mensajeRespuestaDiv, `Error al enviar el comentario: ${errorText || 'Inténtalo de nuevo.'}`, 'danger');
                        }
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
                    respuestaForm.classList.toggle('d-none');
                    const textarea = respuestaForm.querySelector('.mensaje-respuesta-textarea');
                    if (!respuestaForm.classList.contains('d-none')) {
                        textarea.focus();
                    } else {
                        textarea.value = '';
                        respuestaForm.querySelector('.mensaje-respuesta-alerta').innerHTML = '';
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
                const comentarioIndividual = target.closest('.comentario-individual');

                const currentTipoEntidad = comentarioIndividual.dataset.tipoEntidad;
                const currentEntidadId = comentarioIndividual.dataset.entidadId;

                const mensaje = mensajeTextarea.value.trim();

                let juegoIdEnviar = '';
                if (currentTipoEntidad === 'Juego') {
                    juegoIdEnviar = currentEntidadId;
                } else {
                    if (comentariosWrapper && comentariosWrapper.dataset.entidadJuegoid) {
                        juegoIdEnviar = comentariosWrapper.dataset.entidadJuegoid;
                    } else {
                        console.warn(`No se encontró data-entidad-JuegoId para la entidad ${currentTipoEntidad} con ID ${currentEntidadId}.`);
                    }
                }

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
                        body: `tipoEntidad=${currentTipoEntidad}&entidadId=${currentEntidadId}&mensaje=${encodeURIComponent(mensaje)}&comentarioPadreId=${padreId}&juegoId=${juegoIdEnviar}`
                    });

                    if (response.ok) {
                        mensajeTextarea.value = "";
                        mostrarAlerta(mensajeRespuestaDiv, 'Respuesta enviada.', 'success');
                        await cargarComentarios(currentTipoEntidad, currentEntidadId, comentariosWrapper);
                    } else {
                        const errorText = await response.text();
                        console.error('Error al enviar la respuesta:', response.status, response.statusText, errorText);
                        if (response.status === 400) {
                            mostrarAlerta(mensajeRespuestaDiv, `Error: ${errorText}`, 'danger');
                        } else {
                            mostrarAlerta(mensajeRespuestaDiv, `Error al enviar la respuesta: ${errorText || 'Inténtalo de nuevo.'}`, 'danger');
                        }
                    }
                } catch (error) {
                    console.error('Error de red o procesamiento al enviar respuesta:', error);
                    mostrarAlerta(mensajeRespuestaDiv, 'Ocurrió un error inesperado al enviar la respuesta.', 'danger');
                }
            }

            // --- Botón Eliminar Comentario ---
            if (target.classList.contains('btn-eliminar-comentario')) {
                const comentarioId = target.dataset.comentarioId;
                const comentarioIndividual = target.closest('.comentario-individual');

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
                            alert('Comentario eliminado exitosamente.');
                            await cargarComentarios(currentTipoEntidad, currentEntidadId, comentariosWrapper);
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

            // ---  Botones de Like/Dislike ---
            if (target.classList.contains('btn-like-comentario') || target.classList.contains('btn-dislike-comentario')) {
                const button = target.closest('button');
                const comentarioId = button.dataset.comentarioId;
                const isLikeButton = button.dataset.like === 'true'; // True si es el botón de like, false si es el de dislike

                // Obtener la reacción actual del usuario (del atributo data- en el botón)
                const currentUserReaction = button.dataset.currentUserReaction; // "null", "1", o "0"

                let actionType; // "like", "dislike", "remove_like", "remove_dislike"
                let sendLikeValue; // el valor 'like' que se enviará al controlador (true/false)

                if (isLikeButton) { // Si se hizo clic en el botón de LIKE
                    if (currentUserReaction === "1") { // Si el usuario ya dio like, ahora lo quiere quitar
                        actionType = "remove_like";
                        sendLikeValue = false; // O cualquier valor que tu backend interprete como "quitar reacción"
                    } else { // El usuario no dio like (o dio dislike), ahora quiere dar like
                        actionType = "like";
                        sendLikeValue = true;
                    }
                } else { // Si se hizo clic en el botón de DISLIKE
                    if (currentUserReaction === "0") { // Si el usuario ya dio dislike, ahora lo quiere quitar
                        actionType = "remove_dislike";
                        sendLikeValue = true; // O cualquier valor que tu backend interprete como "quitar reacción"
                    } else { // El usuario no dio dislike (o dio like), ahora quiere dar dislike
                        actionType = "dislike";
                        sendLikeValue = false;
                    }
                }

                const antiForgeryToken = getAntiForgeryToken();
                if (!antiForgeryToken) {
                    alert('Error de seguridad: No se pudo obtener el token de verificación.');
                    return;
                }

                try {
                    const response = await fetch('/Comentario/comentariolike', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': antiForgeryToken
                        },
                        // Envía idcomentario y el booleano 'like' al controlador
                        body: `idcomentario=${comentarioId}&like=${sendLikeValue}`
                    });

                    if (response.ok) {
                        // Asumiendo que el controlador devuelve un JSON con los nuevos conteos y la reacción del usuario
                        // Ejemplo: { success: true, likes: 10, dislikes: 2, userReaction: 1 } (1 para like, 0 para dislike, null para ninguna)
                        const data = await response.json(); // Parsea la respuesta JSON

                        // Actualiza los botones y conteos en el frontend
                        const comentarioElement = button.closest('.comentario-individual');
                        updateLikeDislikeButtons(comentarioElement, data.likes, data.dislikes, data.userReaction);

                    } else if (response.status === 401) {
                        alert('Debes iniciar sesión para reaccionar a un comentario.');
                    } else {
                        const errorText = await response.text();
                        console.error('Error al enviar like/dislike:', response.status, response.statusText, errorText);
                        alert(`Error al reaccionar al comentario: ${errorText || 'Inténtalo de nuevo.'}`);
                    }
                } catch (error) {
                    console.error('Error de red o procesamiento al enviar like/dislike:', error);
                    alert('Ocurrió un error inesperado al reaccionar al comentario.');
                }
            }
        });
    });
});