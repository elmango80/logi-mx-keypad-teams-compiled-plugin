# Migración a Plugin Virtual (Microsoft Teams) — Plan de Implementación

> **For Claude:** REQUIRED SUB-SKILL: Use executing-plans para implementar este plan tarea a tarea.

**Goal:** Recrear el plugin de Microsoft Teams para el MX Keypad como **plugin virtual** (solo `actions.json` + iconos + manifiesto), sin C#, sin .NET y sin DLL, en un repo nuevo.

**Architecture:** Un plugin virtual de Logi/Loupedeck es una carpeta con `metadata/LoupedeckPackage.yaml` (`pluginType: virtual`), `virtual/actions.json` (lista de comandos con atajos multiplataforma) y carpetas de iconos (`actionicons/`, `actionsymbols/`) cuyos ficheros se nombran por el `fullName` de cada acción (`VirtualCommand___<HEX>.svg`). El Logi Plugin Service descubre el plugin escaneando `Plugins/` al arrancar. Los tres antiguos comandos multiestado (cámara, micrófono, compartir) se degradan a **acciones simples** que conservan el icono del estado activo.

**Tech Stack:** JSON, YAML, SVG. Sin toolchain de compilación. Verificación mediante `python3 -m json.tool`, reinicio del Logi Plugin Service y comprobación visual en Logi Options+.

**Repo destino:** `logi-mx-keypad-teams-virtual-plugin` (cuenta personal `elmango80`, remoto SSH `github.com-personal`). El repo compilado (`logi-mx-keypad-teams-compiled-plugin`) queda archivado como referencia; **no se toca**.

---

## Contexto imprescindible (leer antes de empezar)

### Referencia canónica del formato virtual

La referencia fiable es el plugin **Figma** instalado en:
```
~/Library/Application Support/Logi/LogiPluginService/Plugins/Figma-EBC79978BC7445FC/
```
Estructura observada:
```
Figma-EBC79978BC7445FC/
├── metadata/
│   ├── LoupedeckPackage.yaml      # pluginType: virtual
│   ├── Icon256x256.png
│   └── PackageHash.bin            # lo genera el servicio; NO se crea a mano
├── virtual/
│   └── actions.json               # languages, groupNames, commands[]
├── actionicons/                   # VirtualCommand___<HEX>.svg  (icono del botón)
├── actionsymbols/                 # VirtualCommand___<HEX>.svg  (símbolo del selector)
├── localization/                  # <name>_<locale>.xliff (opcional)
└── profiles/                      # DefaultProfile70/71/72.lp5 (opcional; NO lo generamos)
```

### `LoupedeckPackage.yaml` virtual (campos reales observados en Figma)
```yaml
type: plugin4
name: Figma-EBC79978BC7445FC
displayName: Figma
version: 1.0.0.54
description: ...
icon256x256: Icon256x256.png
minimumLoupedeckVersion: 6.1
pluginType: virtual                # <-- clave que lo distingue del compilado
pluginCapabilities:
- HasApplication
- ActivatesApplication
backgroundColor: 4286600437
applicationPatterns:
  processNamePattern: ^figma$
  bundleNamePattern: ^com.figma.Desktop$
  executablePathPattern: \\Figma.exe$
```

### `actions.json` (estructura real)
```json
{
    "languages": ["en-US"],
    "primaryOperatingSystem": "Win",
    "secondaryOperatingSystem": "Mac",
    "groupNames": ["..."],
    "commands": [
        {
            "fullName": "VirtualCommand___<HEX>",
            "shortcuts": { "en-US": "ControlOrCommand+Shift+O" },
            "name": "<HEX>",
            "displayName": "Texto visible",
            "description": "Descripción",
            "groupName": "Reunión",
            "operatingSystem": 3
        }
    ]
}
```

