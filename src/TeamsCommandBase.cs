namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Clase base común a todos los comandos estáticos del plugin.
    // Dibuja el icono (SVG de actionicons/, por nombre de clase) CENTRADO sobre el fondo
    // usando el helper TeamsIcon, para que no quede pegado a la parte superior del botón.
    public abstract class TeamsCommandBase : PluginDynamicCommand
    {
        protected TeamsCommandBase(String displayName, String description, String groupName)
            : base(displayName, description, groupName)
        {
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            // SVG nombrado por el nombre completo de la clase
            // (p. ej. Loupedeck.MicrosoftTeamsControls.GoToChatCommand.svg).
            var svg = TeamsIcon.LoadSvg(this.GetType().FullName + ".svg");

            // Sin icono propio: usar el render por defecto (texto/título).
            return String.IsNullOrEmpty(svg)
                ? base.GetCommandImage(actionParameter, imageSize)
                : TeamsIcon.RenderCentered(svg, imageSize);
        }
    }
}
