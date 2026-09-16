namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Abrir Copilot — Ctrl+Cmd+I
    public class OpenCopilotCommand : TeamsCommandBase
    {
        public OpenCopilotCommand()
            : base(displayName: "Abrir Copilot", description: "Abrir Copilot (Ctrl+Cmd+I)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyI, ModifierKey.ControlOrCommand | ModifierKey.Control);
    }
}
