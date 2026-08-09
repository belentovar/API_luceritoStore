# Guía de Git desde cero

> Guía pensada para empezar sin saber nada de Git.
> Primero los conceptos (para entender **qué** pasa), después los comandos
> (para poder **trabajar**), tanto sola como en equipo.

---

## Parte 1 — Los conceptos (leé esto primero)

### 1.1 ¿Qué es Git?

Git es un sistema que saca **fotos** de tu proyecto en el tiempo. Cada foto guarda
el estado completo de tus archivos en un momento dado. A esas fotos las llamamos
**commits**.

Pensalo como una máquina del tiempo:

- Cada vez que terminás algo, sacás una foto (**commit**).
- Si más adelante algo se rompe, podés volver a cualquier foto anterior.
- Nunca perdés trabajo, porque cada foto queda guardada para siempre.

Sin Git, la única forma de "guardar versiones" es tener carpetas como
`proyecto_final`, `proyecto_final_v2`, `proyecto_final_AHORA_SI`. Git resuelve
justamente ese caos.

### 1.2 Git NO es GitHub

Esta confusión es la más común, así que prestá atención:

| | Qué es | Dónde vive |
|---|---|---|
| **Git** | El programa que saca las fotos (commits) | En tu computadora |
| **GitHub** | Una página web donde subís esas fotos para guardarlas y compartirlas | En internet (la nube) |

Analogía: **Git** es la cámara de fotos. **GitHub** es Google Fotos donde subís
el álbum para tenerlo respaldado y compartirlo con otros.

Podés usar Git sin GitHub (solo en tu máquina). Pero para trabajar en equipo o
para usar tu código en otra computadora, subís las fotos a GitHub.

### 1.3 Los tres lugares donde vive tu código

Este es EL concepto que hay que entender. Tu código pasa por tres zonas:

```
   Tu carpeta            Zona de preparación         Repositorio
 (working tree)             (staging)              (historial de commits)
       |                        |                          |
       |   git add archivo      |      git commit          |
       |----------------------->|------------------------->|
       |                        |                          |
   Editás acá            "Estos cambios van          Foto guardada
                          en la próxima foto"          para siempre
```

1. **Working tree (tu carpeta):** donde editás los archivos normalmente.
2. **Staging (zona de preparación):** donde marcás QUÉ cambios querés incluir en
   la próxima foto. Sirve para no meter todo junto: podés elegir.
3. **Repositorio (historial):** donde quedan guardadas las fotos (commits).

El comando `git add` mueve cambios de la carpeta al staging.
El comando `git commit` toma lo que hay en staging y saca la foto.

### 1.4 Local vs remoto

- **Local** = tu computadora.
- **Remoto** (lo llamamos `origin`) = GitHub.

Tus commits primero se guardan **local**. Recién cuando hacés `git push` viajan al
**remoto** (GitHub). Y `git pull` trae al local lo que otros subieron al remoto.

```
   TU COMPUTADORA (local)                      GITHUB (remoto = origin)
   ┌─────────────────────┐                     ┌─────────────────────┐
   │  commits locales    │  ── git push ──>    │  commits en la nube │
   │                     │  <── git pull ──    │                     │
   └─────────────────────┘                     └─────────────────────┘
```

### 1.5 ¿Qué es una rama (branch)?

Una **rama** es una línea de trabajo independiente. La rama principal se suele
llamar `main` (antes `master`).

Imaginá que estás escribiendo un libro. La versión "oficial" es `main`. Si querés
probar un capítulo nuevo sin arruinar lo que ya funciona, creás una rama aparte,
trabajás ahí, y cuando está listo lo unís (**merge**) a `main`.

Trabajando sola al principio podés vivir toda tu vida en `main`. Pero en equipo,
las ramas son fundamentales: cada persona trabaja en su rama y después se unen.

---

## Parte 2 — Configuración inicial (se hace una sola vez por computadora)

Antes de usar Git, decile quién sos. Esto queda en cada foto que saques.

