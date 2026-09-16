namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Contraer barra de aplicaciones — Cmd+\
    // Se envía el carácter '\' (más fiable que el código de tecla en teclados no-US).
    public class CollapseAppBarCommand : TeamsCommandBase
    {
        public CollapseAppBarCommand()
            : base(displayName: "Contraer barra de aplicaciones", description: "Contraer/expandir la barra de aplicaciones (Cmd+\\)", groupName: "Ventana")
        {
        }

        protected override void RunCommand(String actionParameter) =>
            this.Plugin.ClientApplication.SendKeyboardShortcut('\\', ModifierKey.ControlOrCommand);
    }
}
