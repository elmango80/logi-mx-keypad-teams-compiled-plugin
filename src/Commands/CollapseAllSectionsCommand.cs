namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Contraer todas las secciones — Shift+Cmd+L
    public class CollapseAllSectionsCommand : TeamsCommandBase
    {
        public CollapseAllSectionsCommand()
            : base(displayName: "Contraer todas las secciones", description: "Contraer todas las secciones (Shift+Cmd+L)", groupName: "Chat")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.KeyL, ModifierKey.ControlOrCommand | ModifierKey.Shift);
    }
}
