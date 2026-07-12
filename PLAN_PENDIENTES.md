# Plan de tareas pendientes — API_luceritoStore

> Análisis del estado actual del proyecto (.NET 8 Web API) y lista de lo que falta
> para que sea una API funcional, segura y mantenible.
> Este documento describe **qué** falta y **por qué**, con consejos de enfoque.
> No incluye la implementación.

---

## Resumen del estado actual

El proyecto tiene bien armada la **capa de acceso a datos** (carpeta `Data/`) y los
**modelos** (`Models/`), e incluso parte de la **lógica de negocio** (`Services/`).
Sin embargo, **hoy no es una API que se pueda consumir**: es una aplicación de consola.
El `Program.cs` tiene todo el arranque web comentado y en su lugar corre una prueba que
imprime ventas por consola.

En otras palabras: están las "piezas internas" (consultas SQL, validaciones de negocio),
pero falta todo el "cableado" que las expone al exterior como una API REST.

---

## 1. Bloqueantes — sin esto la API no existe

### 1.1 `Program.cs` no levanta la aplicación web
**Qué falta:** todo el código de arranque (`WebApplication.CreateBuilder`, registro de
servicios, `MapControllers`, `app.Run()`) está comentado. En su lugar hay una prueba de
consola que lista ventas.

**Por qué falta:** parece que se usó el `Program.cs` como "banco de pruebas" para
verificar que las consultas a la base funcionaban, y nunca se volvió a dejar el arranque
real. Mientras esté así, ningún endpoint responde por HTTP.

**Consejo:** restaurar el arranque web y mover cualquier prueba manual a un proyecto de
test aparte o eliminarla.

### 1.2 No hay Controllers reales (no hay endpoints)
**Qué falta:** el único controller es `WeatherForecastController`, que es la plantilla
por defecto de Visual Studio. No existen controllers para Usuarios, Productos, Ventas,
Carrito, Favoritos ni Categorías.

**Por qué falta:** se trabajó de "abajo hacia arriba" (datos → servicios) y todavía no se
llegó a la capa que publica las operaciones. Sin controllers, toda la lógica que ya
existe en `Services/` y `Data/` es inalcanzable desde afuera.

**Consejo:** crear un controller por entidad y exponer las operaciones que ya tienen
servicio/datos. Definir bien las rutas REST (p. ej. `GET /productos`,
`GET /productos/{id}`, `POST /ventas`, etc.) y devolver códigos HTTP correctos
(200/201/400/401/404) en lugar de los `int` mágicos actuales.

### 1.3 Inyección de dependencias sin configurar
**Qué falta:** los servicios no se registran en el contenedor de DI. `UsuarioService`
recibe `IConfiguration` por constructor (está pensado para DI), pero como no hay arranque
web ni registro, nadie lo inyecta.

**Por qué falta:** es consecuencia directa de 1.1. Sin el `builder.Services`, no hay
dónde registrar nada.

**Consejo:** registrar servicios y clases de datos en el contenedor. Esto además permite
inyectar la configuración (clave JWT, cadena de conexión) en vez de hardcodearla.

---

## 2. Seguridad — funcionalidad presente pero rota o insegura

### 2.1 La clave JWT está vacía
**Qué falta:** en `appsettings.json`, `Jwt:Key` es `""`. El método `Login` usa esa clave
para firmar el token con HMAC-SHA256.

**Por qué falta:** seguramente se dejó vacía para no subir un secreto al repo, pero nunca
se cargó por otro medio (User Secrets / variables de entorno). Con la clave vacía la firma
falla (HMAC-SHA256 exige una clave de longitud mínima).

**Consejo:** definir la clave fuera del código (User Secrets en desarrollo, variables de
entorno en producción) y nunca commitearla.

### 2.2 No hay validación de tokens (autenticación/autorización)
**Qué falta:** se **emiten** tokens en el login, pero no se configura `AddAuthentication` /
`AddJwtBearer` ni se usa `[Authorize]` en ningún lado. No se validan emisor, audiencia ni
expiración del lado servidor.

**Por qué falta:** otra consecuencia de que el pipeline web no está armado (1.1). Se hizo
la mitad del flujo (emitir) pero no la otra mitad (validar y proteger).

**Consejo:** configurar la validación del token y proteger los endpoints sensibles.
Distinguir endpoints públicos (login, registro, listar productos) de los protegidos
(crear venta, ver carrito propio, etc.).

