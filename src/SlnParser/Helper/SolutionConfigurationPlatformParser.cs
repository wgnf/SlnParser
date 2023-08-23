using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlnParser.Helper
{
    internal sealed class SolutionConfigurationPlatformParser : IParseSolutionConfigurationPlatform
    {
        public IEnumerable<ProjectConfigurationPlatform> Parse(
            IEnumerable<string> fileContents,
            string startSection)
        {
            if (fileContents == null) throw new ArgumentNullException(nameof(fileContents));
            if (string.IsNullOrWhiteSpace(startSection))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(startSection));

            var sectionContents = GetFileContentsInSection(fileContents, startSection);
            var projectConfigurationPlatforms = ParseConfigurationPlatforms(sectionContents);
            return projectConfigurationPlatforms;
        }

        private static IEnumerable<string> GetFileContentsInSection(
            IEnumerable<string> fileContents,
            string startSection)
        {
            const string endSection = "EndGlobalSection";

            var section = fileContents
                .SkipWhile(line => !line.StartsWith(startSection))
                .TakeWhile(line => !line.StartsWith(endSection))
                .Where(line => !line.StartsWith(startSection))
                .Where(line => !line.StartsWith(endSection))
                .Where(line => !string.IsNullOrWhiteSpace(line));

            return section.ToList();
        }

        private static IEnumerable<ProjectConfigurationPlatform> ParseConfigurationPlatforms(
            IEnumerable<string> sectionFileContents)
        {
            var configurations = sectionFileContents
                .Select(ParseConfigurationPlatform);

            return configurations;
        }

        private static ProjectConfigurationPlatform ParseConfigurationPlatform(string line)
        {
            // c.f.: https://regexr.com/65t6u
            const string pattern =
                @"((?<projectId>\{[A-Za-z0-9\-]+\}).)?(?<name>.+) = (?<buildConfiguration>.+)\|(?<buildPlatform>.+)";

            var match = Regex.Match(line, pattern);
            if (!match.Success)
                throw new UnexpectedSolutionStructureException(
                    "Expected to find ConfigurationPlatform but pattern did not match");

            var configurationProjectId = ParseProjectIdFromMatch(match);
            var configurationPlatform = ParseConfigurationPlatformFromMatch(match);

            return new ProjectConfigurationPlatform(configurationProjectId, configurationPlatform);
        }

        private static Guid? ParseProjectIdFromMatch(Match match)
        {
            var configurationProjectIdString = match.Groups["projectId"].Value;
            if (string.IsNullOrWhiteSpace(configurationProjectIdString))
                return null;

            var configurationProjectId = Guid.Parse(configurationProjectIdString);
            return configurationProjectId;
        }

        private static ConfigurationPlatform ParseConfigurationPlatformFromMatch(Match match)
        {
            var configurationName = match.Groups["name"].Value;
            var buildConfiguration = match.Groups["buildConfiguration"].Value;
            var buildPlatform = match.Groups["buildPlatform"].Value;

            var configurationPlatform = new ConfigurationPlatform(
                configurationName,
                buildConfiguration,
                buildPlatform);
            return configurationPlatform;
        }
    }
}
