using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SlnParser.Helper
{
    internal sealed class ProjectDefinitionParser : IParseProjectDefinition
    {
        private readonly IProjectTypeMapper _projectTypeMapper = new ProjectTypeMapper();

        public bool TryParseProjectDefinition(
            Solution solution,
            string projectDefinition,
            out IProject? project)
        {
            project = null;

            if (!projectDefinition.StartsWith("Project(\"{")) return false;

            // c.f.: https://regexr.com/650df
            const string pattern =
                @"Project\(""\{(?<projectTypeGuid>[A-Za-z0-9\-]+)\}""\) = ""(?<projectName>.+)"", ""(?<projectPath>.+)"", ""\{(?<projectGuid>[A-Za-z0-9\-]+)\}";
            var match = Regex.Match(projectDefinition, pattern);
            if (!match.Success) return false;

            var projectTypeGuidString = match.Groups["projectTypeGuid"].Value;
            var projectName = match.Groups["projectName"].Value;
            var projectPath = match.Groups["projectPath"].Value;
            var projectGuidString = match.Groups["projectGuid"].Value;

            var projectTypeGuid = Guid.Parse(projectTypeGuidString);
            var projectGuid = Guid.Parse(projectGuidString);

            var solutionDirectory = Path.GetDirectoryName(solution.File?.FullName);
            if (solutionDirectory == null)
                throw new UnexpectedSolutionStructureException("Solution-Directory could not be determined");

            // NOTE: the path to the project-file is usually separated using '\' - this does not work under linux
            projectPath = projectPath.Replace('/', Path.DirectorySeparatorChar);
            projectPath = projectPath.Replace('\\', Path.DirectorySeparatorChar);
            
            var projectFileCombinedWithSolution = Path.Combine(solutionDirectory, projectPath);
            var projectFile = new FileInfo(projectFileCombinedWithSolution);

            var projectType = _projectTypeMapper.Map(projectTypeGuid);

            project = projectType == ProjectType.SolutionFolder
                ? (IProject)new SolutionFolder(
                    projectGuid,
                    projectName,
                    projectTypeGuid,
                    projectType)
                : new SolutionProject(
                    projectGuid,
                    projectName,
                    projectTypeGuid,
                    projectType,
                    projectFile);

            return true;
        }
    }
}
