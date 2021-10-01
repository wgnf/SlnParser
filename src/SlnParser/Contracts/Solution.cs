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
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public FileInfo File { get; set; }

        /// <inheritdoc />
        public string FileFormatVersion { get; set; }

        /// <inheritdoc />
        public VisualStudioVersion VisualStudioVersion { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IProject> AllProjects { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IProject> Projects { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationPlatform> ConfigurationPlatforms { get; internal set; }
    }
}
