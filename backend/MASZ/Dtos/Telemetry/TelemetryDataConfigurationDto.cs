namespace MASZ.Dtos
{
    public class TelemetryDataConfigurationDto
    {
        public string HashedServer { get; set; }
        public string DeploymentMode { get; set; }
        public string DeploymentVersion { get; set; }
        public int DefaultLanguage { get; set; }
        public bool DemoMode { get; set; }
        public bool PublicFileMode { get; set; }
        public bool CustomPluginsEnabled { get; set; }
        public int CustomPluginsCount { get; set; }
        public int SiteAdminsCount { get; set; }
    }
}