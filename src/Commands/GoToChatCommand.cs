namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir al chat — Cmd+2
    public class GoToChatCommand : PluginDynamicCommand
    {
        public GoToChatCommand()
            : base(displayName: "Ir al chat", description: "Ir al chat de Teams (Cmd+2)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key2, ModifierKey.ControlOrCommand);
    }
}
