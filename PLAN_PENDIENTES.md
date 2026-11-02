# Plan de pendientes — API LuceritoStore

Estado relevado el 2026-08-20. El proyecto compila (0 errores, 87 advertencias).

Orden sugerido de trabajo: Bloque 1 → Bloque 3 (checkout) → Bloque 2 → Bloque 4 → Controllers.

---

## Bloque 1 — Infraestructura de la API ✅ COMPLETADO

> Estado original: `Program.cs` era un script de consola que probaba el carrito.
> No arrancaba un servidor web, por lo que ningun controller podria haber respondido.

Hecho ademas de lo listado: refactor completo a inyeccion de dependencias.
Las 10 clases `*Datos` y los 4 `*Service` reciben sus dependencias por constructor
y estan registrados con `AddScoped` en `Program.cs`. No queda ningun `new` de
clases propias. La API compila con 0 errores y arranca correctamente.

- [x] Restaurar `Program.cs` como aplicacion web:
      `WebApplication.CreateBuilder` -> `AddControllers` -> `AddSwaggerGen` ->
      `UseAuthentication` / `UseAuthorization` -> `MapControllers` -> `app.Run()`
- [x] Mover el script de pruebas del carrito a un proyecto de tests aparte (o eliminarlo)
- [x] Agregar el paquete `Microsoft.AspNetCore.Authentication.JwtBearer`
      (hoy se emite el token pero nadie lo valida: `[Authorize]` no funcionaria)
- [x] Eliminar `WeatherForecast.cs` y `Controllers/WeatherForecastController.cs` (restos de la plantilla)
- [x] Mover la cadena de conexion de `Data/AccesoDatos.cs` a `appsettings.json`
- [x] Reemplazar `"Jwt:Key": "1829"` por una clave de 32 caracteres o mas,
      almacenada en User Secrets (desarrollo) y variables de entorno (produccion).
      HMAC-SHA256 exige 256 bits; con 4 caracteres falla en tiempo de ejecucion.

---

## Bloque 2 — Services que no existen

La capa de datos ya esta escrita para los cuatro. Falta unicamente la capa de
servicio con sus validaciones.

### CategoriaService
Datos disponibles: `listarCategorias`, `AgregarCategoria`, `darDeBajaCategoria`,
`darDeAltaCategoria`, `ExisteCategoria`.
- [ ] `listarCategorias()`
- [ ] `agregarCategoria(CategoriaProducto)` — nombre obligatorio, sin duplicados
- [ ] `darDeBajaCategoria(int)` / `darDeAltaCategoria(int)` — validar id > 0 y existencia

### FavoritosService
Datos disponibles: `existeFavorito`, `agregarFavoritos`, `eliminarFavorito`,
`listarFavoritosPorUsuario`.
- [ ] `listarFavoritosPorUsuario(int)` — validar que el usuario exista
- [ ] `agregarFavorito(int idUsuario, int idProducto)` — validar usuario y producto
- [ ] `eliminarFavorito(int idUsuario, int idFavorito)` — verificar que el favorito
      pertenezca al usuario antes de borrar (mismo criterio que `CarritoService`)

### ProvinciaService
Datos disponibles: `ListarProvincias`, `agregarProvincia`, `darDeBajaProvincia`,
`darDeAltaProvincia`.
- [ ] `listarProvincias()`
- [ ] `agregarProvincia(Provincia)` — nombre obligatorio
- [ ] `darDeBajaProvincia(int)` / `darDeAltaProvincia(int)`

### LocalidadService
Datos disponibles: `listarPorProvincia`, `obtenerLocalidadPorId`, `agregarLocalidad`,
`darDeBajaLocalidad`, `darDeAltaLocalidad`.
- [ ] `listarPorProvincia(int)` — validar que la provincia exista
- [ ] `obtenerLocalidadPorId(int)`
- [ ] `agregarLocalidad(Localidad)` — validar que la provincia exista y este activa
- [ ] `darDeBajaLocalidad(int)` / `darDeAltaLocalidad(int)`

---

## Bloque 3 — Metodos faltantes en los Services existentes

### UsuarioService (el mas incompleto)
- [ ] `obtenerUsuarioPorId(int)` — ver perfil. `UsuariosDatos.obtenerUsuarioPorId` ya existe
- [ ] `darDeBajaUsuario(int)` / `darDeAltaUsuario(int)` — los datos ya existen
- [ ] `actualizarUsuario(Usuario)` — no existe ni en la capa de datos.
      Sin esto nadie puede editar su perfil ni cambiar su clave
- [ ] Incluir el `Rol` en los claims del JWT.
      Sin ese claim no es posible usar `[Authorize(Roles = "admin")]` en ningun endpoint
- [ ] `AgregarUsuario` acepta el `Rol` que envie el cliente: cualquiera puede registrarse
      como administrador. Forzar `"cliente"` dentro del service
- [ ] Validar formato de correo electronico
- [ ] Unificar el manejo de errores: hoy devuelve codigos `-1`, `-2`, `-3`, `-4`
      mientras el resto de los services lanza excepciones

### VentaService
- [ ] `listarVentasPorUsuario(int)` — "mis compras".
      `DetalleVentaDatos.listarVentasConDetallePorUsuario` ya existe y no se usa
- [ ] `listarVentas()` para administracion — falta tambien en la capa de datos
- [ ] `checkoutDesdeCarrito(int idUsuario, ...)` — pieza clave que falta.
      Hoy `CarritoService` y `VentaService` no se comunican. Debe tomar el carrito,
      convertirlo en `List<DetalleVenta>`, llamar a `insertarVenta` y vaciar el carrito
- [ ] Validar `MetodoPago` en el service. Hoy solo lo cubre el CHECK de la base,
      lo que produce una `SqlException` cruda en lugar de un mensaje claro
- [ ] Unificar errores: mezcla `return 0` con `throw`

### CarritoService
- [ ] `obtenerTotalCarrito(int)` — hoy el total se calcula en el cliente
- [ ] `CarritoProductoDatos.obtenerProductosCarrito` selecciona `ImagenProducto`
      pero no lo mapea, y tampoco trae `Stock`. El carrito llega incompleto al cliente

### ProductoService
- [ ] `listarProductosPorCategoria` lanza `Exception` generico;
      el resto lanza `InvalidOperationException`. Unificar
- [ ] `listarProductos` no tiene paginado ni filtro por `Activo`

---

## Bloque 4 — Decision transversal previa a los controllers

- [ ] Definir el contrato de errores de la capa de servicio: excepciones o codigos de retorno.
      Esta decision determina como cada controller traduce los fallos a codigos HTTP
      (400, 404, 409). Tomarla antes de escribir el primer controller evita reescribirlos todos.

---

## Bloque 5 — Controllers (al final, segun lo planificado)

Uno por agregado, cada uno delegando en su service correspondiente:

- [ ] `ProductosController`
- [ ] `CategoriasController`
- [ ] `UsuariosController` (registro, login, perfil)
- [ ] `CarritoController`
- [ ] `VentasController`
- [ ] `FavoritosController`
- [ ] `ProvinciasController` / `LocalidadesController`
