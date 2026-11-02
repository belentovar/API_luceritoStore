# APUNTE

Notas de estudio del proyecto API LuceritoStore.

- **Parte 1 — RECORDAR:** las ideas que hay que tener siempre presentes, agrupadas por tema.
- **Parte 2 — Detalle:** inyección de dependencias explicada a fondo.

---
---

# PARTE 1 — 🧠 RECORDAR

## Tema A — Inyección de Dependencias

### A.1 · Qué es

> **RECORDAR:** DI significa que una clase **declara** lo que necesita en su
> constructor, y otro se lo entrega. Nunca se lo fabrica ella.

**Por qué es así:** cuando una clase se fabrica sus propias herramientas con `new`,
queda **soldada** a esa versión exacta. No podés cambiarla, no podés testearla con
una falsa, no podés configurarla desde afuera. Al pedirla, esa decisión se toma
**en un solo lugar** (`Program.cs`) en vez de estar repetida decenas de veces por
todo el proyecto.

### A.2 · La regla de oro

> **RECORDAR:** .NET **solo puede inyectar en objetos que .NET mismo creó.**

**Por qué es así:** la inyección ocurre en el momento de la construcción. Si vos
escribís `new MiClase()` con tus propias manos, ese objeto nació fuera del
contenedor y nadie tuvo la oportunidad de darle nada.

Por eso el refactor **no es opcional a medias**: si dejás un solo `new`, la cadena
se corta ahí.

### A.3 · Dónde se pide

> **RECORDAR:** **el constructor pide, los métodos usan.**

**Por qué es así:** el constructor corre **una sola vez**, cuando nace el objeto.
Ese es el único momento en que .NET puede entregarte cosas. Los métodos corren
muchas veces después, y para entonces ya nadie está repartiendo nada — por eso
guardás lo recibido en un campo `private readonly`.

### A.4 · Lifetimes

> **RECORDAR:** el lifetime no dice **qué** se crea, dice **cada cuánto**.
> - `Transient` → uno por cada quien lo pide
> - `Scoped` → uno por pedido HTTP
> - `Singleton` → uno para toda la vida de la app

**Por qué existen tres:** porque la pregunta real es *¿este objeto guarda algo
adentro que sería un problema compartir?*

- Una conexión a base de datos guarda estado del usuario actual → si la compartís
  entre usuarios simultáneos se pisan → **Scoped**
- La configuración no cambia nunca → releerla en cada pedido es puro desperdicio
  → **Singleton**
- Un validador no guarda nada y es baratísimo → da igual cuántos haya →
  **Transient**

**El truco mental:** `Scoped` = la comanda de una mesa.
**Comparte adentro, aísla afuera.**

---

## Tema B — Configuración y secretos

### B.1 · Interfaces (`IConfiguration`)

> **RECORDAR:** una **interfaz** es un contrato: dice *qué* se puede hacer, no
> *cómo*. La `I` inicial es convención de C#.

**Por qué pedís `IConfiguration` y no una clase concreta:** porque a tu código no
le importa **de dónde** sale la configuración. Le importa poder preguntar "dame el
valor de esta clave". Hoy sale de un JSON, mañana de User Secrets, en producción
de variables de entorno — **y tu código no cambia una línea.**

Eso es lo que ganás al depender del contrato en vez de la implementación.

### B.2 · La cascada de configuración

> **RECORDAR:** ASP.NET lee la configuración de **cuatro fuentes en orden**, y
> cada una pisa a la anterior:
> `appsettings.json` → `appsettings.Development.json` → **User Secrets** →
> **variables de entorno**

**Por qué es así:** para que **el mismo código** funcione en tu máquina y en
producción sin tocarse. Cambia el entorno, no el código.

**El corolario:** si leés el JSON a mano con `ConfigurationBuilder`, te quedás
**solo con el escalón 1**. Los secretos nunca llegan. Por eso el `IConfiguration`
inyectado no es un capricho de estilo — **es el único que ve la cascada completa.**

### B.3 · Secretos

> **RECORDAR:** una clave, contraseña o cadena de conexión **nunca** va en un
> archivo que se sube a Git.

