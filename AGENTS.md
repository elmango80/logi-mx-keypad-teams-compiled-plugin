# AGENTS.md — Logi Teams Plugin (compiled C#)

> Guía de contexto para agentes de IA (y humanos) que trabajen en este repositorio.
> Contiene TODO lo necesario para entender Logi Options+, sus plugins, el SDK y cómo
> está (o estará) estructurado este proyecto. Léelo entero antes de tocar código.

---

## 1. Objetivo del proyecto

Construir un **plugin COMPILADO (C#)** para **Microsoft Teams** que se use con el
**MX Keypad** (a través de **Logi Options+**), con dos tipos de acción:

1. **Comandos estáticos** (pulsación única → envían un atajo de teclado). Portados desde
   un plugin "virtual" previo que ya funcionaba.
2. **Comandos multiestado** (conmutador de dos estados con iconos distintos) para
   **cámara** (on/off) y **micrófono** (on/off) — el efecto "encender/apagar" con icono
   que cambia. Esto es lo que motivó pasar de plugin virtual a compilado.

Motivo del cambio a compilado: **el formato de plugin "virtual" (JSON) NO soporta
acciones multiestado**. El multiestado solo existe en el SDK compilado
(`PluginMultistateDynamicCommand`).

---

## 2. Cómo funciona el ecosistema Logi Options+ / Loupedeck

Logi Options+ está construido sobre la tecnología de **Loupedeck** (por eso muchos
componentes internos se llaman `Loupedeck*` y el copyright dice "LoupeDeck Oy").

Componentes:

- **Host Application**: la app de escritorio (Logi Options+ o Loupedeck). UI donde el
  usuario instala plugins, configura acciones y las mapea a controles del dispositivo.
  Es una app **Electron**.
- **Logi Plugin Service (LPS)**: servicio en segundo plano que gestiona el ciclo de vida
  de los plugins, ejecuta acciones, almacena perfiles y mapea controles del dispositivo.
  Es el proceso que **carga los plugins**. Runtime **.NET/Mono**.
- **Plugins**: extensiones que aportan acciones. Se integran con apps externas.
- **Profiles**: mapean acciones a controles para una app/flujo concreto. Se guardan
  localmente.
- **Marketplace**: `marketplace.logi.com` — distribución oficial de plugins/perfiles.

### Concepto clave: dos capas separadas

| Capa | Dónde vive | Qué guarda |
|------|-----------|------------|
| **UI (Options+)** | `~/Library/Application Support/LogiOptionsPlus/` | Estado de la app: catálogo de apps detectadas, tema, macros, firmware. `settings.db` (SQLite con un único BLOB JSON). |
| **Servicio (LPS)** | `~/Library/Application Support/Logi/LogiPluginService/` | Plugins y perfiles reales del keypad. Descubre perfiles/plugins **escaneando carpetas al arrancar**. |

### DeviceType interno

Los dispositivos se identifican con `Loupedeck70/71/72` (variantes de hardware). El
activo en este equipo es **`Loupedeck70`** (el MX Keypad).

---

## 3. Rutas importantes en disco (macOS)

```
~/Library/Application Support/Logi/LogiPluginService/
├── Plugins/                         # plugins instalados (uno por carpeta)
│   ├── Photoshop/                   # plugin NATIVO (.dll)
│   ├── Figma-EBC79978BC7445FC/      # plugin VIRTUAL (JSON)
│   └── <TuPlugin>.link              # en desarrollo: apunta al build output
├── Applications/                    # perfiles por dispositivo y app
│   └── Loupedeck70/
│       ├── @_defaultmac/            # perfil del sistema (catálogo)
│       ├── @_photoshop/             # perfil de app de catálogo (slug @_)
│       └── com.microsoft.teams2/    # perfil de app "custom" (bundle id en minúsculas)
├── Media/Icons/DefaultIcons/        # iconos SVG por defecto
├── LoupedeckSettings.ini            # idioma, app activa por dispositivo, etc.
└── Logs/plugin_logs/                # logs por plugin (clave para depurar carga)

~/Library/Application Support/LogiOptionsPlus/
├── settings.db                      # SQLite: 1 fila, BLOB JSON con todo el estado UI
├── cc_config.json / config.json     # config app (idioma, dispositivo activo, tema)
└── icon_cache/                      # PNG cacheados por hash

/Library/Application Support/Logi/LogiOptionsPlus/depots/863393/   # (root) paquetes descargados
```

### Cómo se activa un perfil al enfocar una app
El LPS compara el **bundle id** (macOS) o **process name** (Windows) de la app en primer
plano contra el `processOrBundleName` / patrones del plugin. En macOS, Teams (nuevo) es
**`com.microsoft.teams2`** (el clásico era `com.microsoft.teams`).

---

## 4. Tipos de plugin

| Tipo | Contenido | Multiestado | Cómo se crea |
|------|-----------|-------------|--------------|
| **Nativo / compilado** | `.dll` (SDK .NET) o proceso Node.js | **Sí** | SDK C# o Node.js |
| **Virtual** | Solo `actions.json` + iconos + manifiesto | **No** | Editando JSON a mano |

Este proyecto es **compilado en C#** (el virtual no permite multiestado).

### Por qué C# y no Node.js
- El multiestado (`PluginMultistateDynamicCommand`) está soportado y documentado **solo
  en C#**.
- El SDK de Node.js está en **beta** con un set de funciones **limitado**.
- Arquitectura: C# = DLL que corre **dentro** del LPS; Node.js = proceso aparte por IPC.

---

## 5. Requisitos de desarrollo (macOS)

Qué hace falta y para qué sirve cada pieza:

1. **Logi Options+** (host app). Al instalarlo se instala también el **LogiPluginService**,
   que es quien carga y ejecuta el plugin. Sin esto no hay dónde probar.
   - Descarga: https://www.logitech.com/software/logi-options-plus.html
2. **.NET 8 SDK** (runtime + compilador de C#). El plugin es un `.dll` .NET.
   - Descarga: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verificar: `dotnet --version`  → debe empezar por `8.`
3. **Logi Plugin Tool** (`LogiPluginTool`): CLI oficial para **generar** el proyecto,
   **empaquetar** (`.lplug4`) y **verificar** paquetes.
   ```bash
   dotnet tool install --global LogiPluginTool
   ```
   - Verificar: `logiplugintool --help`
   - Si el comando no se encuentra, añade la carpeta de tools de .NET al PATH:
     `export PATH="$PATH:$HOME/.dotnet/tools"`
4. Editor: VS Code, Visual Studio 2022 o Rider.

> **Ojo — no hay un "SDK de Logi" que descargar aparte.** El SDK de C# se entrega como
> **paquete NuGet** (`PluginApi`, ya referenciado por el proyecto que genera el
> `LogiPluginTool`), y se restaura solo al hacer `dotnet build`. Es decir: instalas
> **LogiPluginTool** + **.NET 8**, generas el proyecto, y el SDK llega vía NuGet.

---

## 6. Estructura del proyecto (objetivo)

El proyecto se generará con:
```bash
logiplugintool generate MicrosoftTeamsControls
```
Esto crea la carpeta `MicrosoftTeamsControlsPlugin/`.

> **Regla del `name`**: solo `[a-zA-Z0-9_-]`, **no puede terminar en "Plugin"**, y es el
> **ID permanente** una vez publicado (no reutilizar el nombre del plugin virtual previo
> `MicrosoftTeams-BB539293FA49494A`).

Estructura esperada del paquete:

```
MicrosoftTeamsControlsPlugin/
├── src/                                  # código C#
│   ├── MicrosoftTeamsControlsPlugin.cs   # clase Plugin (lógica global)
│   ├── MicrosoftTeamsControlsApplication.cs  # ClientApplication (enlace a Teams)
│   ├── PluginResources.cs                # helper para leer recursos embebidos
│   ├── commands/
│   │   ├── OpenChatCommand.cs            # estáticos (PluginDynamicCommand)
│   │   ├── ShareScreenCommand.cs
│   │   ├── RaiseHandCommand.cs
│   │   ├── ToggleCameraCommand.cs        # multiestado (PluginMultistateDynamicCommand)
│   │   └── ToggleMicCommand.cs
│   └── EmbeddedResources/                # PNG 80x80 de iconos de estado
│       ├── CameraOn.png / CameraOff.png
│       └── MicOn.png   / MicOff.png
├── metadata/
│   ├── LoupedeckPackage.yaml             # manifiesto (obligatorio)
│   └── Icon256x256.png                   # icono del plugin
├── actionicons/                          # (opcional) icono por acción, nombre = clase completa
├── actionsymbols/                        # (opcional) SVG pequeño en el selector de acciones
├── icontemplates/                        # (opcional) .ict por acción, nombre = clase completa
└── localization/                         # (opcional) XLIFF por idioma
    └── MicrosoftTeamsControls_es-ES.xliff
```

### Clases núcleo (obligatorias)
- `{PluginName}Plugin : Plugin` — lógica global. Inicializa `PluginLog` y `PluginResources`.
- `{PluginName}Application : ClientApplication` — enlace con la app cliente.

---

## 7. `LoupedeckPackage.yaml` (manifiesto)

Campos obligatorios: `type: plugin4`, `name`, `displayName`, `version`, `author`,
`supportPageUrl`, `license`, `licenseUrl`. Para compilado, además `pluginFileName`,
`pluginFolderMac` (y `pluginFolderWin` si se soporta Windows).

Ejemplo objetivo (macOS; añadir Windows si se decide):
```yaml
type: plugin4
name: MicrosoftTeamsControls
displayName: Microsoft Teams Controls
version: 1.0.0
author: elmango80
copyright: elmango80
supportedDevices:
  - LoupedeckCt
  - LoupedeckLive
pluginFileName: MicrosoftTeamsControls.dll
pluginFolderMac: bin/mac/
license: MIT
licenseUrl: https://opensource.org/licenses/MIT
minimumLoupedeckVersion: "6.1"
```
> `backgroundColor`/`foregroundColor`/`textColor` son ARGB opcionales. Licencias GPL **no**
> son compatibles con el Marketplace; MIT sí.

---

## 8. Enlazar el plugin a Teams

En la clase `ClientApplication`:
```csharp
protected override String GetProcessName() => "ms-teams";            // Windows
protected override String GetBundleName()  => "com.microsoft.teams2"; // macOS (Teams nuevo)
```
Notas:
- Varios nombres: override `GetProcessNames()` (array).
- Filtro por patrón: override `IsProcessNameSupported(String)`.

En la clase `Plugin`, `HasNoApplication` debe ser **`false`** (este plugin SÍ tiene app).

---

## 9. Comandos a implementar

### 9.1 Estáticos (`PluginDynamicCommand`)
Envían un atajo en `RunCommand` con
`this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode, ModifierKey)`.
`ModifierKey` es multiplataforma: en macOS `ModifierKey.Control` actúa como **Cmd**
(convención "ControlOrCommand"). **Verificar en Mac** que el atajo llega correcto.

| Comando | displayName | Tecla | Modificadores |
|---------|-------------|-------|---------------|
| OpenChatCommand | Open Chat | `KeyN` | Control+Shift |
| ShareScreenCommand | Share Screen | `KeyE` | Control+Shift |
| RaiseHandCommand | Raise Hand | `KeyK` | Control+Shift |

(Atajos tomados del plugin virtual previo; ajustar si Teams usa otros en tu versión.)

Plantilla:
```csharp
public class OpenChatCommand : PluginDynamicCommand
{
    public OpenChatCommand()
        : base(displayName: "Open Chat", description: "Open the Teams chat panel", groupName: "Teams") { }

    protected override void RunCommand(String actionParameter) =>
        this.Plugin.ClientApplication.SendKeyboardShortcut(
            VirtualKeyCode.KeyN, ModifierKey.Control | ModifierKey.Shift);
}
```

### 9.2 Multiestado (`PluginMultistateDynamicCommand`) — cámara y micro
Cada estado tiene nombre, descripción, imagen y color de LED. `AddState()` en el
constructor (0-based, número fijo). `ToggleCurrentState()` alterna (solo 2 estados).
`GetCommandImage(..., deviceState, ...)` devuelve el icono por estado.

| Comando | displayName | Tecla | Modificadores | Estados |
|---------|-------------|-------|---------------|---------|
| ToggleCameraCommand | Camera On/Off | `KeyO` | Control+Shift | On(0)/Off(1) |
| ToggleMicCommand | Microphone On/Off | `KeyM` | Control+Shift | On(0)/Off(1) |

Plantilla:
```csharp
public class ToggleCameraCommand : PluginMultistateDynamicCommand
{
    private readonly String _imgOn, _imgOff;

    public ToggleCameraCommand() : base("Camera On/Off", "Turn your camera on or off", "Teams")
    {
        this.AddState("On",  "Camera is on");
        this.AddState("Off", "Camera is off");
        this._imgOn  = PluginResources.FindFile("CameraOn.png");
        this._imgOff = PluginResources.FindFile("CameraOff.png");
    }

    protected override void RunCommand(String actionParameter)
    {
        this.Plugin.ClientApplication.SendKeyboardShortcut(
            VirtualKeyCode.KeyO, ModifierKey.Control | ModifierKey.Shift);
        this.ToggleCurrentState(actionParameter);
    }

    protected override BitmapImage GetCommandImage(String actionParameter, Int32 deviceState, PluginImageSize imageSize)
        => PluginResources.ReadImage(deviceState == 0 ? this._imgOn : this._imgOff);
}
```

> **Limitación conocida (importante):** un plugin de atajos **no sabe el estado real** de la
> cámara/micro en Teams (a menos que use una API de Teams). El toggle es "óptico": alterna
> el icono en cada pulsación asumiendo un estado inicial. Si se desincroniza, una pulsación
> extra realinea. Es la misma limitación que el macro multiestado del perfil.

---

## 10. Iconos y localización

- **Imágenes de estado**: PNG **80x80**, añadidas como **Embedded Resource**. Se leen con
  `PluginResources.FindFile()` / `ReadImage()`.
- **`actionicons/`**: icono de botón por acción; nombre = **nombre completo de la clase**
  (p. ej. `Loupedeck.MicrosoftTeamsControlsPlugin.OpenChatCommand.svg`). Alternativa:
  override `GetCommandImage`.
- **`actionsymbols/`**: SVG pequeño junto al nombre en el selector de acciones.
- **`icontemplates/`**: `.ict` por acción (nombre = clase completa) o
  `metadata/DefaultIconTemplate.ict` para branding por defecto.
- **Localización (XLIFF)**:
  1. Generar: `LogiPluginTool xliff MicrosoftTeamsControls ./` (o deep link
     `loupedeck://plugin/MicrosoftTeamsControls/xliff`). El servicio debe estar corriendo.
  2. Traducir y guardar como `MicrosoftTeamsControls_es-ES.xliff` en `localization/` con
     `target-language="es-ES"`.
  3. Recargar: `loupedeck://plugin/MicrosoftTeamsControls/reload`.
  - **El texto inglés es el ID**: si cambias el inglés, rompes las traducciones (usar un
    XLIFF en→en para renombrar).

---

## 11. Cómo instalar y probar el plugin

Hay **dos formas de instalar**: en desarrollo (automática, vía `.link`) y final (`.lplug4`).

### 11.a Generar el proyecto (una vez)
```bash
cd /Users/n857521/code/elmango80/logi-teams-plugin
logiplugintool generate MicrosoftTeamsControls   # crea MicrosoftTeamsControlsPlugin/
```

### 11.b Instalación en DESARROLLO (recomendada mientras se programa)
Con solo compilar, el plugin queda **auto-instalado** para desarrollo:
```bash
cd MicrosoftTeamsControlsPlugin
dotnet build            # 1) restaura el SDK (NuGet) y compila
                        # 2) crea un archivo .link en la carpeta Plugins del servicio
```
- El **`.link`** aparece en:
  `~/Library/Application Support/Logi/LogiPluginService/Plugins/MicrosoftTeamsControls.link`
  y apunta al directorio de compilación. El servicio carga el plugin **desde ahí** (no hay
  que copiar carpetas a mano).
- **Hot reload** (recompila y recarga al guardar):
  ```bash
  cd src/ && dotnet watch build
  ```
- Verificar que se instaló:
  - En Options+ → vista de personalización del MX Keypad → **All Actions** → debe aparecer
    "Microsoft Teams Controls" bajo **Installed Plugins**.
  - Si NO aparece: Options+ → Ajustes → **"Restart Logi Plugin Service"** (recuerda el
    gotcha de §11.d: el servicio escanea al arrancar).
  - Revisar log de carga:
    `~/Library/Application Support/Logi/LogiPluginService/Logs/plugin_logs/MicrosoftTeamsControls.log`

### 11.c Instalación FINAL (para usar/distribuir, ver §12)
Empaquetar a `.lplug4` e instalar con **doble clic**:
```bash
logiplugintool pack ./bin/Release/ ./MicrosoftTeamsControls_1_0.lplug4
open ./MicrosoftTeamsControls_1_0.lplug4     # lo instala el package installer del servicio
```
> Para producción: quita el `.link` de desarrollo antes de instalar el `.lplug4`, para no
> tener el plugin cargado dos veces.

### 11.d ⚠️ Gotcha CRÍTICO (aprendido en esta sesión)
El **LogiPluginService solo escanea `Plugins/` y las localizaciones AL ARRANCAR**.
Cualquier cambio (nuevo plugin, editar `actions.json`, añadir/editar un `.xliff`) **no se
ve hasta reiniciar el servicio**. Cerrar/abrir solo la ventana de Options+ **no basta**.

Reinicio fiable en macOS (el agente `logioptionsplus_agent` relanza el servicio):
```bash
pkill -9 -f "LogiPluginService"     # SIGKILL: evita que reescriba estado al salir
# el agente lo relanza solo en ~5-10s; si no, abrir Logi Options+
```
Verificar carga en logs:
```
~/Library/Application Support/Logi/LogiPluginService/Logs/plugin_logs/<PluginName>.log
~/Library/Application Support/Logi/LogiPluginService/Logs/plugin_logs/_errors.log
```
Nota: el mensaje `ERROR ... "already loaded"` es **inofensivo** (intento de carga duplicado).

---

## 12. Empaquetado y distribución

```bash
logiplugintool pack ./bin/Release/ ./MicrosoftTeamsControls_1_0.lplug4
logiplugintool verify ./MicrosoftTeamsControls_1_0.lplug4
```
- `.lplug4` = ZIP con estructura concreta + `LoupedeckPackage.yaml`. Se instala con doble clic.
- Marketplace: `marketplace.logitech.com/contribute` (cumplir guías de aprobación; licencia
  compatible como MIT).

---

## 13. Convivencia con lo existente

- Ya existe un plugin **virtual** de Teams (`MicrosoftTeams-BB539293FA49494A`) y un perfil
  del plugin (`@_microsoftteams-bb539293fa49494a`). Cuando el compilado funcione, conviene
  **eliminar el virtual** (borrar su carpeta en `Plugins/` + reiniciar el servicio) para no
  tener dos plugins de Teams.
- El perfil manual `com.microsoft.teams2` ya fue eliminado; la página "Teams" (con las
  macros multiestado de cámara/micro) vive en el perfil del plugin.

---

## 14. Convenciones del repositorio

- **Identidad git = PERSONAL** (configurada solo en local, NO usar la de Santander):
  - `user.name`: Mango Sánchez-Redondo
  - `user.email`: elmango80@gmail.com
  - `user.signingkey`: `~/.ssh/id_ed25519_personal.pub` (firma SSH activada)
- **Rama principal**: `master`.
- **Remoto** (cuando se cree en GitHub personal, cuenta `elmango80`), usar el alias SSH
  personal para no usar la clave de Santander:
  ```bash
  git remote add origin git@github.com-personal:elmango80/logi-teams-plugin.git
  ```
- `.gitignore` ya ignora `bin/`, `obj/`, `*.lplug4`, `*.link`, `.DS_Store`, IDE.

---

## 15. Referencias

- Documentación oficial SDK: https://logitech.github.io/actions-sdk-docs/
  - Introducción C#: `/csharp/plugin-development/introduction`
  - Estructura y `LoupedeckPackage.yaml`: `/csharp/tutorial/plugin-structure`
  - Comando simple: `/csharp/tutorial/add-a-simple-command`
  - Enlazar a app: `/csharp/tutorial/link-the-plugin-to-an-application`
  - **Multiestado**: `/csharp/plugin-features/multistate-plugin-actions`
  - Imagen por estado: `/csharp/tutorial/change-a-button-image`
  - Localización: `/csharp/plugin-features/plugin-localization`
  - Empaquetado: `/csharp/plugin-development/distributing-the-plugin`
- Repo oficial (DemoPlugin): https://github.com/Logitech/actions-sdk
- Ingeniería inversa del formato en disco (comunidad):
  https://github.com/ssurmacz2-arch/logi-options-profiles
- Marketplace: https://marketplace.logi.com

---

## 16. Estado actual del proyecto

- [x] Repositorio git inicializado (rama `master`, identidad personal, `.gitignore`).
- [ ] Instalar .NET 8 SDK + LogiPluginTool.
- [ ] Generar esqueleto (`logiplugintool generate MicrosoftTeamsControls`).
- [ ] Implementar `Application` (enlace a `com.microsoft.teams2`).
- [ ] Comandos estáticos (Open Chat, Share Screen, Raise Hand).
- [ ] Comandos multiestado (cámara, micro) + iconos PNG 80x80.
- [ ] Localización es-ES.
- [ ] Probar con el MX Keypad y empaquetar `.lplug4`.
- [ ] Eliminar el plugin virtual previo.
