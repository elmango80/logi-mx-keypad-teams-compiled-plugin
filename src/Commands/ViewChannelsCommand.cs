namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ver canales — Opt+Cmd+A
    public class ViewChannelsCommand : TeamsCommandBase
    {
        public ViewChannelsCommand()
            : base(displayName: "Canales", description: "Ver canales (Opt+Cmd+A)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyA, ModifierKey.ControlOrCommand | ModifierKey.AltOrOption);
    }
}
