namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Compartir pantalla — Cmd+Shift+E
    public class ShareScreenCommand : PluginDynamicCommand
    {
        public ShareScreenCommand()
            : base(displayName: "Compartir pantalla", description: "Compartir pantalla en Teams (Cmd+Shift+E)", groupName: "Reunión")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyE, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
