namespace Loupedeck.MicrosoftTeamsControls
{
    using System;
    using System.IO;
    using System.Linq;

    // Helper compartido para dibujar iconos SVG del plugin CENTRADOS sobre el fondo,
    // de forma que no queden pegados a la parte superior del botón.
    // Lo usan tanto TeamsCommandBase (comandos estáticos) como los comandos multiestado.
    public static class TeamsIcon
    {
        public const UInt32 BackgroundArgb = 0xFF505AC9u;  // #505AC9 (azul del plugin)
        public const UInt32 OnArgb = 0xFF1E8E3Eu;          // verde (estado ON)
        public const UInt32 OffArgb = 0xFFD93025u;         // rojo (estado OFF)

        // Porcentaje del botón que ocupa el icono (centrado).
        private const Double IconScale = 0.55;

        // Dibuja un SVG centrado sobre el fondo por defecto del plugin.
        public static BitmapImage RenderCentered(String svg, PluginImageSize imageSize) =>
            RenderCentered(svg, imageSize, BackgroundArgb);

        // Dibuja un SVG centrado sobre un fondo de color concreto.
        public static BitmapImage RenderCentered(String svg, PluginImageSize imageSize, UInt32 backgroundArgb)
        {
            using (var bb = new BitmapBuilder(imageSize))
            {
                bb.Clear(BitmapColor.FromArgb(backgroundArgb));

                var icon = BitmapImage.FromSvg(svg);
                var side = Math.Min(bb.Width, bb.Height);
                var iconSize = (Int32)(side * IconScale);
                var x = (bb.Width - iconSize) / 2;
                var y = (bb.Height - iconSize) / 2;
                bb.DrawImage(icon, x, y, iconSize, iconSize, BitmapRotation.None);

                return bb.ToImage();
            }
        }

        // Lee un SVG embebido en la DLL (carpeta EmbeddedResources/), por nombre corto de
        // fichero (p. ej. "CameraOn.svg"). Devuelve null si no existe.
        // No depende de Assembly.Location (que en el runtime del LPS viene vacío).
        public static String LoadEmbeddedSvg(String shortName)
        {
            try
            {
                var asm = typeof(TeamsIcon).Assembly;
                var name = asm.GetManifestResourceNames()
                              .FirstOrDefault(n => n.EndsWith("." + shortName, StringComparison.OrdinalIgnoreCase)
                                                || n.EndsWith(shortName, StringComparison.OrdinalIgnoreCase));
                if (name == null)
                {
                    return null;
                }
                using (var stream = asm.GetManifestResourceStream(name))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
