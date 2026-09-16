namespace Loupedeck.MicrosoftTeamsControls
{
    using System;
    using System.IO;

    // Clase base común a todos los comandos del plugin.
    // Dibuja el icono (SVG de actionicons/, por nombre de clase) CENTRADO sobre el fondo,
    // para que no quede pegado a la parte superior del botón.
    public abstract class TeamsCommandBase : PluginDynamicCommand
    {
        private const UInt32 BackgroundArgb = 0xFF505AC9u;  // #505AC9

        // Porcentaje del botón que ocupa el icono (centrado).
        private const Double IconScale = 0.55;

        protected TeamsCommandBase(String displayName, String description, String groupName)
            : base(displayName, description, groupName)
        {
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var svg = this.LoadIconSvg();

            // Sin icono propio: usar el render por defecto (texto/título).
            if (String.IsNullOrEmpty(svg))
            {
                return base.GetCommandImage(actionParameter, imageSize);
            }

            using (var bb = new BitmapBuilder(imageSize))
            {
                bb.Clear(BitmapColor.FromArgb(BackgroundArgb));

                var icon = BitmapImage.FromSvg(svg);
                var side = Math.Min(bb.Width, bb.Height);
                var iconSize = (Int32)(side * IconScale);
                var x = (bb.Width - iconSize) / 2;
                var y = (bb.Height - iconSize) / 2;
                bb.DrawImage(icon, x, y, iconSize, iconSize, BitmapRotation.None);

                return bb.ToImage();
            }
        }

        // Lee el SVG de la carpeta actionicons/ del propio plugin, nombrado por el nombre
        // completo de la clase (p. ej. Loupedeck.MicrosoftTeamsControls.GoToChatCommand.svg).
        private String LoadIconSvg()
        {
            try
            {
                var dllDir = Path.GetDirectoryName(this.GetType().Assembly.Location); // .../mac
                var pluginDir = Path.GetDirectoryName(dllDir);                        // raíz del plugin
                var path = Path.Combine(pluginDir, "actionicons", this.GetType().FullName + ".svg");
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
