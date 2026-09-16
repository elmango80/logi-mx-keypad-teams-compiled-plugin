namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Salir de la reunión / colgar — Shift+Cmd+H
    // Icono (teléfono rojo) desde actionicons/ (render nativo del servicio).
    public class LeaveCommand : TeamsCommandBase
    {
        public LeaveCommand()
            : base(displayName: "Salir", description: "Salir de la reunión en Teams (Shift+Cmd+H)", groupName: "Reunión")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyH, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
