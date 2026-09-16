namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Ir al calendario — Cmd+4
    public class GoToCalendarCommand : PluginDynamicCommand
    {
        public GoToCalendarCommand()
            : base(displayName: "Ir al calendario", description: "Ir al calendario de Teams (Cmd+4)", groupName: "Navegación")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.Key4, ModifierKey.ControlOrCommand);
    }
}