```bash
git config --global user.name "Belen Tovar"
git config --global user.email "tu-email@ejemplo.com"
```

> Usá el mismo email de tu cuenta de GitHub para que los commits te queden
> atribuidos allá.

Para verificar que quedó bien:

```bash
git config --global user.name
git config --global user.email
```

---

## Parte 3 — Trabajando sola (el flujo del día a día)

Este es el 90% de lo que vas a usar. Memorizá este ciclo:

### 3.1 Ver en qué estado estás

```bash
git status
```

Te dice qué archivos cambiaste, cuáles están en staging y cuáles no. **Ante la
duda, siempre `git status`.** Es tu brújula, no rompe nada, usalo todo el tiempo.

### 3.2 El ciclo básico: editar → add → commit → push

```bash
# 1. Editás tus archivos normalmente (en tu editor / Visual Studio)

# 2. Marcás qué cambios entran en la foto
git add .                    # el punto significa "todos los cambios"

# 3. Sacás la foto con un mensaje que describe qué hiciste
git commit -m "Agrego validacion de stock en la venta"

# 4. Subís la foto a GitHub
git push
```

Eso es todo. Ese ciclo repetido es trabajar con Git.

### 3.3 ¿Cada cuánto hacer commit?

Cada vez que terminás **una cosa** que funciona. No esperes al final del día.
Un commit por tarea completada. Ejemplos de buen momento para commitear:

- Terminaste una función y anda.
- Arreglaste un bug.
- Agregaste una validación.

Muchos commits chiquitos y claros > un commit gigante con todo mezclado.

### 3.4 Cómo escribir un buen mensaje de commit

El mensaje explica QUÉ cambiaste y POR QUÉ. En presente, corto y claro.

```bash
# BIEN
git commit -m "Arreglo fuga de conexion en favoritos"
git commit -m "Agrego endpoint para listar productos"

# MAL
git commit -m "cambios"
git commit -m "asdasd"
git commit -m "arregle cosas"
```

Dentro de seis meses, esos mensajes son tu única forma de entender qué pasó.

---

## Parte 4 — Trabajando en equipo

Acá se suma UNA regla de oro y un concepto nuevo (conflictos).

### 4.1 La regla de oro del equipo

> **Antes de empezar a trabajar, siempre `git pull`.**

¿Por qué? Porque mientras vos no estabas, tu compañera pudo haber subido cambios.
Si empezás a trabajar sin traer eso primero, después chocás. `git pull` trae lo
último de GitHub a tu máquina antes de que empieces.

```bash
git pull        # traé lo último ANTES de trabajar
# ... trabajás, hacés tus commits ...
git push        # subí lo tuyo al terminar
```

### 4.2 Clonar un proyecto que ya existe

Para bajar a tu computadora un repo que ya está en GitHub (por ejemplo, en tu otra
PC o cuando te sumás a un proyecto):

```bash
git clone https://github.com/belentovar/API_luceritoStore.git
```

Eso crea la carpeta con todo el código y todo el historial. Se hace **una sola
vez** por máquina. Después usás `pull` y `push` normalmente.

### 4.3 Trabajar con ramas (lo profesional en equipo)

En equipo, no se trabaja directo en `main`. Cada tarea va en su propia rama:

```bash
# 1. Partís siempre desde main actualizado
git checkout main
git pull

# 2. Creás una rama nueva para tu tarea
git checkout -b agregar-login

# 3. Trabajás y commiteás normalmente en esa rama
git add .
git commit -m "Agrego pantalla de login"

# 4. Subís tu rama a GitHub
git push -u origin agregar-login
```

Después, en GitHub, abrís un **Pull Request** (PR): es un pedido para unir tu rama
a `main`, donde tu equipo puede revisar el código antes de mezclarlo. Cuando lo
aprueban, se hace el merge y tu trabajo entra en `main`.

Comandos útiles con ramas:

```bash
git branch                   # lista tus ramas (la actual tiene un *)
git checkout main            # te movés a la rama main
git checkout agregar-login   # te movés a otra rama
```

