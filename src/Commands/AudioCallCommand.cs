namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Llamada de audio — Opt+Cmd+S
    public class AudioCallCommand : TeamsCommandBase
    {
        public AudioCallCommand()
            : base(displayName: "Llamada de audio", description: "Iniciar llamada de audio (Opt+Cmd+S)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyS, ModifierKey.ControlOrCommand | ModifierKey.AltOrOption);
    }
}
