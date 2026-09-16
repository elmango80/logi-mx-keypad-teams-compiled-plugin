# Thinner Leave Icon Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Reducir el peso visual del icono relleno de `LeaveCommand` sin volver a un icono outline.

**Architecture:** Se mantendrá el mismo `viewBox` y color de icono. La nueva geometría SVG se propagará a la fuente del repositorio, a la copia instalada del plugin y al SVG Base64 guardado en el `.ict` del perfil activo.

**Tech Stack:** SVG, JSON `.ict`, LogiPluginService, .NET 10.

---

### Task 1: Reemplazar la geometría del icono

**Files:**

- Modify: `actionicons/Loupedeck.MicrosoftTeamsControls.LeaveCommand.svg`
- Modify: `actionsymbols/Loupedeck.MicrosoftTeamsControls.LeaveCommand.svg`

**Step 1:** Mantener la raíz de 24x24 con `fill="#F0F0F0"` y `stroke-width="1.5"`.

**Step 2:** Sustituir el path actual por una silueta rellena más estrecha.

**Step 3:** Comprobar que los dos SVG contienen la nueva geometría.

### Task 2: Sincronizar la instalación y el perfil activo

**Files:**

- Modify: `~/Library/Application Support/Logi/LogiPluginService/Plugins/MicrosoftTeamsControls/actionicons/Loupedeck.MicrosoftTeamsControls.LeaveCommand.svg`
- Modify: `~/Library/Application Support/Logi/LogiPluginService/Plugins/MicrosoftTeamsControls/actionsymbols/Loupedeck.MicrosoftTeamsControls.LeaveCommand.svg`
- Modify: `~/Library/Application Support/Logi/LogiPluginService/Applications/Loupedeck70/@_microsoftteamscontrols/Profiles/4817EB0439FD4D319DB78DA32869431B/ActionIcons/$MicrosoftTeamsControls___Loupedeck.MicrosoftTeamsControls.LeaveCommand.ict`

**Step 1:** Copiar la geometría nueva a los SVG instalados.

**Step 2:** Codificar la nueva imagen SVG en Base64 y sustituir solo el campo `image` del `.ict`.

**Step 3:** Preservar el texto, el área, el fondo y el resto de ajustes del perfil.

### Task 3: Recargar y verificar

**Step 1:** Eliminar la caché de iconos de LogiOptionsPlus.

**Step 2:** Reiniciar `LogiPluginService`.

**Step 3:** Ejecutar `dotnet build -c Debug --no-restore` y `git diff --check`.

**Step 4:** Leer de nuevo los SVG instalados y el `.ict`, y confirmar que contienen la nueva imagen rellena.
