namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir a turnos — Cmd+7
    public class GoToShiftsCommand : TeamsCommandBase
    {
        public GoToShiftsCommand()
            : base(displayName: "Turnos", description: "Ir a Turnos en Teams (Cmd+7)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key7, ModifierKey.ControlOrCommand);
    }
}