### 2.3 Bug de seguridad en el login
**Qué falta/está mal:** `ObtenerUsuarioPorLogin` inicializa `usuario = new Usuario()` y lo
devuelve **aunque la contraseña sea incorrecta o el usuario no exista** (devuelve un objeto
vacío, no `null`). El servicio `Login` solo chequea `user == null`, así que no detecta el
caso. Resultado: con credenciales inválidas el flujo intenta generar un token para un
usuario "fantasma" (Id 0, nombre nulo) y termina fallando de forma inesperada.

**Por qué falta:** se mezcló la convención de "devolver objeto vacío" con la de "devolver
null". El chequeo de null en el servicio asume que datos devuelve null cuando no hay match,
pero datos devuelve un objeto vacío.

**Consejo:** unificar el contrato: que la capa de datos devuelva `null` cuando no haya
usuario válido o la contraseña no coincida, y validar eso en el servicio antes de emitir
el token.

### 2.4 No hay roles ni control de propiedad
**Qué falta:** operaciones que deberían ser de administrador (actualizar/eliminar
productos, cambiar estado de ventas) no tienen ninguna restricción de rol. Tampoco se
verifica que el `idUsuario` que pide un carrito o sus ventas sea el mismo del token.

**Por qué falta:** el modelo `Usuario` no tiene campo de rol y la autorización todavía no
se diseñó. Sin esto, cualquiera podría ver el carrito o las ventas de otro usuario pasando
otro id.

**Consejo:** decidir si habrá roles (admin/cliente) y, en los endpoints por usuario, tomar
el id desde el token en vez de confiar en el id que manda el cliente.

---

## 3. Configuración y base de datos

### 3.1 Cadena de conexión hardcodeada
**Qué falta:** `AccesoDatos` tiene la cadena de conexión escrita en el código
(`localhost\SQLEXPRESS`...), en vez de leerla de `appsettings`.

**Por qué falta:** es la forma más rápida de hacer andar las pruebas locales, pero rompe
en cualquier otra máquina o entorno y mezcla configuración con código.

**Consejo:** mover la cadena a configuración e inyectarla. Así cada entorno (local, test,
producción) usa la suya sin tocar el código.

### 3.2 No hay script de creación de la base de datos
**Qué falta:** no existe ningún `.sql` que cree `DB_luceritoStore` ni las tablas
(`Usuarios`, `Productos`, `CategoriaProductos`, `Ventas`, `DetalleVentas`, `Carritos`,
`CarritoProductos`, `Favoritos`).

**Por qué falta:** la base se creó manualmente en la máquina de desarrollo y nunca se
versionó. Cualquiera que clone el repo no puede levantar el proyecto porque no sabe el
esquema exacto (nombres de columnas, tipos, claves foráneas).

**Consejo:** versionar un script de esquema (y opcionalmente datos de ejemplo) en el repo.
Sirve como documentación viva de la estructura que el código asume.

---

## 4. Inconsistencias en el modelo de datos

### 4.1 `DetalleVenta` tiene `idProducto` y `Producto` a la vez
**Qué falta/está mal:** el modelo tiene tanto `idProducto` (entero suelto) como un objeto
`Producto`. En `VentaService.insertarVenta` se lee `detalle.idProducto`, pero en
`DetalleVentaDatos.insertarDetalle` se usa `detalle.Producto.IdProducto`. Si el front manda
solo el id (lo natural), `Producto` será null y al insertar el detalle se cae con
NullReferenceException.

**Por qué falta:** el modelo creció con dos ideas distintas de cómo representar el producto
de un detalle y nunca se unificó. Es un campo duplicado que genera ambigüedad.

**Consejo:** elegir una sola representación (lo más simple: el id en la entrada y el objeto
`Producto` solo en la salida) y usar la misma en servicio y datos.

### 4.2 El modelo `Usuario` expone la clave
**Qué falta:** `Usuario` incluye `ClaveUsuario`, y los modelos se usan directamente como
entrada y salida de la API (no hay DTOs). Aunque varias consultas evitan traer la clave,
el contrato queda frágil: es fácil filtrar el hash sin querer.

**Por qué falta:** es lo más rápido reutilizar los modelos del dominio como contratos de
API. Funciona al principio pero acopla la base de datos a lo que ve el cliente.

**Consejo:** separar modelos de dominio de los DTOs de entrada/salida, sobre todo para no
devolver nunca la clave y para validar entradas de forma explícita.

---

## 5. Calidad y robustez del código