### 4.4 Conflictos (no les tengas miedo)

Un **conflicto** pasa cuando vos y otra persona editaron **la misma línea del
mismo archivo** de forma distinta. Git no sabe cuál elegir, así que te pregunta.

Cuando pasa, Git marca el archivo así:

```
<<<<<<< HEAD
    tu version de la linea
=======
    la version de tu compañera
>>>>>>> main
```

Para resolverlo:

1. Abrís el archivo y buscás esas marcas.
2. Decidís qué código queda (el tuyo, el de ella, o una mezcla).
3. Borrás las marcas `<<<<<<<`, `=======`, `>>>>>>>`.
4. Guardás, y hacés `git add` + `git commit`.

No es magia negra: es Git diciendo "decidí vos, yo no me quiero equivocar".

---

## Parte 5 — Comandos para cuando algo sale mal

Guardá esta sección para el día que entres en pánico. Respirá, no rompiste nada.

```bash
# Ver el historial de commits (fotos)
git log --oneline

# Descartar TODOS los cambios sin commitear y volver a la ultima foto
git checkout .
# (cuidado: esto borra los cambios que no commiteaste)

# Descartar los cambios de UN archivo puntual
git checkout -- nombre-del-archivo

# Sacar un archivo del staging (pero conservar los cambios)
git restore --staged nombre-del-archivo

# Volver todo el proyecto a una foto vieja (usá el codigo del git log)
git reset --hard abc1234
# (cuidado: descarta lo posterior a esa foto)
```

> Regla de seguridad: los comandos con `--hard` y `checkout .` **borran** cambios
> no guardados. Si tenés dudas, primero `git status` y `git log`, que no rompen nada.

---

## Parte 6 — Cosas que NO se suben a Git

No todo va al repositorio. Dos categorías nunca se suben:

1. **Archivos generados automáticamente** (compilaciones): en .NET son las carpetas
   `bin/` y `obj/`. Se regeneran solos al compilar, no tiene sentido versionarlos.
2. **Secretos** (claves, contraseñas, tokens): NUNCA van en el repo. Si subís una
   clave a GitHub, queda en el historial para siempre. Para eso existen los
   *User Secrets* y las variables de entorno.

Para decirle a Git qué ignorar, se usa un archivo llamado `.gitignore`. Cada línea
es un patrón de lo que Git debe ignorar. Ejemplo del que tiene este proyecto:

```
/.vs
[Bb]in/
[Oo]bj/
*.user
```

---

## Parte 7 — Machete rápido (para tener a mano)

```bash
# --- CONFIGURACION (una vez) ---
git config --global user.name "Tu Nombre"
git config --global user.email "tu@email.com"

# --- EMPEZAR ---
git clone <url>          # bajar un repo que ya existe
git init                 # crear un repo nuevo en una carpeta

# --- EL DIA A DIA (sola) ---
git status               # ¿en que estado estoy? (usalo siempre)
git add .                # preparar todos los cambios
git commit -m "mensaje"  # sacar la foto
git push                 # subir a GitHub
git pull                 # bajar de GitHub

# --- EN EQUIPO ---
git pull                 # SIEMPRE antes de empezar a trabajar
git checkout -b mi-rama  # crear y moverse a una rama nueva
git checkout main        # volver a la rama principal
git branch               # ver mis ramas

# --- MIRAR / DESHACER ---
git log --oneline        # ver el historial
git checkout .           # descartar cambios sin commitear (cuidado)
```

---

## Parte 8 — El flujo mínimo que tenés que memorizar

Si te olvidás de todo lo demás, memorizá esto:

**Trabajando sola:**
```
git pull  →  (trabajar)  →  git add .  →  git commit -m "..."  →  git push
```

**Sumándote a algo que ya existe:**
```
git clone <url>  →  (después, el flujo de arriba)
```

Con eso ya podés trabajar. El resto lo vas incorporando con la práctica. No hace
falta que sepas todo hoy: Git se aprende usándolo, no memorizándolo.