**Por qué:** porque Git guarda **historial**. Si la subís y después la borrás,
sigue estando en el historial para siempre. La única forma segura es que nunca
haya entrado.

User Secrets vive **fuera de la carpeta del proyecto**, así que Git no la puede
tocar ni queriendo.

---

## Tema C — El arranque de la aplicación

### C.1 · Las dos mitades de `Program.cs`

> **RECORDAR:** `builder.Build()` parte el archivo en dos. **Arriba se prepara,
> abajo se atiende.** Y `app.Run()` bloquea para siempre.

**Por qué es así:** .NET necesita el catálogo de recetas **completo** antes de
abrir la cocina. Una vez que la app arrancó, el catálogo se congela — si no, un
pedido a mitad de camino podría encontrarse con reglas distintas a las de otro
pedido simultáneo.

**El corolario:** cualquier cosa escrita después de `app.Run()` compila pero
**nunca se ejecuta**. El compilador no te avisa.

### C.2 · El pipeline

> **RECORDAR:** cada pedido atraviesa una **fila de middlewares**, en orden, uno
> atrás del otro. Cada `app.UseAlgo()` agrega uno a la fila.

**Por qué importa el orden:** es como la fila del aeropuerto. Primero mostrás el
pasaje, después pasás por seguridad, después migraciones. Invertirlo no da error;
simplemente no funciona.

---

## Tema D — HTTP y seguridad

### D.1 · Anatomía de un pedido

> **RECORDAR:** un pedido HTTP tiene **cuatro partes**, no una:
> **método + URL + headers + body.**

**Por qué importa:** `GET /api/productos/5` y `DELETE /api/productos/5` tienen la
misma URL y hacen cosas opuestas. **La URL sola no dice qué está pasando.**

**El corolario de seguridad:** el token va en un **header** y la contraseña va en
el **body** — nunca en la URL, porque las URLs quedan guardadas en el historial
del navegador y en los logs del servidor.

### D.2 · Autenticación ≠ Autorización

> **RECORDAR:** **Autenticación = ¿quién sos?** · **Autorización = ¿podés hacer
> esto?** Siempre en ese orden.

**Por qué el orden es obligatorio:** no podés preguntar "¿este usuario es admin?"
antes de saber **qué usuario es**. Por eso `app.UseAuthentication()` va antes que
`app.UseAuthorization()`. Si los invertís, la app arranca igual y la seguridad
simplemente **no funciona** — el peor tipo de bug: silencioso.

### D.3 · Códigos de estado

> **RECORDAR:** `2xx` salió bien · `4xx` se equivocó el cliente · `5xx` se rompió
> el servidor.

**Por qué importa:** ese es el trabajo del controller — **traducir tu lógica a
HTTP.** Producto no existe → `404`. Datos mal formados → `400`. Stock insuficiente
→ `409`. Si mandás un `500` cuando el usuario escribió mal un id, le estás echando
la culpa a tu servidor por un error de él.

---

## Tema E — Principios generales

### E.1 · Fallar temprano

> **RECORDAR:** **fallar temprano y fuerte es mejor que fallar tarde y en
> silencio.**

Por eso el `?? throw new InvalidOperationException(...)` en `AccesoDatos`: si falta
la configuración, la app **explota al arrancar con un mensaje claro**, en vez de
dar un `NullReferenceException` misterioso tres semanas después, en producción, un
viernes a las 6 de la tarde.

Esta idea vuelve a aparecer una y otra vez en toda la carrera.

---
---

# PARTE 2 — Inyección de Dependencias en detalle

## 1. El problema que resuelve

Antes, cada clase se fabricaba sola lo que necesitaba:

```csharp
public List<CategoriaProducto> listarCategorias()
{
    AccesoDatos acceso = new AccesoDatos();   // se lo fabrica a sí misma
    ...
}
```

Ahora lo **pide** y alguien se lo entrega:

```csharp
public class categoriaDatos
{
    private readonly AccesoDatos acceso;

    public categoriaDatos(AccesoDatos acceso)   // lo pide
    {
        this.acceso = acceso;
    }
}
```

