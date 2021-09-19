using System;

namespace SlnParser.Contracts.Helper
{
    internal sealed class ProjectConfigurationPlatform
    {
        public ProjectConfigurationPlatform(
            Guid? projectId,
            ConfigurationPlatform configurationPlatform)
        {
            ProjectId = projectId;
            ConfigurationPlatform = configurationPlatform;
        }

        public Guid? ProjectId { get; }

        public ConfigurationPlatform ConfigurationPlatform { get; }
    }
}