### 5.1 Conexiones que pueden quedar abiertas (fugas)
**Qué falta:** varios métodos no cierran la conexión si hay excepción, porque no usan
`try/finally` ni `using`. Ejemplos: `VentaDatos.actualizarEstadoVenta`,
`VentaDatos.obtenerVentaPorId` (cuando encuentra la venta, hace `return` **antes** de
cerrar), y los tres métodos de `CarritoDatos`.

**Por qué falta:** se fue copiando el patrón de "abrir, ejecutar, cerrar" sin la red de
seguridad del `finally`. Mientras no haya errores parece funcionar, pero bajo carga o ante
fallos las conexiones se acumulan y se agota el pool.

**Consejo:** envolver las conexiones en `using` (se cierran solas) o asegurar el cierre en
`finally` en todos los métodos, como ya se hace en otros.

### 5.2 Manejo de errores inconsistente
**Qué falta:** conviven tres estilos: devolver códigos `int` mágicos (`-1`, `-2`...),
devolver `null`, y lanzar `Exception` genérica. No hay un manejador global de errores.

**Por qué falta:** cada método se resolvió de forma aislada. Sin una convención común, los
controllers (cuando existan) van a tener que traducir cada caso a mano y es fácil
equivocarse.

**Consejo:** unificar el estilo de errores y agregar un manejador global de excepciones
para que la API responda siempre con un formato y código HTTP coherentes.

### 5.3 Falta capa de servicio para varias entidades
**Qué falta:** `Carrito`, `CarritoProducto`, `Favoritos` y varias operaciones de
`Productos` (listar, eliminar, buscar por nombre) tienen capa de datos pero **no** tienen
servicio con validaciones.

**Por qué falta:** se priorizó la lógica de ventas y usuarios. El resto quedó "a medias":
se puede leer/escribir en la base pero sin reglas de negocio (p. ej. validar que el
producto exista antes de agregarlo al carrito, o que la cantidad sea positiva).

**Consejo:** completar los servicios faltantes con sus validaciones antes de exponerlos.

### 5.4 Limpieza de la plantilla y convenciones de nombres
**Qué falta:** siguen los archivos de plantilla (`WeatherForecast.cs`,
`WeatherForecastController.cs`) que no pertenecen al dominio. Además hay nombres de clases
en minúscula (`categoriaDatos`, `favoritosDatos`) que rompen la convención del resto.

**Por qué falta:** restos del template inicial y de distintos momentos/personas escribiendo
código. No es grave, pero ensucia y confunde.

**Consejo:** eliminar lo de Weather y unificar el estilo de nombres (PascalCase para
clases).

---

## 6. Faltantes para integrarse con un front-end

### 6.1 CORS sin configurar
**Qué falta:** no hay política de CORS. Si la tienda tiene un front (web/móvil) en otro
dominio, el navegador bloqueará las llamadas.

**Por qué falta:** todavía no se llegó a la etapa de integración con el cliente (depende de
que primero exista el pipeline web, punto 1.1).

**Consejo:** definir una política de CORS que permita el origen del front.

### 6.2 Documentación de la API (Swagger)
**Qué falta:** el paquete de Swagger está instalado pero, al estar comentado el arranque,
no se expone. Sin endpoints tampoco hay nada que documentar todavía.

**Por qué falta:** consecuencia de 1.1 y 1.2.

**Consejo:** una vez levantada la API, habilitar Swagger en desarrollo para probar y
documentar los endpoints.

---

## Orden sugerido de trabajo

1. **Restaurar el arranque web** (`Program.cs`) y la inyección de dependencias. *(1.1, 1.3)*
2. **Crear los controllers** y definir las rutas/contratos REST. *(1.2)*
3. **Cerrar el flujo de seguridad:** clave JWT por configuración, validación de tokens,
   arreglar el bug de login. *(2.1, 2.2, 2.3)*
4. **Mover la cadena de conexión a configuración** y **versionar el script de base de
   datos**. *(3.1, 3.2)*
5. **Unificar el modelo `DetalleVenta`** y separar DTOs. *(4.1, 4.2)*
6. **Tapar fugas de conexión** y **unificar manejo de errores**. *(5.1, 5.2)*
7. **Completar servicios faltantes** (carrito, favoritos, productos). *(5.3)*
8. **Roles/propiedad, CORS, Swagger y limpieza** del template. *(2.4, 5.4, 6.1, 6.2)*

> La idea del orden es: primero que la API **exista y responda**, después que sea **segura**,
> después que sea **consistente**, y por último **pulido** e integración con el front.
