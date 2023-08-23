using System;
using System.Collections.Generic;
using System.IO;

namespace SlnParser.Contracts
{
    /// <summary>
    ///     An interface representing all the information contained in a Visual Studio Solution File (sln)
    /// </summary>
    public interface ISolution
    {
        /// <summary>
        ///     The name of the solution
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The File of the solution
        /// </summary>
        FileInfo? File { get; set; }

        /// <summary>
        ///     The file format version of the solution
        /// </summary>
        string FileFormatVersion { get; set; }

        /// <summary>
        ///     The <see cref="VisualStudioVersion" /> of the solution
        /// </summary>
        VisualStudioVersion VisualStudioVersion { get; set; }

        /// <summary>
        ///     A flat list of all <see cref="IProject" />s contained in the solution
        /// </summary>
        IReadOnlyCollection<IProject> AllProjects { get; }

        /// <summary>
        ///     A structured list of all <see cref="IProject" />s contained in the solution
        /// </summary>
        IReadOnlyCollection<IProject> Projects { get; }

        /// <summary>
        ///     The <see cref="ConfigurationPlatform" />s configured for this solution
        /// </summary>
        IReadOnlyCollection<ConfigurationPlatform> ConfigurationPlatforms { get; }

        /// <summary>
        /// The <see cref="Guid"/> of the solution.
        /// </summary>
        Guid? Guid { get; }
    }
}
