using SlnParser.Contracts;
using SlnParser.Contracts.Exceptions;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SlnParser.Helper
{
    internal sealed class SlnxParser
    {
        private readonly IProjectTypeMapper _projectTypeMapper = new  ProjectTypeMapper();
        
        public ISolution Parse(FileInfo solutionFile)
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

            solution.AllProjects = structuredProjects.AsReadOnly();
            solution.Projects = structuredProjects.AsReadOnly();
            solution.ConfigurationPlatforms = ParseSolutionConfigurations(root).AsReadOnly();
            
            return solution;
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
                if (childElement.Name == "File")
                {
                    var solutionFolderFile = ParseSolutionFolderFile(childElement, solutionFile);
                    if (solutionFolderFile == null)
                    {
                        throw new UnexpectedSolutionStructureException($"Could not parse file of solution folder '{actualFolderName}' in solution file '{solutionFile.FullName}'");
                    }

                    solutionFolder.AddFile(solutionFolderFile);
                }
                
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

            var projectFullPath = EnsureAbsolute(projectPath, solutionFile);

            // the project name is part of its path.
            var actualProjectName = Path.GetFileNameWithoutExtension(projectFullPath);

            // elements in the SLNX file don't have any IDs anymore, so it's omitted.
            var solutionProject = new SolutionProject(Guid.Empty, actualProjectName, projectTypeId, projectType, new FileInfo(projectFullPath));
            
            var projectConfigurations = ParseProjectConfigurations(xmlElement);
            foreach (var configuration in projectConfigurations) solutionProject.AddConfigurationPlatform(configuration);

            return new List<IProject> { solutionProject };
        }

        private static List<ConfigurationPlatform> ParseSolutionConfigurations(XmlElement rootElement)
        {
            var configurations = new List<ConfigurationPlatform>();
            var configurationPlatforms = rootElement.SelectNodes("Configurations/Platform");
            if (configurationPlatforms == null)
            {
                return configurations;
            }
            
            foreach (XmlNode configurationPlatform in configurationPlatforms)
            {
                if (configurationPlatform.Attributes == null) continue;
                
                var name = configurationPlatform.Attributes["Name"].Value;
                // there is not much info anymore
                var platform = new ConfigurationPlatform(name, string.Empty, name);
                configurations.Add(platform);
            }
            
            return configurations;
        }

        private static List<ConfigurationPlatform> ParseProjectConfigurations(XmlElement projectElement)
        {
            var configurations = new List<ConfigurationPlatform>();
            var configurationPlatforms = projectElement.SelectNodes("Platform");
            if (configurationPlatforms == null)
            {
                return configurations;
            }

            foreach (XmlNode configurationPlatform in configurationPlatforms)
            {
                if (configurationPlatform.Attributes == null) continue;
                
                var projectName = configurationPlatform.Attributes["Project"].Value;
                // there is not much info anymore
                var platform = new ConfigurationPlatform(projectName, string.Empty, projectName);
                configurations.Add(platform);
            }

            return configurations;
        }

        private static FileInfo? ParseSolutionFolderFile(XmlElement xmlElement, FileInfo solutionFile)
        {
            var path = xmlElement.GetAttribute("Path");
            if (string.IsNullOrWhiteSpace(path)) return null;
            
            var fullPath = EnsureAbsolute(path, solutionFile);
            var fullPathInfo = new FileInfo(fullPath);
            return fullPathInfo;
        }

        private static string EnsureAbsolute(string path, FileInfo solutionFile)
        {
            var filePath = new FileInfo(path);
            var isRelative = Path.IsPathRooted(filePath.FullName);
            var fileFullPath = isRelative
                ? Path.Combine(solutionFile.Directory?.FullName ?? string.Empty, filePath.FullName)
                : filePath.FullName;
            return fileFullPath;
        }
    }
}
