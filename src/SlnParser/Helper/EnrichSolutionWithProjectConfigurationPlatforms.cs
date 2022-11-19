using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SlnParser.Helper
{
    internal sealed class EnrichSolutionWithProjectConfigurationPlatforms : IEnrichSolution
    {
        private readonly IParseSolutionConfigurationPlatform _parseSolutionConfigurationPlatform;

        public EnrichSolutionWithProjectConfigurationPlatforms()
        {
            _parseSolutionConfigurationPlatform = new SolutionConfigurationPlatformParser();
        }

        public void Enrich(Solution solution, IEnumerable<string> fileContents)
        {
            var projectConfigurations = _parseSolutionConfigurationPlatform.Parse(
                fileContents,
                "GlobalSection(ProjectConfiguration");
            MapConfigurationPlatformsToProjects(solution, projectConfigurations);
        }

        private static void MapConfigurationPlatformsToProjects(
            ISolution solution,
            IEnumerable<ProjectConfigurationPlatform> projectConfigurations)
        {
            foreach (var configuration in projectConfigurations)
                MapConfigurationPlatformToProject(solution, configuration);
        }

        private static void MapConfigurationPlatformToProject(
            ISolution solution,
            ProjectConfigurationPlatform configuration)
        {
            if (!configuration.ProjectId.HasValue)
                throw new UnexpectedSolutionStructureException(
                    "Expected to find a project-id " +
                    $"for the Project-Platform-Configuration '{configuration.ConfigurationPlatform.Name}'");

            var project = solution
                .AllProjects
                .FirstOrDefault(project => project.Id == configuration.ProjectId.Value);

            if (project == null) return;

            if (!(project is SolutionProject solutionProject))
                throw new UnexpectedSolutionStructureException(
                    "Expected to find a Solution-Project with the id " +
                    $"'{configuration.ProjectId.Value}' for the Project-Platform-Configuration " +
                    $"'{configuration.ConfigurationPlatform.Name}' but found " +
                    $" project of type '{project.GetType().Name}' instead");

            solutionProject.AddConfigurationPlatform(configuration.ConfigurationPlatform);
        }
    }
}
