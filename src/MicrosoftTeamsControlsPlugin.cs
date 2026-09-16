namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Clase Plugin: logica global. El LogiPluginService descubre esta clase (hereda de Plugin).
    public class MicrosoftTeamsControlsPlugin : Plugin
    {
        // Es un plugin ligado a una aplicacion (Microsoft Teams), no universal.
        public override Boolean HasNoApplication => false;

        public MicrosoftTeamsControlsPlugin() { }

        public override void Load() { }

        public override void Unload() { }
    }
}
