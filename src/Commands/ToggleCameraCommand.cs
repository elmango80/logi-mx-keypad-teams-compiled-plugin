namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Cámara On/Off — Cmd+Shift+O (multiestado).
    // Toggle "óptico": el plugin no conoce el estado real de Teams, así que alterna el icono
    // en cada pulsación asumiendo un estado inicial (Off). Si se desincroniza, una pulsación
    // extra realinea. Estados: Off(0, inicial) / On(1).
    public class ToggleCameraCommand : PluginMultistateDynamicCommand
    {
        public ToggleCameraCommand()
            : base("Cámara On/Off", "Encender/apagar la cámara en Teams (Cmd+Shift+O)", "Reunión")
        {
            this.AddState("Apagar", "La cámara está apagada");    // índice 0 (estado inicial)
            this.AddState("Encender", "La cámara está encendida"); // índice 1
        }

        protected override void RunCommand(String actionParameter)
        {
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyO, ModifierKey.ControlOrCommand | ModifierKey.Shift);
            this.ToggleCurrentState(actionParameter);
            // Notificar al servicio para que redibuje el botón con el icono del nuevo estado.
            this.ActionImageChanged(actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, Int32 deviceState, PluginImageSize imageSize)
        {
            var on = deviceState == 1;
            var svg = TeamsIcon.LoadEmbeddedSvg(on ? "CameraOn.svg" : "CameraOff.svg");
            return String.IsNullOrEmpty(svg)
                ? base.GetCommandImage(actionParameter, deviceState, imageSize)
                : TeamsIcon.RenderCentered(svg, imageSize);
        }
    }
}
