using SlnParser.Contracts;
using SlnParser.Contracts.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SlnParser.Helper
{
    internal sealed class EnrichSolutionWithSolutionConfigurationPlatforms : IEnrichSolution
    {
        private readonly IParseSolutionConfigurationPlatform _parseSolutionConfigurationPlatform;

        public EnrichSolutionWithSolutionConfigurationPlatforms()
        {
            _parseSolutionConfigurationPlatform = new SolutionConfigurationPlatformParser();
        }

        public void Enrich(Solution solution, IEnumerable<string> fileContents)
        {
            var projectConfigurations = _parseSolutionConfigurationPlatform.Parse(
                fileContents,
                "GlobalSection(SolutionConfiguration");
            solution.ConfigurationPlatforms = projectConfigurations
                .Select(projectConfiguration => projectConfiguration.ConfigurationPlatform)
                .ToList()
                .AsReadOnly();
        }
    }
}
