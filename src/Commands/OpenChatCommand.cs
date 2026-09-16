namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Abrir chat (nuevo) — Cmd+Shift+N
    public class OpenChatCommand : PluginDynamicCommand
    {
        public OpenChatCommand()
            : base(displayName: "Abrir chat", description: "Abrir el panel de chat de Teams (Cmd+Shift+N)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyN, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
