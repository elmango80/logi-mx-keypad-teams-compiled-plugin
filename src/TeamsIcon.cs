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
        // Porcentaje del botón que ocupa el icono (centrado).
        private const Double IconScale = 0.55;

        // Dibuja un SVG centrado sobre fondo TRANSPARENTE (se ve el fondo propio del botón).
        public static BitmapImage RenderCentered(String svg, PluginImageSize imageSize)
        {
            using (var bb = new BitmapBuilder(imageSize))
            {
                bb.Clear(BitmapColor.FromArgb(0x00000000u)); // transparente

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
