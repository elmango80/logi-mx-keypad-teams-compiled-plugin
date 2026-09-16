namespace Loupedeck.MicrosoftTeamsControls
{
    using System;
    using System.IO;

    // Helper compartido para dibujar iconos SVG del plugin CENTRADOS sobre el fondo,
    // de forma que no queden pegados a la parte superior del botón.
    // Lo usan tanto TeamsCommandBase (comandos estáticos) como los comandos multiestado.
    public static class TeamsIcon
    {
        public const UInt32 BackgroundArgb = 0xFF505AC9u;  // #505AC9

        // Porcentaje del botón que ocupa el icono (centrado).
        private const Double IconScale = 0.55;

        // Dibuja un SVG centrado sobre el fondo del plugin.
        public static BitmapImage RenderCentered(String svg, PluginImageSize imageSize)
        {
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

        // Lee un SVG de la carpeta actionicons/ del propio plugin por nombre de fichero
        // (p. ej. "Loupedeck.MicrosoftTeamsControls.GoToChatCommand.svg"). Devuelve null si no existe.
        public static String LoadSvg(String fileName)
        {
            try
            {
                var dllDir = Path.GetDirectoryName(typeof(TeamsIcon).Assembly.Location); // .../mac
                var pluginDir = Path.GetDirectoryName(dllDir);                           // raíz del plugin
                var path = Path.Combine(pluginDir, "actionicons", fileName);
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
