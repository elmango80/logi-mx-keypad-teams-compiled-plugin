namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ver no leído — Opt+Cmd+U
    public class ViewUnreadCommand : TeamsCommandBase
    {
        public ViewUnreadCommand()
            : base(displayName: "Ver no leído", description: "Filtrar por no leído (Opt+Cmd+U)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyU, ModifierKey.ControlOrCommand | ModifierKey.AltOrOption);
    }
}