> **Inyección de dependencias (DI)** = en vez de que cada clase se fabrique lo que
> necesita, hay un "repartidor" central que se lo entrega. La clase solo declara
> en su constructor qué necesita, y aparece.

**Analogía:** en un restaurante, el cocinero no va al mercado a comprar tomates.
Los pide y alguien se los trae. El cocinero cocina; conseguir ingredientes es
problema de otro.

---

## 2. Qué hace (y qué NO hace) `AddScoped`

```csharp
builder.Services.AddScoped<DetalleVentaDatos>();
```

**NO crea ningún objeto.** Esto es lo primero que hay que sacarse de la cabeza.
Cuando esa línea se ejecuta no se fabrica nada.

Lo que dice, literalmente, es:

> *"Che .NET, anotate esto: si alguna vez alguien te pide un `DetalleVentaDatos`,
> vos sabés fabricarlo. Guardá la receta."*

Es **registrar una receta en un catálogo**, no cocinar el plato.

### ¿Y cuál es la receta? .NET la deduce sola

Vos no escribís la receta. .NET la deduce mirando el constructor:

```csharp
public DetalleVentaDatos(AccesoDatos acceso)
```

Y razona así:

> Para fabricar un `DetalleVentaDatos` necesito un `AccesoDatos`.
> ¿Tengo receta de `AccesoDatos`? Sí, está registrada.
> A ver su constructor... `AccesoDatos(IConfiguration configuration)`.
> Necesito un `IConfiguration`. ¿Tengo? Sí, ese lo registré yo solo al arrancar.
> Listo, tengo todo.

Y arma la cadena **de abajo hacia arriba**:

```
IConfiguration ✔ → AccesoDatos ✔ → DetalleVentaDatos ✔ → te lo entrega
```

Todo eso, sin que vos escribas un solo `new`.

### La analogía del restaurante

| En el restaurante                             | En la API                                    |
| --------------------------------------------- | -------------------------------------------- |
| El **libro de recetas** de la cocina          | El contenedor de DI                          |
| Anotar una receta nueva en el libro           | `builder.Services.AddScoped<T>()`            |
| El **cocinero** que lee la receta y prepara   | .NET, cuando llega un pedido                 |
| Un **mozo pide** "una milanesa"               | Un controller pide `DetalleVentaDatos`       |
| La receta dice "necesita carne y pan rallado" | El constructor dice "necesita `AccesoDatos`" |
| Buscar carne y pan rallado                    | .NET busca `AccesoDatos` en el catálogo      |

Si la receta **no está anotada**, el mozo pide milanesa y el cocinero dice
"no sé qué es eso".

---

## 3. Los tres lifetimes: Singleton, Scoped, Transient

> **Lifetime (tiempo de vida)** = cada cuánto .NET fabrica un objeto **nuevo**
> en lugar de reusar uno que ya tenía.

Los tres registran la misma receta. Lo único que cambia es **cuántas veces se cocina**.

### Las tres frases

| Método              | Se crea uno nuevo...                       |
| ------------------- | ------------------------------------------ |
| `AddTransient<T>()` | **cada vez que alguien lo pide**           |
| `AddScoped<T>()`    | **una vez por cada pedido HTTP**           |
| `AddSingleton<T>()` | **una sola vez en toda la vida de la app** |

> **Pedido HTTP (request)** = una sola operación completa. Alguien entra a
> `GET /api/ventas/mis-compras`, la API responde, y ese pedido terminó.
> El siguiente pedido, aunque sea del mismo usuario, es otro pedido distinto.

### El ejemplo concreto

Supongamos este pedido:

```
GET /api/ventas/mis-compras
```

Para responderlo, la cadena de la app usa **dos** clases que necesitan `AccesoDatos`:

```
VentaController
      └── VentaService
             ├── VentaDatos           → necesita AccesoDatos
             └── DetalleVentaDatos    → necesita AccesoDatos
```

Pregunta clave: **¿cuántos objetos `AccesoDatos` se crean?**

Depende del lifetime.

---

#### 🔴 Transient — "cada vez que me piden"

