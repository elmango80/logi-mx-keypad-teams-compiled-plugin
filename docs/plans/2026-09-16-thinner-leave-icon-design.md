# Thinner Leave Icon Design

**Goal:** Mantener el icono de `LeaveCommand` relleno, pero reducir el grosor visual de la silueta del teléfono.

**Design:** Sustituir la geometría rellena actual por una silueta de auricular más estrecha dentro del mismo `viewBox="0 0 24 24"`. El icono seguirá usando `fill="#F0F0F0"`, sin depender de un trazo outline.

**Synchronization:** Actualizar el SVG fuente del repositorio, la copia instalada en `LogiPluginService` y la imagen Base64 almacenada en el `.ict` del perfil activo. Después se limpiará la caché de iconos y se reiniciará el servicio.

**Verification:** Comprobar el contenido de las tres capas, ejecutar `dotnet build -c Debug --no-restore`, validar `git diff --check` y confirmar que `LogiPluginService` vuelve a estar ejecutándose.
