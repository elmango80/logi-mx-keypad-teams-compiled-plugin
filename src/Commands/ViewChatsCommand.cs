namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ver chats — Opt+Cmd+C
    public class ViewChatsCommand : TeamsCommandBase
    {
        public ViewChatsCommand()
            : base(displayName: "Chats", description: "Ver chats (Opt+Cmd+C)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyC, ModifierKey.ControlOrCommand | ModifierKey.AltOrOption);
    }
}
