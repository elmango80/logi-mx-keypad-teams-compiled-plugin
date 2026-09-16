namespace Loupedeck.MicrosoftTeamsControls
{
    using System;

    // Enlaza el plugin con Microsoft Teams: su perfil se activa cuando Teams tiene el foco.
    public class MicrosoftTeamsControlsApplication : ClientApplication
    {
        public MicrosoftTeamsControlsApplication() { }

        // Windows: nombre de proceso.
        protected override String GetProcessName() => "ms-teams";

        // macOS: bundle id de Teams (version nueva). En este equipo es com.microsoft.teams2.
        protected override String GetBundleName() => "com.microsoft.teams2";
    }
}
