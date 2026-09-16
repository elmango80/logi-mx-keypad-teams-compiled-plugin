namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Marcar todos como leídos — Shift+Esc
    public class MarkAllAsReadCommand : TeamsCommandBase
    {
        public MarkAllAsReadCommand()
            : base(displayName: "Marcar todos como leídos", description: "Marcar todos los chats como leídos (Shift+Esc)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Escape, ModifierKey.Shift);
    }
}
