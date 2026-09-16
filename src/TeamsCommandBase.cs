namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Clase base común a todos los comandos estáticos del plugin.
    // El icono del botón lo aporta el servicio de forma nativa desde la carpeta actionicons/
    // (fichero nombrado por el nombre completo de la clase). No renderizamos aquí porque
    // Assembly.Location no es fiable en el runtime del LogiPluginService (ver ToggleXxx).
    public abstract class TeamsCommandBase : PluginDynamicCommand
    {
        protected TeamsCommandBase(String displayName, String description, String groupName)
            : base(displayName, description, groupName)
        {
        }
    }
}
