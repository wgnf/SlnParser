using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using SlnParser.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlnParser
{
	/// <inheritdoc/>
    public sealed class SolutionParser : ISolutionParser
    {
        private readonly IEnumerable<IEnrichSolution> _solutionEnrichers;

	    /// <summary>
	    ///		Creates a new instance of <see cref="SolutionParser"/>
	    /// </summary>
	    public SolutionParser()
        {
            _solutionEnrichers = new List<IEnrichSolution>
            {
                new EnrichSolutionWithProjects(), 
                new EnrichSolutionWithSolutionConfigurationPlatforms(),
                /*
                 * NOTE: It's important that this happens _after_ the 'EnrichSolutionWithProjects',
                 * because we need the parsed projects before we can map the configurations to them
                 */
                new EnrichSolutionWithProjectConfigurationPlatforms()
            };
        }
	    
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
            var allLinesTrimmed = allLines
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();
            
            foreach (var enricher in _solutionEnrichers)
                enricher.Enrich(solution, allLinesTrimmed);

            foreach (var line in allLines)
                ProcessLine(line, solution);

            return solution;
        }

        private static void ProcessLine(string line, Solution solution)
        {
			ProcessSolutionFileFormatVersion(line, solution);
			ProcessVisualStudioVersion(line, solution);
			ProcessMinimumVisualStudioVersion(line, solution);
        }

		private static void ProcessSolutionFileFormatVersion(string line, Solution solution)
		{
			if (!line.StartsWith("Microsoft Visual Studio Solution File, ")) return;

			/*
			 * 54 characters, because...
			 * "Microsoft Visual Studio Solution File, Format Version " is 54 characters long
			*/
			var fileFormatVersion = string.Concat(line.Skip(54));
			solution.FileFormatVersion = fileFormatVersion;
		}

		private static void ProcessVisualStudioVersion(string line, Solution solution)
		{
			if (!line.StartsWith("VisualStudioVersion = ")) return;

			// because "VisualStudioVersion = " is 22 characters long
			var visualStudioVersion = string.Concat(line.Skip(22));

			solution.VisualStudioVersion ??= new VisualStudioVersion();
			solution.VisualStudioVersion.Version = visualStudioVersion;
		}

		private static void ProcessMinimumVisualStudioVersion(string line, Solution solution)
		{
			if (!line.StartsWith("MinimumVisualStudioVersion = ")) return;

			// because "MinimumVisualStudioVersion = " is 29 characters long
			var minimumVisualStudioVersion = string.Concat(line.Skip(29));

			solution.VisualStudioVersion ??= new VisualStudioVersion();
			solution.VisualStudioVersion.MinimumVersion = minimumVisualStudioVersion;
		}
    }
}
