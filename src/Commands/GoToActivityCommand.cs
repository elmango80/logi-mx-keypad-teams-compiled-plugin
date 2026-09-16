namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir a actividad — Cmd+1
    public class GoToActivityCommand : TeamsCommandBase
    {
        public GoToActivityCommand()
            : base(displayName: "Actividad", description: "Ir a Actividad en Teams (Cmd+1)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key1, ModifierKey.ControlOrCommand);
    }
}
