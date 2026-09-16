namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Abrir chat (nuevo) — Cmd+Shift+N
    public class OpenChatCommand : TeamsCommandBase
    {
        public OpenChatCommand()
            : base(displayName: "Abrir chat", description: "Abrir el panel de chat de Teams (Cmd+Shift+N)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyN, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
