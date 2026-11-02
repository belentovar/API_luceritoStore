# Estado del proyecto — API LuceritoStore

> Archivo de contexto para retomar el trabajo. Actualizado: 2026-11-02.
> Documentos relacionados: `PLAN_PENDIENTES.md` (plan por bloques),
> `APUNTE_INYECCION_DEPENDENCIAS.md` (apunte de estudio), `GUIA_GIT.md`.

---

## Resumen en una línea

La API arranca, compila con 0 errores y tiene la capa de servicios casi completa.
No tiene controllers todavía, así que no responde pedidos HTTP.

---

## Cómo preferiría trabajar

Belén está aprendiendo .NET y ASP.NET Core. Pide explícitamente:

- Definir la jerga técnica la primera vez que aparece, al costado, en una línea.
- Explicar la idea antes del código, y después recorrer el código línea por línea.
  No volcar un bloque y seguir.
- Una analogía concreta por concepto.
- Mostrar antes/después cuando se refactoriza.
- Verificar el estado real (compilar, leer el archivo) antes de afirmar algo.
  Valora que se le diga con honestidad cuando algo no compila.
- Darle el patrón resuelto en un ejemplo y dejarle a ella el trabajo repetitivo,
  con un checklist.
- No asumir conocimiento operativo: cómo se abre una terminal, qué significa
  "desde la raíz", dónde vive un archivo.

Sigue tutoriales de YouTube, así que puede arrastrar patrones que funcionan pero
son frágiles. Conviene explicar POR QUÉ el patrón del tutorial es frágil, en lugar
de decir solamente que está mal.

---

## Stack

