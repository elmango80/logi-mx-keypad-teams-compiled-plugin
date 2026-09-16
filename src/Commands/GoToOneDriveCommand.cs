namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir a OneDrive — Cmd+6
    public class GoToOneDriveCommand : TeamsCommandBase
    {
        public GoToOneDriveCommand()
            : base(displayName: "OneDrive", description: "Ir a OneDrive en Teams (Cmd+6)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key6, ModifierKey.ControlOrCommand);
    }
}
