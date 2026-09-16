# Progreso — sesión 2026-09-16

> Notas de progreso de esta sesión: lo implementado, lo aprendido y lo pendiente.
> Guía estable del proyecto: ../../AGENTS.md

### ✅ Implementado
- Proyecto C# (`src/`) que compila **offline** contra la `PluginApi` local (net10.0).
- Clase `Plugin` + `Application` enlazada a **Microsoft Teams** (`com.microsoft.teams2`);
  su perfil se activa al enfocar Teams (verificado).
- **19 comandos estáticos** (atajos de teclado) en 4 grupos:
  - **Navegación**: Actividad (Cmd+1), Chat (Cmd+2), Copilot (Cmd+3), Calendario (Cmd+4),
    Llamadas (Cmd+5), OneDrive (Cmd+6), Turnos (Cmd+7).
  - **Chat**: Abrir chat (Cmd+Shift+N), Marcar todos como leídos (Shift+Esc), Contraer
    todas las secciones (Shift+Cmd+L), Ver no leído (Opt+Cmd+U), Ver canales (Opt+Cmd+A),
    Ver chats (Opt+Cmd+C), Llamada de audio (Opt+Cmd+S), Abrir Copilot (Ctrl+Cmd+I).
  - **Reunión**: Videollamada (Cmd+Shift+S), Compartir pantalla (Cmd+Shift+E), Levantar la
    mano (Cmd+Shift+K).
  - **Ventana**: Contraer barra de aplicaciones (Cmd+\).
- Nombres en **español** (directos en el código, sin XLIFF).
- **Colores** en el manifiesto: fondo `#505AC9` (`backgroundColor 4283456201`), icono/texto
  `#F0F0F0` (`foregroundColor`/`textColor 4293980400`).
- **Iconos** (SVG en `actionicons/` + `actionsymbols/`, nombrados por clase). Clase base
  `TeamsCommandBase` que dibuja el icono **centrado** (55%) sobre el fondo vía
  `GetCommandImage` (evita que quede pegado arriba). Faltan por pegar algunos SVG (ver abajo).
- Plugin virtual antiguo (`MicrosoftTeams-BB539293FA49494A`) **eliminado** por completo.
- Instalación en dev: carpeta `Plugins/MicrosoftTeamsControls/` con `metadata/` + `mac/DLL`
  + `actionicons/` + `actionsymbols/` (creada a mano; ver §5/§11 y AGENTS.local.md).

### 💡 Aprendizajes / a tener en cuenta
- **Iconos SVG**: usar color en **atributos de presentación** (`stroke="#F0F0F0"`,
  `fill="none"`), **NO** en `style="..."` (CSS inline). El renderizador de Logi ignora el
  color del `style=` y pinta en negro (le pasó al SVG "Line Color" de OneDrive de SVG Repo).
- **Centrado del icono**: por defecto el botón reserva la parte inferior para el título y el
  icono queda arriba. Se soluciona dibujando el icono nosotros centrado en `GetCommandImage`
  (clase `TeamsCommandBase`). Si no hay SVG, se hace `base.GetCommandImage(...)` (render por
  defecto), así los comandos sin icono no se rompen.
- **Nombre de archivo de icono** = nombre COMPLETO de la clase, p. ej.
  `Loupedeck.MicrosoftTeamsControls.GoToChatCommand.svg`. `actionicons/` = icono del botón;
  `actionsymbols/` = iconito del panel de selección de acciones.
- **Colores del plugin** (manifiesto) en **ARGB** (opaco = `0xFF` + hex). Aplican al render
  por defecto (texto) y al recoloreado de SVG monocromos.
- **Modificadores** (`ModifierKey`): `ControlOrCommand` = Cmd(mac)/Ctrl(win), `Shift`,
  `AltOrOption` = Opt(mac)/Alt(win), `Control` = Ctrl literal. Para `\` se usa la sobrecarga
  por carácter `SendKeyboardShortcut('\\', ...)` (más fiable en teclado no-US).
- **Reinicio del servicio**: cualquier cambio (DLL, iconos, manifiesto) requiere reiniciar el
  LogiPluginService; cerrar la ventana no basta (ver §11.d / AGENTS.local.md).
- `PluginLog` NO existe en la `PluginApi` local (era helper de plantilla); no usarlo.

### ⏳ Pendiente
- Pegar los SVG que faltan: Copilot, Llamadas, Contraer secciones, Ver canales, Ver chats,
  Abrir Copilot, Contraer barra de aplicaciones. (Con `style=` los normalizo a atributos.)
- **Comandos multiestado** (cámara on/off y micrófono on/off) — objetivo final del proyecto
  (usar `PluginMultistateDynamicCommand`, ver §9.2).
- (Opcional) Localización formal vía XLIFF si se quiere multi-idioma.
- (Opcional) Empaquetar `.lplug4` para instalación/distribución final.
- Ajustar tamaño del icono (`IconScale` en `TeamsCommandBase`) si 55% no convence.
