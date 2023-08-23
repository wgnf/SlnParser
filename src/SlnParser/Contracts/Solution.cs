using System;
using System.Collections.Generic;
using System.IO;

namespace SlnParser.Contracts
{
    /// <inheritdoc />
    public class Solution : ISolution
    {
        /// <summary>
        ///     Creates a new instance of <see cref="Solution" />
        /// </summary>
        public Solution()
        {
            AllProjects = new List<IProject>();
            Projects = new List<IProject>();
            ConfigurationPlatforms = new List<ConfigurationPlatform>();
        }

        /// <inheritdoc />
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc />
        public FileInfo? File { get; set; }

        /// <inheritdoc />
        public string FileFormatVersion { get; set; } = string.Empty;

        /// <inheritdoc />
        public VisualStudioVersion VisualStudioVersion { get; set; } = new VisualStudioVersion();

        /// <inheritdoc />
        public IReadOnlyCollection<IProject> AllProjects { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IProject> Projects { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationPlatform> ConfigurationPlatforms { get; internal set; }

        /// <inheritdoc/>
        public Guid? Guid { get; internal set; }
    }
}
