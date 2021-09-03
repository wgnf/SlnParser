using SlnParser.Contracts;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlnParser
{
	/// <inheritdoc/>
    public class SolutionParser : ISolutionParser
    {
		/// <inheritdoc/>
		public Solution Parse(string solutionFileName)
        {
            if (string.IsNullOrWhiteSpace(solutionFileName))
                throw new ArgumentException($"'{nameof(solutionFileName)}' cannot be null or whitespace.", nameof(solutionFileName));

            var solutionFile = new FileInfo(solutionFileName);
            return Parse(solutionFile);
        }

		/// <inheritdoc/>
		public Solution Parse(FileInfo solutionFile)
        {
            if (solutionFile is null)
                throw new ArgumentNullException(nameof(solutionFile));
            if (!solutionFile.Exists)
                throw new FileNotFoundException("Provided Solution-File does not exist", solutionFile.FullName);
			if (!solutionFile.Extension.Equals(".sln"))
				throw new InvalidDataException("The provided file is not a solution file!");

            try
            {
                var solution = ParseInternal(solutionFile);
                return solution;
            }
            catch (Exception exception)
            {
                throw new ParseSolutionFailedException(solutionFile, exception);
            }
        }

		/// <inheritdoc/>
		public bool TryParse(string solutionFileName, out Solution solution)
        {
            if (string.IsNullOrWhiteSpace(solutionFileName))
                throw new ArgumentException($"'{nameof(solutionFileName)}' cannot be null or whitespace.", nameof(solutionFileName));

            var solutionFile = new FileInfo(solutionFileName);
            return TryParse(solutionFile, out solution);
        }

		/// <inheritdoc/>
		public bool TryParse(FileInfo solutionFile, out Solution solution)
        {
            if (solutionFile is null)
                throw new ArgumentNullException(nameof(solutionFile));
            solution = null;

            try
            {
                solution = Parse(solutionFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Solution ParseInternal(FileInfo solutionFile)
        {
            var solution = new Solution
            {
                Name = Path.GetFileNameWithoutExtension(solutionFile.FullName),
                File = solutionFile
            };
            var allLines = File.ReadAllLines(solutionFile.FullName);

            foreach (var line in allLines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Length == 0) continue;

                ProcessLine(line, solution);
            }

            return solution;
        }

        private void ProcessLine(string line, Solution solution)
        {
			ProcessSolutionFileFormatVersion(line, solution);
			ProcessVisualStudioVersion(line, solution);
			ProcessMinimumVisualStudioVersion(line, solution);
            ProcessProject(line, solution);
        }

		private void ProcessSolutionFileFormatVersion(string line, Solution solution)
		{
			if (!line.StartsWith("Microsoft Visual Studio Solution File, ")) return;

			/*
			 * 54 characters, because...
			 * "Microsoft Visual Studio Solution File, Format Version " is 54 characters long
			*/
			var fileFormatVersion = string.Concat(line.Skip(54));
			solution.FileFormatVersion = fileFormatVersion;
		}

		private void ProcessVisualStudioVersion(string line, Solution solution)
		{
			if (!line.StartsWith("VisualStudioVersion = ")) return;

			// because "VisualStudioVersion = " is 22 characters long
			var visualStudioVersion = string.Concat(line.Skip(22));

			solution.VisualStudioVersion ??= new VisualStudioVersion();
			solution.VisualStudioVersion.Version = visualStudioVersion;
		}

		private void ProcessMinimumVisualStudioVersion(string line, Solution solution)
		{
			if (!line.StartsWith("MinimumVisualStudioVersion = ")) return;

			// because "MinimumVisualStudioVersion = " is 29 characters long
			var minimumVisualStudioVersion = string.Concat(line.Skip(29));

			solution.VisualStudioVersion ??= new VisualStudioVersion();
			solution.VisualStudioVersion.MinimumVersion = minimumVisualStudioVersion;
		}

        private void ProcessProject(string line, Solution solution)
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

			solution.Projects.Add(project);
        }
    }
}
