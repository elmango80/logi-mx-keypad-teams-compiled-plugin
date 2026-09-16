# Diseño del README de Microsoft Teams Controls

## Objetivo

Crear un README orientado principalmente a usuarios finales que explique qué hace el plugin, qué necesita el usuario, cómo instalarlo y cómo configurarlo en Logi Options+ con un MX Keypad.

## Alcance aprobado

El README tendrá un enfoque user-first con una sección técnica breve. Incluirá:

- Descripción del plugin y sus beneficios.
- Características principales.
- Requisitos de uso.
- Instalación y configuración en Logi Options+.
- Tabla de acciones agrupadas por navegación, chat, reunión y ventana.
- Limitaciones conocidas, especialmente la falta de sincronización real de los estados de cámara, micrófono y compartir contenido.
- Solución de problemas habituales.
- Licencia MIT.

No será una guía completa de desarrollo: no detallará la arquitectura interna, el build offline ni el empaquetado para colaboradores salvo que sea necesario para contextualizar el proyecto.

## Fuente de verdad

La lista de acciones y atajos se comprobará contra las clases actuales de `src/Commands/`. El nombre visible, versión, autor y licencia se comprobarán contra `metadata/LoupedeckPackage.yaml`. Las instrucciones reflejarán el flujo real de instalación del plugin y no presentarán como disponible una API de Teams retirada.

## Estructura propuesta

```text
# Microsoft Teams Controls

Descripción breve

## Características
## Requisitos
## Instalación
## Configuración en Logi Options+
## Acciones disponibles
  - Navegación
  - Chat
  - Reunión
  - Ventana
## Limitaciones conocidas
## Solución de problemas
## Licencia
```

## Criterios de aceptación

- Un usuario final puede entender qué hace el plugin sin conocer C# ni Loupedeck.
- Las instrucciones distinguen claramente entre instalar un paquete publicado y compilar desde el repositorio.
- La tabla de acciones coincide con el código actual.
- Se documenta explícitamente que los toggles mantienen un estado interno/óptico y pueden desincronizarse de Teams.
- El Markdown es válido, legible y no contiene rutas o datos exclusivos del entorno local del desarrollador.
