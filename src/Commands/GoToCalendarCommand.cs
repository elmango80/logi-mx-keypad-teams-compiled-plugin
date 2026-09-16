namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir al calendario — Cmd+4
    public class GoToCalendarCommand : TeamsCommandBase
    {
        public GoToCalendarCommand()
            : base(displayName: "Calendario", description: "Ir al calendario de Teams (Cmd+4)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key4, ModifierKey.ControlOrCommand);
    }
}
