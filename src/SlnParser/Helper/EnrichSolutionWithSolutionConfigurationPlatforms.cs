using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlnParser.Helper
{
    internal sealed class EnrichSolutionWithSolutionConfigurationPlatforms : IEnrichSolution
    {
        public void Enrich(Solution solution, IEnumerable<string> fileContents)
        {
            var configurationSection = GetSolutionConfigurationPlatformsSection(fileContents);
            var parsedConfigurations = ParseConfigurationPlatforms(configurationSection);
            solution.ConfigurationPlatforms = parsedConfigurations.ToList().AsReadOnly();
        }

        private static IEnumerable<string> GetSolutionConfigurationPlatformsSection(IEnumerable<string> fileContents)
        {
            const string startSolutionConfigurationPlatforms = "GlobalSection(SolutionConfiguration";
            const string endSolutionConfigurationPlatforms = "EndGlobalSection";

            var section = fileContents
                .SkipWhile(line => !line.StartsWith(startSolutionConfigurationPlatforms))
                .TakeWhile(line => !line.StartsWith(endSolutionConfigurationPlatforms))
                .Where(line => !line.StartsWith(startSolutionConfigurationPlatforms))
                .Where(line => !line.StartsWith(endSolutionConfigurationPlatforms))
                .Where(line => !string.IsNullOrWhiteSpace(line));

            return section.ToList();
        }

        private static IEnumerable<ConfigurationPlatform> ParseConfigurationPlatforms(
            IEnumerable<string> configurationSection)
        {
            var configurations = configurationSection
                .Select(ParseConfigurationPlatform);

            return configurations;
        }

        private static ConfigurationPlatform ParseConfigurationPlatform(string line)
        {
            // c.f.: https://regexr.com/65rmo
            const string pattern = @"(?<name>.+) = (?<buildConfiguration>.+)\|(?<buildPlatform>.+)";
            var match = Regex.Match(line, pattern);
            if (!match.Success)
                throw new UnexpectedSolutionStructureException(
                    "Expected to find ConfigurationPlatform but pattern did not match");

            var configurationName = match.Groups["name"].Value;
            var buildConfigurationString = match.Groups["buildConfiguration"].Value;
            var buildPlatformString = match.Groups["buildPlatform"].Value;

            var buildConfiguration = ParseBuildConfiguration(buildConfigurationString);
            var buildPlatform = ParseBuildPlatform(buildPlatformString);

            var configurationPlatform = new ConfigurationPlatform(
                configurationName,
                buildConfiguration,
                buildPlatform);
            return configurationPlatform;
        }

        private static BuildConfiguration ParseBuildConfiguration(string buildConfigurationString)
        {
            return buildConfigurationString switch
            {
                "Debug" => BuildConfiguration.Debug,
                "Release" => BuildConfiguration.Release,
                _ => throw new UnexpectedSolutionStructureException(
                    $"{buildConfigurationString} is not recognized as a possible value for {nameof(BuildConfiguration)}")
            };
        }

        private static BuildPlatform ParseBuildPlatform(string buildPlatformString)
        {
            return buildPlatformString switch
            {
                "Any CPU" => BuildPlatform.AnyCpu,
                "x64" => BuildPlatform.X64,
                "x86" => BuildPlatform.X86,
                _ => throw new UnexpectedSolutionStructureException(
                    $"{buildPlatformString} is not recognized as a possible value for {nameof(BuildPlatform)}")
            };
        }
    }
}
