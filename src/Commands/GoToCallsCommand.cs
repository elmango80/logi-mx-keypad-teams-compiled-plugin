namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Llamadas — Cmd+5
    public class GoToCallsCommand : TeamsCommandBase
    {
        public GoToCallsCommand()
            : base(displayName: "Llamadas", description: "Ir a Llamadas en Teams (Cmd+5)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key5, ModifierKey.ControlOrCommand);
    }
}
