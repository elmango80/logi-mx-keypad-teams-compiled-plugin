namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir a Copilot — Cmd+3
    public class GoToCopilotCommand : TeamsCommandBase
    {
        public GoToCopilotCommand()
            : base(displayName: "Copilot", description: "Ir a Copilot en Teams (Cmd+3)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key3, ModifierKey.ControlOrCommand);
    }
}