### Tokens de atajo multiplataforma (confirmados en Figma)
- `ControlOrCommand` → Ctrl en Windows, Cmd en macOS (equivale a `ModifierKey.ControlOrCommand` del compilado).
- `AltOrOption` → Alt/Option (equivale a `ModifierKey.AltOrOption`). También aparece `AltOrOpt` / `CtrlOrCmd` como alias.
- `Shift` → Shift.
- Teclas: letras sueltas (`O`, `N`), dígitos (`1`), o nombres OEM (`Oem5` = `\` en teclado US).
- Un único atajo con `"operatingSystem": 3` cubre Windows y macOS. **No hay que duplicar por SO.**

### Convención de iconos
- El fichero se nombra por el `fullName` completo: `VirtualCommand___<HEX>.svg`.
- `actionicons/` = icono del botón en el keypad.
- `actionsymbols/` = símbolo pequeño en el selector de acciones.

### Enlace del plugin a Teams (macOS/Windows)
Equivalente a `com.microsoft.teams2` (macOS) y `ms-teams` (Windows), vía `applicationPatterns`:
```yaml
applicationPatterns:
  processNamePattern: ^ms-teams$
  bundleNamePattern: ^com\.microsoft\.teams2$
```
> Verificar el proceso real en Windows; en el compilado era `ms-teams`.

### Reinicio del servicio (imprescindible tras cualquier cambio)
```bash
pkill -9 -f "LogiPluginService"   # el agente logioptionsplus_agent lo relanza en ~5-10s
```
Log de carga:
```
~/Library/Application Support/Logi/LogiPluginService/Logs/plugin_logs/MicrosoftTeams.log
```

---

## Tabla de mapeo: compilado → virtual

`name` del plugin: **`MicrosoftTeams`**. Grupos: `Navegación`, `Chat`, `Reunión`, `Ventana`.

| # | fullName (`VirtualCommand___` + HEX) | displayName | Grupo | shortcut (en-US) | Icono a reutilizar |
|---|---|---|---|---|---|
| 1 | `0F94B7AFCC3A4EC89E39A544E8AFF24F` | Actividad | Navegación | `ControlOrCommand+1` | GoToActivityCommand |
| 2 | `033C1F7B7E4D456D97892D55729B1BCE` | Chat | Navegación | `ControlOrCommand+2` | GoToChatCommand |
| 3 | `C836308C6D6A4F51A57005BB8645867B` | Copilot | Navegación | `ControlOrCommand+3` | GoToCopilotCommand |
| 4 | `262C36D0993A45EEB90E391CEEF3DD01` | Calendario | Navegación | `ControlOrCommand+4` | GoToCalendarCommand |
| 5 | `C16913D60554463987230F910B83E896` | Llamadas | Navegación | `ControlOrCommand+5` | GoToCallsCommand |
| 6 | `CA0F6545E00748B592F034114EC6CD42` | OneDrive | Navegación | `ControlOrCommand+6` | GoToOneDriveCommand |
| 7 | `F22BA75C5C3E4C70AF5AF4BCD380F9C2` | Turnos | Navegación | `ControlOrCommand+7` | GoToShiftsCommand |
| 8 | `DA7CDD0EB8E443BCAD151F211210EC0A` | Abrir chat | Chat | `ControlOrCommand+Shift+N` | OpenChatCommand |
| 9 | `A1EBE0B093324CB28F00D4065E7367E0` | Llamada de audio | Chat | `AltOrOption+ControlOrCommand+S` | AudioCallCommand |
| 10 | `BDF04DC05E3F4AE08B825E433CEE0B27` | Videollamada | Chat | `ControlOrCommand+Shift+S` | VideoCallCommand |
| 11 | `D9144AAEFD784166802254B9E2F4ACBA` | Abrir Copilot | Chat | `Control+ControlOrCommand+I` ⚠️ | OpenCopilotCommand |
| 12 | `7FCB69280E5643DD9A45B4803BDD6B39` | Canales | Chat | `AltOrOption+ControlOrCommand+A` | ViewChannelsCommand |
| 13 | `C8309671F75C472D9A34B41D6B416352` | Chats | Chat | `AltOrOption+ControlOrCommand+C` | ViewChatsCommand |
| 14 | `EC9BC84782D64DF993893844A3E201CD` | No leído | Chat | `AltOrOption+ControlOrCommand+U` | (falta icono; ver Task 5) |
| 15 | `4F0289FFB527439689D9F22BE9778443` | Marcar todos como leídos | Chat | `Shift+Escape` | MarkAllAsReadCommand |
| 16 | `613D7A226E5A471FBC9CABC3CACB5F87` | Contraer todas las secciones | Chat | `ControlOrCommand+Shift+L` | CollapseAllSectionsCommand |
| 17 | `70C926FAC21843628E6F26B608B0A01E` | **Encender/Apagar cámara** | Reunión | `ControlOrCommand+Shift+O` | **CameraOn** (estado activo) |
| 18 | `234416A040D148FB9671BF5C7B40A2B3` | **Activar/Silenciar micrófono** | Reunión | `ControlOrCommand+Shift+M` | **MicOn** (estado activo) |
| 19 | `CEF97641BBF24D548CB507F18C50E6D3` | **Compartir/Dejar de compartir contenido** | Reunión | `ControlOrCommand+Shift+E` | **ShareOn** (recuadro con flecha) |
| 20 | `7D35DFB5B24648C0A51505FA6E37411B` | Levantar la mano | Reunión | `ControlOrCommand+Shift+K` | RaiseHandCommand |
| 21 | `95DFA17B3AB64811A9D8CA55911EE47A` | Salir | Reunión | `ControlOrCommand+Shift+H` | LeaveCommand |
| 22 | `D166AECAE2FE4EBEA56F038D649AA045` | Contraer barra de aplicaciones | Ventana | `ControlOrCommand+Oem5` | CollapseAppBarCommand (falta icono; ver Task 5) |

### ⚠️ Notas de riesgo sobre atajos
- **#11 Abrir Copilot** (`Ctrl+Cmd+I` en el compilado, vía `ControlOrCommand | Control`): es un atajo raro (Ctrl **y** Cmd en Mac; en Windows quedaría solo Ctrl). El token `Control` literal **no está verificado** en el formato virtual. Propuesta tentativa `Control+ControlOrCommand+I`. **Verificar en Mac**; si no funciona, probar `AltOrOption+ControlOrCommand+I` o confirmar el atajo real de Copilot en tu Teams.
- **#22 Contraer barra** (`Cmd+\`): `Oem5` es `\` en teclado US. Si tu teclado no es US, verificar y ajustar (Figma usó `Oem5` para el mismo tipo de tecla).
- **#15 Marcar como leído** (`Shift+Esc`): confirmar que `Escape` es el nombre de tecla aceptado (alternativa `Esc`).
- El **orden** de los modificadores probablemente es irrelevante (Figma mezcla `Shift+ControlOrCommand+O` y `AltOrOption+ControlOrCommand+L`); se estandariza como `AltOrOption+ControlOrCommand+Shift+Tecla`.

### Degradación de multiestado (decisión del usuario)
Los 3 toggles pasan a acción simple: **envían el atajo pero NO cambian de icono**. Conservan el icono del **estado activo** (cámara encendida, micro activo, compartiendo = recuadro con flecha). El `displayName` fusiona ambos estados.

---

## Task 0: Preparar el repo nuevo

**Files:**
- Trabajo en un nuevo directorio local clonado del repo vacío `logi-mx-keypad-teams-virtual-plugin`.

**Prerequisito (usuario):** crear el repo vacío en `github.com/elmango80/logi-mx-keypad-teams-virtual-plugin` (como se hizo con el compilado).

**Step 1: Clonar el repo nuevo (SSH personal)**
```bash
cd ~/code/elmango80
git clone git@github.com-personal:elmango80/logi-mx-keypad-teams-virtual-plugin.git
cd logi-mx-keypad-teams-virtual-plugin
```

**Step 2: Confirmar identidad personal local**
```bash
git config user.name "Mango Sánchez-Redondo"
git config user.email "elmango80@gmail.com"
git config user.signingkey ~/.ssh/id_ed25519_personal.pub
git config gpg.format ssh
git config commit.gpgsign true
```
Expected: sin errores.

**Step 3: Crear estructura de carpetas**
```bash
mkdir -p metadata virtual actionicons actionsymbols docs/plans
```

**Step 4: `.gitignore` mínimo**
Create `.gitignore`:
```gitignore
.DS_Store
metadata/PackageHash.bin
```

**Step 5: Commit inicial**
```bash
git add .gitignore
git commit -m "chore: estructura inicial del plugin virtual"
```

---

## Task 1: Manifiesto `LoupedeckPackage.yaml`

**Files:**
- Create: `metadata/LoupedeckPackage.yaml`
- Copiar: `metadata/Icon256x256.png` (reutilizar el icono del repo compilado si existe; si no, exportar uno 256x256)

**Step 1: Escribir el manifiesto virtual**
Create `metadata/LoupedeckPackage.yaml`:
```yaml
type: plugin4
name: MicrosoftTeams
displayName: Microsoft Teams
version: 1.0.0
description: Atajos de Microsoft Teams para el MX Keypad (plugin virtual)
icon256x256: Icon256x256.png
minimumLoupedeckVersion: 6.1
pluginType: virtual
pluginCapabilities:
- HasApplication
- ActivatesApplication
backgroundColor: 4283456201   # #505AC9
applicationPatterns:
  processNamePattern: ^ms-teams$
  bundleNamePattern: ^com\.microsoft\.teams2$
```

**Step 2: Añadir el icono 256x256**
```bash
# Si existe en el repo compilado:
cp ~/code/elmango80/logi-teams-plugin/metadata/Icon256x256.png metadata/ 2>/dev/null || echo "FALTA: exportar Icon256x256.png"
```
Expected: existe `metadata/Icon256x256.png`. Si no existía en el compilado, exportar uno.

**Step 3: Validar YAML**
```bash
python3 -c "import yaml,sys; yaml.safe_load(open('metadata/LoupedeckPackage.yaml')); print('OK')"
```
Expected: `OK`.

**Step 4: Commit**
```bash
git add metadata/
git commit -m "feat: manifiesto virtual y icono del plugin"
```

---

## Task 2: `actions.json` — comandos de Navegación y Ventana

Construimos `actions.json` de forma incremental para poder validar por bloques. Empezamos por el esqueleto + Navegación + Ventana.

**Files:**
- Create: `virtual/actions.json`

**Step 1: Escribir esqueleto + comandos 1–7 y 22**
Create `virtual/actions.json` (usar los HEX de la tabla de mapeo):
```json
{
    "languages": ["en-US"],
    "primaryOperatingSystem": "Win",
    "secondaryOperatingSystem": "Mac",
    "groupNames": ["Navegación", "Chat", "Reunión", "Ventana"],
    "commands": [
        { "fullName": "VirtualCommand___0F94B7AFCC3A4EC89E39A544E8AFF24F", "name": "0F94B7AFCC3A4EC89E39A544E8AFF24F", "displayName": "Actividad", "description": "Ir a Actividad en Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+1" } },
        { "fullName": "VirtualCommand___033C1F7B7E4D456D97892D55729B1BCE", "name": "033C1F7B7E4D456D97892D55729B1BCE", "displayName": "Chat", "description": "Ir al chat de Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+2" } },
        { "fullName": "VirtualCommand___C836308C6D6A4F51A57005BB8645867B", "name": "C836308C6D6A4F51A57005BB8645867B", "displayName": "Copilot", "description": "Ir a Copilot en Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+3" } },
        { "fullName": "VirtualCommand___262C36D0993A45EEB90E391CEEF3DD01", "name": "262C36D0993A45EEB90E391CEEF3DD01", "displayName": "Calendario", "description": "Ir al calendario de Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+4" } },
        { "fullName": "VirtualCommand___C16913D60554463987230F910B83E896", "name": "C16913D60554463987230F910B83E896", "displayName": "Llamadas", "description": "Ir a Llamadas en Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+5" } },
        { "fullName": "VirtualCommand___CA0F6545E00748B592F034114EC6CD42", "name": "CA0F6545E00748B592F034114EC6CD42", "displayName": "OneDrive", "description": "Ir a OneDrive en Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+6" } },
        { "fullName": "VirtualCommand___F22BA75C5C3E4C70AF5AF4BCD380F9C2", "name": "F22BA75C5C3E4C70AF5AF4BCD380F9C2", "displayName": "Turnos", "description": "Ir a Turnos en Teams", "groupName": "Navegación", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+7" } },
        { "fullName": "VirtualCommand___D166AECAE2FE4EBEA56F038D649AA045", "name": "D166AECAE2FE4EBEA56F038D649AA045", "displayName": "Contraer barra de aplicaciones", "description": "Contraer o expandir la barra de aplicaciones", "groupName": "Ventana", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Oem5" } }
    ]
}
```

**Step 2: Validar JSON**
```bash
python3 -m json.tool virtual/actions.json > /dev/null && echo "JSON OK"
```
Expected: `JSON OK`.

**Step 3: Commit**
```bash
git add virtual/actions.json
git commit -m "feat: actions.json con navegación y ventana"
```

---

## Task 3: `actions.json` — comandos de Chat

**Files:**
- Modify: `virtual/actions.json` (añadir comandos 8–16 al array `commands`)

**Step 1: Insertar los 9 comandos de Chat**
Añadir estos objetos al array `commands` (antes del `]`):
```json
        { "fullName": "VirtualCommand___DA7CDD0EB8E443BCAD151F211210EC0A", "name": "DA7CDD0EB8E443BCAD151F211210EC0A", "displayName": "Abrir chat", "description": "Abrir el panel de chat de Teams", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+N" } },
        { "fullName": "VirtualCommand___A1EBE0B093324CB28F00D4065E7367E0", "name": "A1EBE0B093324CB28F00D4065E7367E0", "displayName": "Llamada de audio", "description": "Iniciar una llamada de audio", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "AltOrOption+ControlOrCommand+S" } },
        { "fullName": "VirtualCommand___BDF04DC05E3F4AE08B825E433CEE0B27", "name": "BDF04DC05E3F4AE08B825E433CEE0B27", "displayName": "Videollamada", "description": "Iniciar una videollamada", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+S" } },
        { "fullName": "VirtualCommand___D9144AAEFD784166802254B9E2F4ACBA", "name": "D9144AAEFD784166802254B9E2F4ACBA", "displayName": "Abrir Copilot", "description": "Abrir Copilot", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "Control+ControlOrCommand+I" } },
        { "fullName": "VirtualCommand___7FCB69280E5643DD9A45B4803BDD6B39", "name": "7FCB69280E5643DD9A45B4803BDD6B39", "displayName": "Canales", "description": "Ver canales", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "AltOrOption+ControlOrCommand+A" } },
        { "fullName": "VirtualCommand___C8309671F75C472D9A34B41D6B416352", "name": "C8309671F75C472D9A34B41D6B416352", "displayName": "Chats", "description": "Ver chats", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "AltOrOption+ControlOrCommand+C" } },
        { "fullName": "VirtualCommand___EC9BC84782D64DF993893844A3E201CD", "name": "EC9BC84782D64DF993893844A3E201CD", "displayName": "No leído", "description": "Filtrar por elementos no leídos", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "AltOrOption+ControlOrCommand+U" } },
        { "fullName": "VirtualCommand___4F0289FFB527439689D9F22BE9778443", "name": "4F0289FFB527439689D9F22BE9778443", "displayName": "Marcar todos como leídos", "description": "Marcar todos los chats como leídos", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "Shift+Escape" } },
        { "fullName": "VirtualCommand___613D7A226E5A471FBC9CABC3CACB5F87", "name": "613D7A226E5A471FBC9CABC3CACB5F87", "displayName": "Contraer todas las secciones", "description": "Contraer las secciones del chat", "groupName": "Chat", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+L" } }
```
> Recordar añadir la coma al comando anterior (Ventana) si queda antes en el array. Ordenar el array como se prefiera; el `groupName` es lo que agrupa en la UI.

**Step 2: Validar JSON**
```bash
python3 -m json.tool virtual/actions.json > /dev/null && echo "JSON OK"
```

**Step 3: Commit**
```bash
git add virtual/actions.json
git commit -m "feat: actions.json con comandos de chat"
```

---

## Task 4: `actions.json` — comandos de Reunión (incluye toggles degradados)

**Files:**
- Modify: `virtual/actions.json` (añadir comandos 17–21)

**Step 1: Insertar los 5 comandos de Reunión**
```json
        { "fullName": "VirtualCommand___70C926FAC21843628E6F26B608B0A01E", "name": "70C926FAC21843628E6F26B608B0A01E", "displayName": "Encender/Apagar cámara", "description": "Encender o apagar la cámara en Teams", "groupName": "Reunión", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+O" } },
        { "fullName": "VirtualCommand___234416A040D148FB9671BF5C7B40A2B3", "name": "234416A040D148FB9671BF5C7B40A2B3", "displayName": "Activar/Silenciar micrófono", "description": "Activar o silenciar el micrófono en Teams", "groupName": "Reunión", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+M" } },
        { "fullName": "VirtualCommand___CEF97641BBF24D548CB507F18C50E6D3", "name": "CEF97641BBF24D548CB507F18C50E6D3", "displayName": "Compartir/Dejar de compartir contenido", "description": "Compartir o dejar de compartir contenido en Teams", "groupName": "Reunión", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+E" } },
        { "fullName": "VirtualCommand___7D35DFB5B24648C0A51505FA6E37411B", "name": "7D35DFB5B24648C0A51505FA6E37411B", "displayName": "Levantar la mano", "description": "Levantar o bajar la mano en Teams", "groupName": "Reunión", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+K" } },
        { "fullName": "VirtualCommand___95DFA17B3AB64811A9D8CA55911EE47A", "name": "95DFA17B3AB64811A9D8CA55911EE47A", "displayName": "Salir", "description": "Salir de la reunión en Teams", "groupName": "Reunión", "operatingSystem": 3, "shortcuts": { "en-US": "ControlOrCommand+Shift+H" } }
```

**Step 2: Validar JSON y contar comandos (deben ser 22)**
```bash
python3 -c "import json; d=json.load(open('virtual/actions.json')); print('comandos:', len(d['commands']))"
```
Expected: `comandos: 22`.

**Step 3: Commit**
```bash
git add virtual/actions.json
git commit -m "feat: actions.json con reunión y toggles degradados a acción simple"
```

---

## Task 5: Iconos (`actionicons/` y `actionsymbols/`)

Cada acción necesita `actionicons/VirtualCommand___<HEX>.svg` (botón) y, opcionalmente, `actionsymbols/VirtualCommand___<HEX>.svg` (selector). Reutilizamos los SVG del repo compilado, **renombrándolos al `fullName`**.

**Files:**
- Create: `actionicons/VirtualCommand___<HEX>.svg` (×22)
- Create: `actionsymbols/VirtualCommand___<HEX>.svg` (×22)
- Fuente: `~/code/elmango80/logi-teams-plugin/actionicons/` y `.../actionsymbols/` y `.../src/EmbeddedResources/`

**Step 1: Script de copia/renombrado**
Crear un script temporal `map-icons.sh` con el mapeo `<ClaseOrigen> <HEX>` y ejecutarlo. Para los 3 toggles se usa el icono del **estado activo** (`CameraOn`, `MicOn`, `ShareOn` de `src/EmbeddedResources/`).
```bash
SRC=~/code/elmango80/logi-teams-plugin
AI="$SRC/actionicons"; AS="$SRC/actionsymbols"; ER="$SRC/src/EmbeddedResources"

# formato: HEX|icono_boton(ruta)|simbolo(ruta o -)
map='
0F94B7AFCC3A4EC89E39A544E8AFF24F|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToActivityCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToActivityCommand.svg
033C1F7B7E4D456D97892D55729B1BCE|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToChatCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToChatCommand.svg
C836308C6D6A4F51A57005BB8645867B|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToCopilotCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToCopilotCommand.svg
262C36D0993A45EEB90E391CEEF3DD01|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToCalendarCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToCalendarCommand.svg
C16913D60554463987230F910B83E896|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToCallsCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToCallsCommand.svg
CA0F6545E00748B592F034114EC6CD42|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToOneDriveCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToOneDriveCommand.svg
F22BA75C5C3E4C70AF5AF4BCD380F9C2|'"$AI"'/Loupedeck.MicrosoftTeamsControls.GoToShiftsCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.GoToShiftsCommand.svg
DA7CDD0EB8E443BCAD151F211210EC0A|'"$AI"'/Loupedeck.MicrosoftTeamsControls.OpenChatCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.OpenChatCommand.svg
A1EBE0B093324CB28F00D4065E7367E0|'"$AI"'/Loupedeck.MicrosoftTeamsControls.AudioCallCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.AudioCallCommand.svg
BDF04DC05E3F4AE08B825E433CEE0B27|'"$AI"'/Loupedeck.MicrosoftTeamsControls.VideoCallCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.VideoCallCommand.svg
D9144AAEFD784166802254B9E2F4ACBA|'"$AI"'/Loupedeck.MicrosoftTeamsControls.OpenCopilotCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.OpenCopilotCommand.svg
7FCB69280E5643DD9A45B4803BDD6B39|'"$AI"'/Loupedeck.MicrosoftTeamsControls.ViewChannelsCommand.svg|-
C8309671F75C472D9A34B41D6B416352|'"$AI"'/Loupedeck.MicrosoftTeamsControls.ViewChatsCommand.svg|-
4F0289FFB527439689D9F22BE9778443|'"$AI"'/Loupedeck.MicrosoftTeamsControls.MarkAllAsReadCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.MarkAllAsReadCommand.svg
613D7A226E5A471FBC9CABC3CACB5F87|'"$AI"'/Loupedeck.MicrosoftTeamsControls.CollapseAllSectionsCommand.svg|-
70C926FAC21843628E6F26B608B0A01E|'"$ER"'/CameraOn.svg|-
234416A040D148FB9671BF5C7B40A2B3|'"$ER"'/MicOn.svg|-
CEF97641BBF24D548CB507F18C50E6D3|'"$ER"'/ShareOn.svg|-
7D35DFB5B24648C0A51505FA6E37411B|'"$AI"'/Loupedeck.MicrosoftTeamsControls.RaiseHandCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.RaiseHandCommand.svg
95DFA17B3AB64811A9D8CA55911EE47A|'"$AI"'/Loupedeck.MicrosoftTeamsControls.LeaveCommand.svg|'"$AS"'/Loupedeck.MicrosoftTeamsControls.LeaveCommand.svg
D166AECAE2FE4EBEA56F038D649AA045|'"$AI"'/Loupedeck.MicrosoftTeamsControls.CollapseAppBarCommand.svg|-
'
echo "$map" | while IFS='|' read -r hex icon sym; do
  [ -z "$hex" ] && continue
  cp "$icon" "actionicons/VirtualCommand___${hex}.svg"
  [ "$sym" != "-" ] && cp "$sym" "actionsymbols/VirtualCommand___${hex}.svg"
done
```
> Notas:
> - **#3 Copilot** (`GoToCopilotCommand`): en el compilado había dos iconos (logo oficial y destello). Confirmar cuál se quiere para "Copilot" vs "Abrir Copilot".
> - **#14 No leído** y **#22 Contraer barra**: NO existen iconos en `actionicons/` del compilado (revisar). Si faltan, crear un SVG 24×24 con `stroke #F0F0F0` (convención del proyecto) o dejar que el servicio use el icono por defecto.
> - Faltan varios `actionsymbols` en el compilado (AudioCall, VideoCall, ViewChannels, etc. sí existen; verificar). El `actionsymbol` es opcional.

**Step 2: Verificar que hay 22 iconos de botón**
```bash
ls actionicons/ | grep -c '^VirtualCommand___'
```
Expected: `22` (o menos si se decide dejar acciones sin icono propio; documentarlo).

**Step 3: Comprobar que todos los `fullName` del JSON tienen icono**
```bash
python3 - <<'PY'
import json, os
d = json.load(open('virtual/actions.json'))
missing = [c['fullName'] for c in d['commands']
           if not os.path.exists(f"actionicons/{c['fullName']}.svg")]
print("sin icono:", missing or "ninguno")
PY
```
Expected: `sin icono: ninguno` (o la lista consciente de excepciones).

**Step 4: Commit**
```bash
git add actionicons/ actionsymbols/
git commit -m "feat: iconos de acciones renombrados al fullName virtual"
```

---

## Task 6: Instalación local y prueba en el servicio

**Files:**
- Instalación en: `~/Library/Application Support/Logi/LogiPluginService/Plugins/MicrosoftTeams/`

**Step 1: Copiar el plugin a la carpeta del servicio**
```bash
DEST=~/Library/Application\ Support/Logi/LogiPluginService/Plugins/MicrosoftTeams
rm -rf "$DEST"
mkdir -p "$DEST"
cp -R metadata virtual actionicons actionsymbols "$DEST"/
```
> Alternativa: crear `MicrosoftTeams.link` apuntando al repo, pero para virtual la copia directa es la más fiable.

**Step 2: Reiniciar el servicio**
```bash
pkill -9 -f "LogiPluginService"
sleep 12
```

**Step 3: Revisar el log de carga**
```bash
cat ~/Library/Application\ Support/Logi/LogiPluginService/Logs/plugin_logs/MicrosoftTeams.log 2>/dev/null | tail -40
```
Expected: carga sin errores de parseo de `actions.json` ni de manifiesto.

**Step 4: Verificación visual (usuario)**
- Abrir Logi Options+ → personalización del MX Keypad → **All Actions**.
- Confirmar que aparece **Microsoft Teams** con los 4 grupos y las 22 acciones.
- Confirmar iconos: cámara, micrófono y el recuadro con flecha (compartir) en las 3 acciones de Reunión.

**Step 5: Prueba funcional (usuario)**
- Arrastrar 3–4 acciones representativas a botones (incluyendo cámara, micro y compartir).
- Con Teams en primer plano, pulsar y confirmar que ejecutan el atajo correcto.
- **Verificar especialmente los atajos de riesgo:** #11 Abrir Copilot, #22 Contraer barra, #15 Marcar como leído.

**Step 6: Ajustes si algún atajo falla**
- Editar el `shortcuts["en-US"]` correspondiente en `virtual/actions.json`, recopiar (Step 1), reiniciar (Step 2) y reprobar.
- Commit de cada ajuste:
```bash
git add virtual/actions.json && git commit -m "fix: corrige atajo de <acción>"
```

---

## Task 7: Documentación — `README.md` y `AGENTS.md`

**Files:**
- Create: `README.md`
- Create: `AGENTS.md`
- Create: `AGENTS.local.md` (no versionado; añadir a `.gitignore`)

**Step 1: `AGENTS.md` para plugin VIRTUAL**
Reescribir la guía del compilado adaptándola a virtual. Debe cubrir, como mínimo:
- **Tipo de plugin**: virtual (JSON), NO compilado. Sin .NET, sin DLL, sin `PluginApi.dll`, sin `LogiPluginTool` obligatorio.
- **Estructura**: `metadata/LoupedeckPackage.yaml` (`pluginType: virtual`), `virtual/actions.json`, `actionicons/`, `actionsymbols/`, `localization/` (opcional).
- **Formato de `actions.json`**: campos por comando (`fullName`, `name`, `displayName`, `description`, `groupName`, `operatingSystem`, `shortcuts`).
- **Tokens de atajo**: `ControlOrCommand`, `AltOrOption`, `Shift`, teclas y `Oem*`. Un atajo `operatingSystem: 3` vale para Win+Mac.
- **Convención de iconos**: fichero = `VirtualCommand___<HEX>.svg`; SVG 24×24, `stroke #F0F0F0`.
- **Enlace a Teams**: `applicationPatterns` (`^ms-teams$`, `^com\.microsoft\.teams2$`).
- **Instalación/prueba**: copiar a `Plugins/MicrosoftTeams/`, reiniciar servicio (`pkill -9 -f LogiPluginService`), revisar log.
- **Limitación de sincronización**: NO hay API pública de estado (Third-party app API de Teams retirada el 30/06/2026). Por eso los antiguos toggles son ahora acciones simples con icono fijo.
- **Empaquetado `.lplug4`**: ZIP con la estructura (se puede crear a mano).
- **Git/identidad personal**: `elmango80`, remoto SSH `github.com-personal`, commits firmados.
- Eliminar todo lo específico de C#/.NET (net10.0, HintPath a `PluginApi.dll`, `dotnet build/format`, `nuget.config`, etc.).

**Step 2: `AGENTS.local.md`** (no versionado)
- Rutas locales, versión del servicio, particularidades de la máquina. Añadir `AGENTS.local.md` a `.gitignore`.

**Step 3: `README.md`**
- Adaptar el README del compilado: descripción, requisitos, instalación (`.lplug4` / copia manual), tabla de acciones (usar la tabla de mapeo de este plan, con `displayName` y atajo), limitaciones (estado no sincronizable, atajos dependientes de versión/idioma), solución de problemas.

**Step 4: Validar y commit**
```bash
python3 -c "import yaml; yaml.safe_load(open('metadata/LoupedeckPackage.yaml')); print('OK')"
git add README.md AGENTS.md .gitignore
git commit -m "docs: README y AGENTS para el plugin virtual"
```

---

## Task 8: Publicación

**Step 1: Push**
```bash
git push -u origin master
```
Expected: rama `master` en `origin`, commits firmados con identidad personal.

**Step 2: (Opcional) Empaquetar `.lplug4`**
```bash
cd .. && zip -r MicrosoftTeams_1_0.lplug4 logi-mx-keypad-teams-virtual-plugin \
  -x '*/.git/*' '*/docs/*' '*/.DS_Store' '*/AGENTS*.md' '*/README.md'
```
> Ajustar exclusiones según lo que deba llevar el paquete distribuible (normalmente solo `metadata/`, `virtual/`, `actionicons/`, `actionsymbols/`, `localization/`).

---

## Checklist de verificación final

- [ ] `actions.json` válido y con 22 comandos.
- [ ] Manifiesto `pluginType: virtual` y `applicationPatterns` correctos.
- [ ] 22 iconos de botón nombrados por `fullName` (o excepciones documentadas).
- [ ] El servicio carga el plugin sin errores en el log.
- [ ] Aparecen los 4 grupos y las 22 acciones en Options+.
- [ ] Los 3 antiguos toggles son acciones simples con icono del estado activo.
- [ ] Atajos de riesgo (#11, #15, #22) verificados en Teams.
- [ ] `AGENTS.md` reescrito para virtual (sin restos de C#/.NET).
- [ ] Commits firmados con identidad personal; push a `origin`.
