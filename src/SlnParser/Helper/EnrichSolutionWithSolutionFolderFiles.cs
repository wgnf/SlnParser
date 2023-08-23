using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlnParser.Helper
{
    internal sealed class EnrichSolutionWithSolutionFolderFiles : IEnrichSolution
    {
        private readonly IParseProjectDefinition _parseProjectDefinition = new ProjectDefinitionParser();
        private bool _inASolutionItemsSection;

        private SolutionFolder? _solutionFolderForCurrentSection;

        /*
         * line block:
         * Project("...
         * ProjectSection(SolutionItems) = preProject
         * file1/file1 \
         * file2/file2  }-- we want these
         * file3/file3 /
         * EndProjectSection
         */
        public void Enrich(Solution solution, IEnumerable<string> fileContents)
        {
            if (solution == null) throw new ArgumentNullException(nameof(solution));
            if (fileContents == null) throw new ArgumentNullException(nameof(fileContents));

            foreach (var line in fileContents)
                ProcessLine(solution, line);
        }

        private void ProcessLine(Solution solution, string line)
        {
            if (_solutionFolderForCurrentSection == null)
            {
                // if the project-definition could be parsed we can assume we are in a "Project" --> "EndProject" block
                TryGetSolutionFolder(solution, line, out _solutionFolderForCurrentSection);
                return;
            }

            DetermineEndProject(line);
            AddSolutionItemFile(solution, line);
            DetermineProjectItemsSection(line);
        }

        private void TryGetSolutionFolder(
            Solution solution,
            string line,
            out SolutionFolder? solutionFolder)
        {
            solutionFolder = null;
            if (!_parseProjectDefinition.TryParseProjectDefinition(solution, line, out var project))
                return;

            if (!(project is SolutionFolder slnFolder))
                return;

            var actualSolutionFolder = solution
                .AllProjects
                .OfType<SolutionFolder>()
                .FirstOrDefault(folder => folder.Id == slnFolder.Id);
            if (actualSolutionFolder == null) return;

            _inASolutionItemsSection = false;
            solutionFolder = actualSolutionFolder;
        }

        private void DetermineEndProject(string line)
        {
            if (!line.StartsWith("EndProject")) return;

            _solutionFolderForCurrentSection = null;
            _inASolutionItemsSection = false;
        }

        private void DetermineProjectItemsSection(string line)
        {
            if (_inASolutionItemsSection) return;

            _inASolutionItemsSection = line.StartsWith("ProjectSection(SolutionItems)");
        }

        private void AddSolutionItemFile(ISolution solution, string line)
        {
            if (!_inASolutionItemsSection) return;

            if (!TryGetSolutionItemFile(solution, line, out var solutionItemFile) || solutionItemFile == null)
                return;

            _solutionFolderForCurrentSection?.AddFile(solutionItemFile);
        }

        private static bool TryGetSolutionItemFile(
            ISolution solution,
            string line,
            out FileInfo? solutionItemFile)
        {
            solutionItemFile = null;

            var solutionItem = line.Split('=').FirstOrDefault();
            if (solutionItem == null) return false;

            solutionItem = solutionItem.Trim();

            var solutionDirectory = Path.GetDirectoryName(solution.File?.FullName);
            if (solutionDirectory == null)
                throw new UnexpectedSolutionStructureException("Solution-Directory could not be determined");

            var solutionItemCombined = Path.Combine(solutionDirectory, solutionItem);
            solutionItemFile = new FileInfo(solutionItemCombined);
            return true;
        }
    }
}