```
Pedido HTTP #1  (usuario Belén)
  ├── VentaDatos          recibe  AccesoDatos #1   ← objeto nuevo
  └── DetalleVentaDatos   recibe  AccesoDatos #2   ← OTRO objeto nuevo

Pedido HTTP #2  (usuario Juan)
  ├── VentaDatos          recibe  AccesoDatos #3
  └── DetalleVentaDatos   recibe  AccesoDatos #4
```

**2 objetos para UN solo pedido.** Dos conexiones distintas a SQL Server para
atender una sola operación. Funciona, pero es un desperdicio.

Analogía: una **servilleta**. Cada vez que hace falta, una nueva.

---

#### 🟢 Scoped — "uno por pedido HTTP" ← el que usamos

```
Pedido HTTP #1  (usuario Belén)
  ├── VentaDatos          recibe  AccesoDatos #1   ← se crea acá
  └── DetalleVentaDatos   recibe  AccesoDatos #1   ← EL MISMO, reusado
                                                     (al terminar, se descarta)

Pedido HTTP #2  (usuario Juan)
  ├── VentaDatos          recibe  AccesoDatos #2   ← objeto nuevo y limpio
  └── DetalleVentaDatos   recibe  AccesoDatos #2   ← el mismo #2
```

**1 objeto por pedido.** Se comparte **dentro** del pedido (no se desperdicia),
pero cada usuario tiene el suyo (queda aislado de los demás).

Analogía: la **comanda de una mesa**. Una por mesa, se usa durante toda la cena,
se tira al final. La mesa de al lado tiene la suya.

---

#### 🔴 Singleton — "uno para toda la app"

```
Pedido HTTP #1  (usuario Belén)
  ├── VentaDatos          recibe  AccesoDatos #1
  └── DetalleVentaDatos   recibe  AccesoDatos #1

Pedido HTTP #2  (usuario Juan)          ← ¡al mismo tiempo que Belén!
  ├── VentaDatos          recibe  AccesoDatos #1   ← EL MISMO que Belén
  └── DetalleVentaDatos   recibe  AccesoDatos #1   ← EL MISMO

Pedido HTTP #5000 (tres semanas después)
  └── ...                 recibe  AccesoDatos #1   ← todavía el mismo
```

**1 objeto para siempre, compartido por todos los usuarios a la vez.**

Para acceso a base de datos esto es un **desastre**: si Belén y Juan compran en
el mismo instante, comparten la misma conexión a SQL Server. Se pisan los datos
entre sí y aparecen errores imposibles de reproducir.

Analogía: el **horno de la cocina**. Hay uno solo y lo usa todo el restaurante.
Perfecto para un horno; pésimo para una comanda.

---

### La diferencia que más cuesta ver

Las dos comparaciones ocurren en ejes distintos:

- **Transient vs Scoped** → se diferencian **DENTRO** de un mismo pedido.
  Transient crea uno por cada clase que lo pide; Scoped lo reusa.
- **Scoped vs Singleton** → se diferencian **ENTRE** pedidos distintos.
  Scoped fabrica uno nuevo para cada usuario; Singleton reusa el mismo siempre.

`Scoped` está justo en el medio: **comparte adentro, aísla afuera.**

---

### ¿Se usan Singleton y Transient en la práctica?

Sí, los tres se usan. Y de hecho **ya estás usando un Singleton sin saberlo**:
`IConfiguration` está registrado por ASP.NET como Singleton, porque la
configuración se lee una vez al arrancar y no cambia.

**Singleton — para lo que no cambia o es caro de crear**

| Ejemplo real          | Por qué                                              |
| --------------------- | ---------------------------------------------------- |
| `IConfiguration`      | Se lee una vez al arrancar, no cambia                |
| `ILogger`             | El escritor de logs es uno solo para toda la app     |
| `IMemoryCache`        | Si cada usuario tuviera su propio caché, no sería caché |
| `IHttpClientFactory`  | Administra un pool de conexiones compartido          |

**Transient — para cosas chiquitas y descartables**

| Ejemplo real                    | Por qué                                       |
| ------------------------------- | --------------------------------------------- |
| Validadores (ej. FluentValidation) | Reciben un objeto, devuelven sí/no, se olvidan |
| Mappers / conversores           | Convierten de A a B, no recuerdan nada        |
| Helpers de utilidad             | Cálculos puros                                |

