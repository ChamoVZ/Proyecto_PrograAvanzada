(function () {
    "use strict";

    var formulario = document.getElementById("historial-filtros");
    var contenido = document.getElementById("historial-contenido");
    var tabla = document.getElementById("historial-tabla");
    var paginacion = document.getElementById("historial-paginacion");
    var mensaje = document.getElementById("historial-mensaje");
    var limpiar = document.getElementById("historial-limpiar");
    var solicitudActual;

    if (!formulario || !contenido || !tabla || !paginacion || !window.fetch) {
        return;
    }

    var selectorInicial = paginacion.querySelector("select[name='tamanoHistorial']");
    if (selectorInicial) {
        selectorInicial.removeAttribute("onchange");
    }

    function crearElemento(etiqueta, clases, texto) {
        var elemento = document.createElement(etiqueta);
        if (clases) {
            elemento.className = clases;
        }
        if (texto !== undefined) {
            elemento.textContent = texto;
        }
        return elemento;
    }

    function agregarCelda(fila, texto, clases) {
        var celda = crearElemento("td", clases, texto);
        fila.appendChild(celda);
        return celda;
    }

    function dibujarTabla(elementos) {
        tabla.replaceChildren();

        if (!elementos.length) {
            tabla.appendChild(crearElemento(
                "p",
                "text-muted mb-0 text-center py-4",
                "No hay partidas que coincidan con los filtros."));
            return;
        }

        var responsive = crearElemento("div", "table-responsive");
        var elementoTabla = crearElemento("table", "table align-middle mb-0");
        var encabezado = document.createElement("thead");
        var filaEncabezado = document.createElement("tr");
        var columnas = ["Fecha", "Reto", "Modo", "Resultado", "Tiempo (s)", "XP"];

        columnas.forEach(function (columna, indice) {
            var th = crearElemento("th", indice > 3 ? "text-end" : "", columna);
            th.scope = "col";
            filaEncabezado.appendChild(th);
        });

        encabezado.appendChild(filaEncabezado);
        elementoTabla.appendChild(encabezado);

        var cuerpo = document.createElement("tbody");
        elementos.forEach(function (partida) {
            var fila = document.createElement("tr");
            agregarCelda(fila, partida.fechaJuego);
            agregarCelda(fila, partida.TituloReto);
            agregarCelda(fila, partida.Modo);

            var resultado = agregarCelda(fila);
            resultado.appendChild(crearElemento(
                "span",
                "badge rounded-pill " + (partida.Acertado ? "bg-success" : "bg-secondary"),
                partida.Acertado ? "Acertado" : "Fallado"));

            agregarCelda(fila, partida.TiempoEmpleadoSegundos, "text-end");
            agregarCelda(fila, partida.XpGanado, "text-end");
            cuerpo.appendChild(fila);
        });

        elementoTabla.appendChild(cuerpo);
        responsive.appendChild(elementoTabla);
        tabla.appendChild(responsive);
    }

    function crearEnlacePagina(texto, pagina, deshabilitado, activo) {
        var item = crearElemento("li", "page-item");
        if (deshabilitado) {
            item.classList.add("disabled");
        }
        if (activo) {
            item.classList.add("active");
        }

        var enlace = crearElemento("a", "page-link", texto);
        if (!deshabilitado) {
            var url = new URL(window.location.href);
            url.searchParams.set("paginaHistorial", pagina);
            enlace.href = url.pathname + url.search + url.hash;
            enlace.dataset.pagina = pagina;
        }

        item.appendChild(enlace);
        return item;
    }

    function dibujarPaginacion(datos) {
        paginacion.replaceChildren();

        if (datos.totalRegistros === 0) {
            return;
        }

        var controles = crearElemento(
            "div",
            "mx-pagination-toolbar d-flex flex-wrap justify-content-between align-items-center gap-3 mt-4");
        var grupoTamano = crearElemento("div", "d-flex align-items-center gap-2");
        var etiqueta = crearElemento("label", "form-label mb-0 text-nowrap", "Por página");
        var selector = crearElemento("select", "form-select form-select-sm");
        selector.name = "tamanoHistorial";
        selector.setAttribute("aria-label", "Registros por página");
        etiqueta.htmlFor = "tamanoHistorialAjax";
        selector.id = "tamanoHistorialAjax";

        [10, 20].forEach(function (tamano) {
            var opcion = crearElemento("option", "", tamano);
            opcion.value = tamano;
            opcion.selected = tamano === datos.tamanoPagina;
            selector.appendChild(opcion);
        });

        grupoTamano.appendChild(etiqueta);
        grupoTamano.appendChild(selector);
        controles.appendChild(grupoTamano);

        if (datos.totalPaginas > 1) {
            var nav = document.createElement("nav");
            nav.setAttribute("aria-label", "Navegación del historial");
            var lista = crearElemento("ul", "pagination pagination-sm mb-0");
            var inicial = Math.max(1, datos.paginaActual - 2);
            var final = Math.min(datos.totalPaginas, inicial + 4);
            inicial = Math.max(1, final - 4);

            lista.appendChild(crearEnlacePagina(
                "Anterior",
                datos.paginaActual - 1,
                datos.paginaActual === 1,
                false));

            if (inicial > 1) {
                lista.appendChild(crearEnlacePagina("1", 1, false, false));
                if (inicial > 2) {
                    lista.appendChild(crearEnlacePagina("…", 0, true, false));
                }
            }

            for (var numero = inicial; numero <= final; numero++) {
                lista.appendChild(crearEnlacePagina(
                    numero,
                    numero,
                    false,
                    numero === datos.paginaActual));
            }

            if (final < datos.totalPaginas) {
                if (final < datos.totalPaginas - 1) {
                    lista.appendChild(crearEnlacePagina("…", 0, true, false));
                }
                lista.appendChild(crearEnlacePagina(
                    datos.totalPaginas,
                    datos.totalPaginas,
                    false,
                    false));
            }

            lista.appendChild(crearEnlacePagina(
                "Siguiente",
                datos.paginaActual + 1,
                datos.paginaActual === datos.totalPaginas,
                false));
            nav.appendChild(lista);
            controles.appendChild(nav);
        }

        paginacion.appendChild(controles);
    }

    function parametrosHistorial(pagina) {
        var parametros = new URLSearchParams(new FormData(formulario));
        var selector = paginacion.querySelector("select[name='tamanoHistorial']");
        var tamano = selector ? selector.value : parametros.get("tamanoHistorial") || "10";
        parametros.set("paginaHistorial", pagina);
        parametros.set("tamanoHistorial", tamano);
        return parametros;
    }

    function actualizarUrl(parametros, paginaActual, tamanoPagina) {
        var url = new URL(window.location.href);
        var nombres = [
            "Filtros.Resultado",
            "Filtros.Modo",
            "Filtros.Desde",
            "Filtros.Hasta",
            "paginaHistorial",
            "tamanoHistorial"
        ];

        nombres.forEach(function (nombre) {
            url.searchParams.delete(nombre);
            var valor = parametros.get(nombre);
            if (valor) {
                url.searchParams.set(nombre, valor);
            }
        });
        url.searchParams.set("paginaHistorial", paginaActual);
        url.searchParams.set("tamanoHistorial", tamanoPagina);
        window.history.replaceState(null, "", url.pathname + url.search + url.hash);
        sincronizarRanking(url, nombres);
    }

    function sincronizarRanking(urlActual, nombres) {
        var contenedor = document.getElementById("ranking-paginacion");
        if (!contenedor) {
            return;
        }

        contenedor.querySelectorAll("a.page-link").forEach(function (enlace) {
            if (!enlace.href) {
                return;
            }
            var url = new URL(enlace.href);
            nombres.forEach(function (nombre) {
                url.searchParams.delete(nombre);
                if (urlActual.searchParams.has(nombre)) {
                    url.searchParams.set(nombre, urlActual.searchParams.get(nombre));
                }
            });
            enlace.href = url.pathname + url.search + url.hash;
        });

        contenedor.querySelectorAll("form").forEach(function (form) {
            nombres.forEach(function (nombre) {
                form.querySelectorAll("input[name='" + nombre + "']").forEach(function (input) {
                    input.remove();
                });
                if (urlActual.searchParams.has(nombre)) {
                    var oculto = document.createElement("input");
                    oculto.type = "hidden";
                    oculto.name = nombre;
                    oculto.value = urlActual.searchParams.get(nombre);
                    form.appendChild(oculto);
                }
            });
        });
    }

    function mostrarMensaje(texto) {
        mensaje.textContent = texto || "No se pudo actualizar el historial.";
        mensaje.classList.remove("d-none");
    }

    function ocultarMensaje() {
        mensaje.textContent = "";
        mensaje.classList.add("d-none");
    }

    function cambiarEstadoCarga(cargando) {
        contenido.classList.toggle("is-loading", cargando);
        contenido.setAttribute("aria-busy", cargando ? "true" : "false");
    }

    function cargarHistorial(pagina) {
        if (solicitudActual) {
            solicitudActual.abort();
        }

        var solicitud = new AbortController();
        solicitudActual = solicitud;
        var parametros = parametrosHistorial(pagina);
        ocultarMensaje();
        cambiarEstadoCarga(true);

        fetch(formulario.dataset.url + "?" + parametros.toString(), {
            method: "GET",
            credentials: "same-origin",
            headers: {
                "Accept": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            },
            signal: solicitud.signal
        })
            .then(function (respuesta) {
                var tipoContenido = respuesta.headers.get("content-type") || "";
                if (tipoContenido.indexOf("application/json") === -1) {
                    throw new Error("La sesión venció. Actualice la página para volver a ingresar.");
                }
                return respuesta.json().then(function (datos) {
                    if (!respuesta.ok || !datos.correcto) {
                        throw new Error(datos.mensaje || "No se pudo actualizar el historial.");
                    }
                    return datos;
                });
            })
            .then(function (datos) {
                dibujarTabla(datos.elementos);
                actualizarUrl(parametros, datos.paginaActual, datos.tamanoPagina);
                dibujarPaginacion(datos);
            })
            .catch(function (error) {
                if (error.name !== "AbortError") {
                    mostrarMensaje(error.message);
                }
            })
            .finally(function () {
                if (solicitudActual === solicitud) {
                    cambiarEstadoCarga(false);
                }
            });
    }

    formulario.addEventListener("submit", function (evento) {
        evento.preventDefault();
        cargarHistorial(1);
    });

    limpiar.addEventListener("click", function (evento) {
        evento.preventDefault();
        formulario.querySelectorAll("[name^='Filtros.']").forEach(function (campo) {
            campo.value = "";
        });
        cargarHistorial(1);
    });

    paginacion.addEventListener("click", function (evento) {
        var enlace = evento.target.closest("a.page-link");
        if (!enlace || !enlace.href) {
            return;
        }

        var pagina = enlace.dataset.pagina;
        if (!pagina) {
            pagina = new URL(enlace.href).searchParams.get("paginaHistorial");
        }
        if (!pagina) {
            return;
        }

        evento.preventDefault();
        cargarHistorial(parseInt(pagina, 10));
    });

    paginacion.addEventListener("change", function (evento) {
        if (evento.target.name === "tamanoHistorial") {
            cargarHistorial(1);
        }
    });
})();
