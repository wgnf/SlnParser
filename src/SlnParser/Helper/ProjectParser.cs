using SlnParser.Contracts;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SlnParser.Helper
{
	internal class ProjectParser : IProjectParser
	{
		public void Enrich(Solution solution, IEnumerable<string> fileContents)
		{
			if (solution == null) throw new ArgumentNullException(nameof(solution));
			if (fileContents == null) throw new ArgumentNullException(nameof(fileContents));

			var flatProjectList = new List<IProject>();
			foreach (var line in fileContents)
				ProcessLine(line, flatProjectList);

			solution.Projects = flatProjectList;
		}
		private static void ProcessLine(string line, IList<IProject> flatProjectList)
		{
			if (!line.StartsWith("Project(\"{")) return;

			// c.f.: regexr.com/650df
			const string pattern = @"Project\(""\{(?<projectTypeGuid>[A-Za-z0-9\-]+)\}""\) = ""(?<projectName>.+)"", ""(?<projectPath>.+)"", ""\{(?<projectGuid>[A-Za-z0-9\-]+)\}";
			var match = Regex.Match(line, pattern);
			if (!match.Success) return;

			var projectTypeGuidString = match.Groups["projectTypeGuid"].Value;
			var projectName = match.Groups["projectName"].Value;
			var projectPath = match.Groups["projectPath"].Value;
			var projectGuidString = match.Groups["projectGuid"].Value;

			var projectTypeGuid = Guid.Parse(projectTypeGuidString);
			var projectGuid = Guid.Parse(projectGuidString);
			var projectFile = new FileInfo(projectPath);

			var project = new SolutionProject(
				projectGuid,
				projectName,
				projectTypeGuid,
				ProjectType.Unknown,
				projectFile);

			flatProjectList.Add(project);
		}
	}
}