**Scoped — base de datos y todo lo que dependa del usuario actual.**
El caso más famoso: en Entity Framework, `DbContext` **siempre** va `Scoped`.
Misma razón que `AccesoDatos`.

---

### Cómo elegir

| Si el objeto...                                         | Usá         |
| ------------------------------------------------------- | ----------- |
| Toca la base de datos o guarda datos del usuario actual | `Scoped`    |
| No guarda ningún estado y es carísimo de crear          | `Singleton` |
| No guarda ningún estado y es barato de crear            | `Transient` |

> **Estado** = datos que el objeto guarda adentro y que cambian.
> Una conexión abierta a la base es estado. Una función de matemática pura, no.

**Regla práctica para este proyecto:** todas las clases `*Datos` y todos los
`*Service` van con `Scoped`.

---

## 4. Dónde van los registros

`Program.cs` tiene **dos mitades**, separadas por `builder.Build()`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// ══════ MITAD 1: PREPARAR ══════
// Acá van TODOS los AddScoped / AddSingleton / AddTransient
builder.Services.AddControllers();
builder.Services.AddScoped<AccesoDatos>();
builder.Services.AddScoped<categoriaDatos>();
builder.Services.AddScoped<DetalleVentaDatos>();

var app = builder.Build();   // ← el libro de recetas se CIERRA acá

// ══════ MITAD 2: ATENDER ══════
// Acá va el pipeline. Ya no se pueden registrar servicios.
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();   // ← bloquea. Nada escrito después de esta línea se ejecuta.
```

⚠️ Un `AddScoped` escrito después de `app.Run()` **compila igual pero nunca se
ejecuta**. Es como escribir la lista del súper cuando ya volviste de comprar.

---

## 5. Errores típicos

### `Unable to resolve service for type 'X'`

Te olvidaste de registrarlo. Traducción: *"me pidieron esto y no tengo la receta"*.

Ojo con el momento en que aparece: la app **arranca perfecto** y explota recién
cuando llega el primer pedido HTTP que necesita esa clase.

**Solución:** agregar `builder.Services.AddScoped<X>();` antes de `builder.Build()`.

---

### `CS7036: No se ha dado ningún argumento que corresponda al parámetro requerido`

Quedó un `new X()` en algún lado, pero `X` ahora tiene un constructor que pide
argumentos.

**Solución:** borrar el `new` y pedir la dependencia por constructor.
Este error viaja **en cadena**: al ponerle constructor a `AccesoDatos` se rompen
las clases `*Datos`, y al arreglar esas se rompen los `*Service`. Es normal — el
dominó sube un piso por vez.

---

### `Cannot consume scoped service from singleton`

Registraste algo como `Singleton` y ese algo necesita un `Scoped` adentro.

Tiene sentido: el Singleton vive para siempre, el Scoped muere al terminar el
pedido. El Singleton quedaría agarrado de un objeto muerto.

**Regla:** un servicio nunca puede depender de otro de vida más corta.

```
Singleton  ✗ no puede usar → Scoped o Transient
Scoped     ✓ puede usar    → Scoped, Transient, Singleton
Transient  ✓ puede usar    → todo
```

---

## 6. Chuleta rápida

```csharp
// Registrar (MITAD 1, antes de builder.Build())
builder.Services.AddScoped<MiClase>();

// Pedir (en cualquier clase creada por .NET)
public class OtraClase
{
    private readonly MiClase dependencia;

    public OtraClase(MiClase dependencia)   // ← el constructor pide
    {
        this.dependencia = dependencia;
    }

    public void UnMetodo()
    {
        dependencia.HacerAlgo();            // ← los métodos usan
    }
}
```

**Las tres reglas de oro:**

1. **El constructor pide, los métodos usan.** .NET solo inyecta en constructores.
2. **.NET solo inyecta en objetos que .NET crea.** Si escribís `new` a mano, no
   hay inyección. Por eso hay que borrar todos los `new`.
3. **Todo registro va antes de `builder.Build()`.**

Los controllers son la excepción a la regla de registrar: `AddControllers()` ya
se encarga de ellos.
