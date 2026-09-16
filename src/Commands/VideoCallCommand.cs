namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Videollamada — Cmd+Shift+S
    public class VideoCallCommand : PluginDynamicCommand
    {
        public VideoCallCommand()
            : base(displayName: "Videollamada", description: "Iniciar videollamada en Teams (Cmd+Shift+S)", groupName: "Reunión")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyS, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
