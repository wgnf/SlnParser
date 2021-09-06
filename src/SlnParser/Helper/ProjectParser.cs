using SlnParser.Contracts;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlnParser.Helper
{
	internal class ProjectParser : IProjectParser
	{
		private readonly IProjectTypeMapper _projectTypeMapper;
		
		public ProjectParser()
		{
			_projectTypeMapper = new ProjectTypeMapper();
		}
		
		public void Enrich(Solution solution, IEnumerable<string> fileContents)
		{
			if (solution == null) throw new ArgumentNullException(nameof(solution));
			if (fileContents == null) throw new ArgumentNullException(nameof(fileContents));

			var flatProjectList = GetProjectsFlat(fileContents);
			solution.Projects = flatProjectList.ToList().AsReadOnly();
		}

		private IEnumerable<IProject> GetProjectsFlat(IEnumerable<string> fileContents)
		{
			var flatProjectList = new Collection<IProject>();
			foreach (var line in fileContents)
				ProcessLine(line, flatProjectList);
			
			return flatProjectList;
		}

		private void ProcessLine(string line, ICollection<IProject> flatProjectList)
		{
			if (!line.StartsWith("Project(\"{")) return;

			// c.f.: https://regexr.com/650df
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

			var projectType = _projectTypeMapper.Map(projectTypeGuid);

			IProject project;
			if (projectType == ProjectType.SolutionFolder)
				project = new SolutionFolder(
					projectGuid,
					projectName,
					projectTypeGuid,
					projectType);
			else
				project = new SolutionProject(
					projectGuid,
					projectName,
					projectTypeGuid,
					projectType,
					projectFile);

			flatProjectList.Add(project);
		}
	}
}