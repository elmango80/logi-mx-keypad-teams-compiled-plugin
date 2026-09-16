namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Micrófono On/Off — Cmd+Shift+M (multiestado).
    // Toggle "óptico": el plugin no conoce el estado real de Teams, así que alterna el icono
    // en cada pulsación asumiendo un estado inicial (Off). Si se desincroniza, una pulsación
    // extra realinea. Estados: Off(0, inicial) / On(1).
    public class ToggleMicCommand : PluginMultistateDynamicCommand
    {
        public ToggleMicCommand()
            : base("Micrófono On/Off", "Silenciar/activar el micrófono en Teams (Cmd+Shift+M)", "Reunión")
        {
            this.AddState("Off", "El micrófono está silenciado"); // índice 0 (estado inicial)
            this.AddState("On", "El micrófono está activo");      // índice 1
        }

        protected override void RunCommand(String actionParameter)
        {
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyM, ModifierKey.ControlOrCommand | ModifierKey.Shift);
            this.ToggleCurrentState(actionParameter);
            // Notificar al servicio para que redibuje el botón con el icono del nuevo estado.
            this.ActionImageChanged(actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, Int32 deviceState, PluginImageSize imageSize)
        {
            var on = deviceState == 1;
            var svg = TeamsIcon.LoadEmbeddedSvg(on ? "MicOn.svg" : "MicOff.svg");
            return String.IsNullOrEmpty(svg)
                ? base.GetCommandImage(actionParameter, deviceState, imageSize)
                : TeamsIcon.RenderCentered(svg, imageSize, on ? TeamsIcon.OnArgb : TeamsIcon.OffArgb);
        }
    }
}
