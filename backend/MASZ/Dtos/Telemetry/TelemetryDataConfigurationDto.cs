namespace masz.Dtos
{
    public class TelemetryDataConfigurationDto
    {
        public string DeploymentMode { get; set; }
        public string DeploymentVersion { get; set; }
        public string DefaultLanguage { get; set; }
        public bool DemoMode { get; set; }
        public bool PublicFileMode { get; set; }
        public bool CustomPluginsEnabled { get; set; }
        public int CustomPluginsCount { get; set; }
        public int SiteAdminsCount { get; set; }
    }
}