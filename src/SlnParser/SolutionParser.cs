using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using SlnParser.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SlnParser
{
    /// <inheritdoc />
    public sealed class SolutionParser : ISolutionParser
    {
        private readonly IEnumerable<IEnrichSolution> _solutionEnrichers;
        private readonly IProjectTypeMapper _projectTypeMapper = new  ProjectTypeMapper();

        /// <summary>
        ///     Creates a new instance of <see cref="SolutionParser" />
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
                new EnrichSolutionWithProjectConfigurationPlatforms(),
                new EnrichSolutionWithSolutionFolderFiles(),
                new EnrichSolutionWithSolutionGuid(),
            };
        }

        /// <inheritdoc />
        public ISolution Parse(string solutionFileName)
        {
            if (string.IsNullOrWhiteSpace(solutionFileName))
                throw new ArgumentException($"'{nameof(solutionFileName)}' cannot be null or whitespace.",
                    nameof(solutionFileName));

            var solutionFile = new FileInfo(solutionFileName);
            return Parse(solutionFile);
        }

        /// <inheritdoc />
        public ISolution Parse(FileInfo solutionFile)
        {
            if (solutionFile is null)
                throw new ArgumentNullException(nameof(solutionFile));
            if (!solutionFile.Exists)
                throw new FileNotFoundException("Provided Solution-File does not exist", solutionFile.FullName);

            var fileExtension = solutionFile.Extension;

            try
            {
                var solution = fileExtension switch
                {
                    ".sln" => ParseSlnInternal(solutionFile),
                    ".slnx" => ParseSlnxInternal(solutionFile),
                    _ => throw new InvalidDataException($"The provided file '{solutionFile.FullName}' is not a solution file!"),
                };
                return solution;
            }
            catch (Exception exception)
            {
                throw new ParseSolutionFailedException(solutionFile, exception);
            }
        }

        /// <inheritdoc />
        public bool TryParse(string solutionFileName, out ISolution? solution)
        {
            if (string.IsNullOrWhiteSpace(solutionFileName))
                throw new ArgumentException($"'{nameof(solutionFileName)}' cannot be null or whitespace.",
                    nameof(solutionFileName));

            var solutionFile = new FileInfo(solutionFileName);
            return TryParse(solutionFile, out solution);
        }

        /// <inheritdoc />
        public bool TryParse(FileInfo solutionFile, out ISolution? solution)
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

        private ISolution ParseSlnInternal(FileInfo solutionFile)
        {
            var solution = new Solution
            {
                Name = Path.GetFileNameWithoutExtension(solutionFile.FullName), 
                File = solutionFile,
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

        private ISolution ParseSlnxInternal(FileInfo solutionFile)
        {
            // the new SLNX has no information about the file format version, VS version and minimal VS version, so it's omitted.
            var solution = new Solution
            {
                Name = Path.GetFileNameWithoutExtension(solutionFile.FullName), 
                File = solutionFile,
            };
            
            var fileContent = File.ReadAllText(solutionFile.FullName);
            if (string.IsNullOrEmpty(fileContent.Trim()))
            {
                return solution;
            }
            
            var xmlReaderSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, // Prevent XXE
                XmlResolver = null, // Disable external entity resolution
            };
            using var xmlContentStream = solutionFile.OpenRead();
            var xmlDocument = new XmlDocument();

            using var xmlReader = XmlReader.Create(xmlContentStream, xmlReaderSettings);
            xmlDocument.Load(xmlReader);
            
            var root = xmlDocument.DocumentElement;
            if (root == null) throw new UnexpectedSolutionStructureException($"Solution file '{solutionFile.FullName}' does not contain a root element");

            var structuredProjects = new List<IProject>();

            foreach (var xmlElement in root.ChildNodes.OfType<XmlElement>())
            {
                var projectsFromElement = ParseSlnxElement(xmlElement, solutionFile);
                structuredProjects.AddRange(projectsFromElement);
            }

            solution.AllProjects = structuredProjects;
            solution.Projects = structuredProjects;
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

            solution.VisualStudioVersion.Version = visualStudioVersion;
        }

        private static void ProcessMinimumVisualStudioVersion(string line, ISolution solution)
        {
            if (!line.StartsWith("MinimumVisualStudioVersion = ")) return;

            // because "MinimumVisualStudioVersion = " is 29 characters long
            var minimumVisualStudioVersion = string.Concat(line.Skip(29));

            solution.VisualStudioVersion.MinimumVersion = minimumVisualStudioVersion;
        }
        
        private List<IProject> ParseSlnxElement(XmlElement xmlElement, FileInfo solutionFile)
        {
            return xmlElement.Name switch
            {
                "Folder" => ParseSlnxFolder(xmlElement, solutionFile),
                "Project" => ParseSlnxProject(xmlElement, solutionFile),
                _ => new List<IProject>(),
            };
        }
        
        private List<IProject> ParseSlnxFolder(XmlElement xmlElement, FileInfo solutionFile)
        {
            var folderName = xmlElement.GetAttribute("Name");
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new UnexpectedSolutionStructureException($"Could not find solution folder name attribute in solution '{solutionFile.FullName}'");
            }

            var actualFolderName = folderName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();

            var solutionFolderTypeId = new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8");
            var solutionFolder = new SolutionFolder(Guid.Empty, actualFolderName, solutionFolderTypeId, ProjectType.SolutionFolder);
            var projects = new List<IProject>
            {
                solutionFolder,
            };

            foreach (var childElement in xmlElement.ChildNodes.OfType<XmlElement>())
            {
                var childProjects = ParseSlnxElement(childElement, solutionFile);
                foreach (var childProject in childProjects)
                {
                    solutionFolder.AddProject(childProject);
                    projects.Add(childProject);
                }
            }

            return projects;
        }

        private List<IProject> ParseSlnxProject(XmlElement xmlElement, FileInfo solutionFile)
        {
            const string defaultProjectTypeId = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

            var projectPath = xmlElement.GetAttribute("Path");
            if (string.IsNullOrWhiteSpace(projectPath))
            {
                throw new UnexpectedSolutionStructureException($"Could not find solution project path attribute in solution file '{solutionFile.FullName}'.");
            }

            var projectTypeIdText = xmlElement.GetAttribute("Type");
            projectTypeIdText = string.IsNullOrWhiteSpace(projectTypeIdText)
                ? defaultProjectTypeId
                // the new project type id notation does not quite fit the known format, so it is adjusted.
                : $"{{{projectTypeIdText.ToUpper()}}}";
            var projectTypeId = Guid.Parse(projectTypeIdText);
            var projectType = _projectTypeMapper.Map(projectTypeId);

            var projectFilePath = new FileInfo(projectPath);
            var isRelative = Path.IsPathRooted(projectFilePath.FullName);
            var projectFullPath = isRelative
                ? Path.Combine(solutionFile.Directory?.FullName ?? string.Empty, projectFilePath.FullName)
                : projectFilePath.FullName;

            // the project name is part of its path.
            var actualProjectName = Path.GetFileNameWithoutExtension(projectFullPath);

            // elements in the SLNX file don't have any IDs anymore, so it's omitted.
            var solutionProject = new SolutionProject(Guid.Empty, actualProjectName, projectTypeId, projectType, new FileInfo(projectFullPath));
            return new List<IProject> { solutionProject };
        }
    }
}
