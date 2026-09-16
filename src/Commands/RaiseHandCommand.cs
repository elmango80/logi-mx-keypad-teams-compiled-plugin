namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Levantar la mano — Cmd+Shift+K
    public class RaiseHandCommand : TeamsCommandBase
    {
        public RaiseHandCommand()
            : base(displayName: "Levantar la mano", description: "Levantar o bajar la mano en Teams (Cmd+Shift+K)", groupName: "Reunión")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyK, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