| | |
| --- | --- |
| Framework | .NET 8 (`net8.0`) — ASP.NET Core Web API |
| Base de datos | SQL Server Express, `DB_luceritoStore` |
| Acceso a datos | ADO.NET a mano (`Microsoft.Data.SqlClient`), sin ORM |
| Autenticación | JWT (`Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.11) |
| Hash de claves | BCrypt.Net-Next |
| Documentación | Swashbuckle (Swagger) |
| Secretos | User Secrets en desarrollo |
| Script de la base | `bdd/script.sql` |

Arquitectura en tres capas: `Data/` (acceso a datos) → `Services/` (lógica de
negocio y validaciones) → `Controllers/` (traducción HTTP, todavía vacía).

---

## ✅ Bloque 1 — Infraestructura (COMPLETADO)

- `Program.cs` convertido de script de consola a aplicación web: `AddControllers`,
  Swagger, validación de JWT Bearer, `UseAuthentication` / `UseAuthorization`,
  `MapControllers`, `app.Run()`.
- Paquete `JwtBearer` 8.0.11 instalado (la primera vez se probó una versión 9.x,
  incompatible con `net8.0`).
- Cadena de conexión movida a `appsettings.json`. Se corrigió un bug: la clave
  estaba escrita `cadenacadenaConexion` y el código leía `cadenaConexion`,
  así que devolvía `null`.
- Clave JWT de 64 caracteres guardada en User Secrets, fuera del repositorio.
  `appsettings.json` quedó con la sección `"Jwt": {}` vacía.
- `WeatherForecast.cs` y su controller eliminados.
- Script de pruebas del carrito eliminado de `Program.cs`.
- **Refactor completo a inyección de dependencias**: las 10 clases `*Datos` y los
  4 `*Service` reciben sus dependencias por constructor. No queda ningún `new` de
  clases propias. 15 registros `AddScoped` en `Program.cs`, todos antes de
  `builder.Build()`. Se pasó de 49 errores de compilación a 0.

---

## 🔄 Bloque 3 — Métodos faltantes en los services (en progreso)

### Hecho

| Método | Archivo |
| --- | --- |
| `obtenerTotalCarrito(int)` | `Services/CarritoService.cs:178` |
| `checkoutDesdeCarrito(...)` | `Services/VentaService.cs:27` |

`checkoutDesdeCarrito` era la pieza que faltaba en todo el proyecto: antes
`CarritoService` y `VentaService` no se comunicaban. Ahora `VentaService` recibe
`CarritoService` por constructor y el flujo completo es: leer el carrito →
armar los `DetalleVenta` → llamar a `insertarVenta` (que valida stock y usa
transacción) → vaciar el carrito → devolver el `idVenta`.

Firma: `checkoutDesdeCarrito(int idUsuario, string metodoPago, string tipoEntrega, string direccion, int idLocalidad)`.
El `idUsuario` tiene que salir del token JWT, nunca del body.

### Falta

**VentaService**
- `listarVentasPorUsuario(int)` — "mis compras".
  `DetalleVentaDatos.listarVentasConDetallePorUsuario` ya existe y no se usa,
  así que es casi gratis.
- `listarVentas()` para administración — falta también en la capa de datos.
- Validar `MetodoPago` en el service. Hoy solo lo cubre el CHECK de la base,
  lo que produce una `SqlException` cruda en lugar de un mensaje claro.

**UsuarioService** (el más incompleto)
- `obtenerUsuarioPorId(int)` — `UsuariosDatos.obtenerUsuarioPorId` ya existe.
- `darDeBajaUsuario(int)` / `darDeAltaUsuario(int)` — los datos ya existen.
- `actualizarUsuario(Usuario)` — no existe ni en la capa de datos. Sin esto nadie
  puede editar su perfil ni cambiar su clave.
- Incluir el `Rol` en los claims del JWT. Sin ese claim no se puede usar
  `[Authorize(Roles = "admin")]` en ningún endpoint.
- **Seguridad**: `AgregarUsuario` acepta el `Rol` que envíe el cliente, así que
  cualquiera puede registrarse como administrador. Hay que forzar `"cliente"`
  dentro del service.
- Validar formato de correo electrónico.

**CarritoService**
- `CarritoProductoDatos.obtenerProductosCarrito` selecciona `ImagenProducto`
  pero no lo mapea, y tampoco trae `Stock`. El carrito llega incompleto al cliente.

**ProductoService**
- `listarProductosPorCategoria` lanza `Exception` genérico; el resto lanza
  `InvalidOperationException`. Unificar.
- `listarProductos` no tiene paginado ni filtro por `Activo`.

---

## ⏭️ Decisión pendiente: Bloque 4 — contrato de errores

Era el próximo paso acordado, antes de seguir escribiendo services.

Hoy los services avisan los errores de tres formas distintas, a veces en el mismo
método:

1. `return 0` — `VentaService.insertarVenta` cuando el usuario no existe o el
   detalle está vacío.
2. Excepciones — `throw new Exception("No existe el producto")`,
   `throw new InvalidOperationException(...)`, 20 líneas más abajo en ese mismo método.
3. Códigos negativos — `UsuarioService.AgregarUsuario` devuelve `-1`, `-2`, `-3`, `-4`.

**Por qué importa:** ya produjo un agujero real. En `checkoutDesdeCarrito`, si
`insertarVenta` devuelve `0`, el código sigue de largo y **vacía el carrito** sin
haber vendido nada. Con excepciones eso no podría pasar, porque cortan la ejecución.

Además, los controllers tienen que traducir el resultado del service a un código
HTTP (400, 404, 409). Con un `0` no se puede elegir cuál corresponde.

**Recomendación: excepciones para todo (opción A).** Razones: ya es la convención
mayoritaria (`CarritoService` y `ProductoService` enteros la usan), no se puede
ignorar por accidente, y el valor de retorno queda libre para el resultado real
(`checkoutDesdeCarrito` devuelve el `idVenta`, no un código disfrazado).

Trabajo que implica: elegir 2 o 3 tipos de excepción, reemplazar los `return 0` de
`VentaService` y los `-1..-4` de `UsuarioService`, cambiar los
`throw new Exception(...)` genéricos por tipos concretos, y anotar la regla en el
apunte.

---

## 📋 Orden de trabajo acordado

```
Bloque 1 ✅  →  Bloque 3 (en progreso)  →  Bloque 4  →  Bloque 2  →  Controllers
```

El Bloque 4 se adelantó respecto del plan original: si se deja para el final, los
cuatro services nuevos del Bloque 2 se escriben con una convención y después hay
que reescribirlos.

**Bloque 2** (pendiente) son cuatro services nuevos, mecánicos, cuya capa de datos
ya está escrita: `CategoriaService`, `FavoritosService`, `ProvinciaService`,
`LocalidadService`.

**Bloque 5** son los controllers, uno por agregado. Un controller solo traduce:
recibe el pedido, delega en el service, convierte el resultado en un código HTTP.
Toda la inteligencia vive en los services, y por eso van al final.

---

## ⚠️ Pendiente de Git (sin resolver)

Nada de este trabajo está commiteado. El último commit es `2889ed0 "cambios 10/8"`.

Y hay un problema que conviene arreglar antes de commitear: **57 archivos de
`bin/` y `obj/` están trackeados por Git**. El `.gitignore` existe y es correcto
(ignora `[Bb]in/` y `[Oo]bj/`), pero llegó después de que esos archivos ya
estuvieran en el repositorio, y `.gitignore` solo afecta a archivos que Git todavía
no sigue.

Los pasos, en este orden:

```
git rm -r --cached API_luceritoStore/bin API_luceritoStore/obj
git status
git add .
git status
git commit -m "feat: migrar a inyeccion de dependencias y agregar checkout desde carrito"
git push
```

`--cached` saca los archivos del índice de Git pero los deja en el disco. Sin ese
flag, `git rm` los borraría de verdad.

Los commits del repositorio dicen `"cambios 10/8"`, `"cambios 9-8"`. La fecha ya la
guarda Git sola; el mensaje debería decir qué cambió. Convención a usar:
conventional commits (`feat:`, `fix:`, `refactor:`, `docs:`, `chore:`).

---

## Notas del entorno

- `dotnet build` funciona. Hubo un episodio en el que la instalación de .NET quedó
  a medio actualizar y faltaba el runtime (`host/fxr does not exist`); se resolvió
  solo al terminar la actualización.
- El build arroja 87 advertencias, ninguna nueva ni bloqueante (casi todas de
  nulabilidad).
- Valores exactos que exigen los CHECK de la base, sensibles a mayúsculas:
  - `metodoPago`: `transferencia` | `efectivo` | `mercadopago` (minúscula)
  - `tipoEntrega`: `retiro` | `envio` (minúscula)
  - `estado`: `Pendiente` | `Pagado` | `Enviado` | `Cancelado` (inicial mayúscula)
