namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Compartir contenido On/Off — Cmd+Shift+E (multiestado).
    // Toggle "óptico": alterna el icono en cada pulsación asumiendo un estado inicial
    // (no compartiendo). Estados: no compartiendo (0, inicial) / compartiendo (1).
    // Nota: se mantiene el nombre de clase ShareScreenCommand como ID estable de la acción.
    public class ShareScreenCommand : PluginMultistateDynamicCommand
    {
        public ShareScreenCommand()
            : base("Compartir contenido", "Compartir o dejar de compartir contenido en Teams (Cmd+Shift+E)", "Reunión")
        {
            this.AddState("Compartir contenido", "No estás compartiendo"); // índice 0 (estado inicial)
            this.AddState("Dejar de compartir", "Estás compartiendo");     // índice 1
        }

        protected override void RunCommand(String actionParameter)
        {
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyE, ModifierKey.ControlOrCommand | ModifierKey.Shift);
            this.ToggleCurrentState(actionParameter);
            // Notificar al servicio para que redibuje el botón con el icono del nuevo estado.
            this.ActionImageChanged(actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, Int32 deviceState, PluginImageSize imageSize)
        {
            var sharing = deviceState == 1;
            var svg = TeamsIcon.LoadEmbeddedSvg(sharing ? "ShareOn.svg" : "ShareOff.svg");
            return String.IsNullOrEmpty(svg)
                ? base.GetCommandImage(actionParameter, deviceState, imageSize)
                : TeamsIcon.RenderCentered(svg, imageSize);
        }
    }
}
