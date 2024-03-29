﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SlnParser.Contracts
{
    /// <summary>
    ///     A Solution Project that can be contained in a <see cref="Solution" />
    /// </summary>
    public class SolutionProject : IProject
    {
        private readonly ICollection<ConfigurationPlatform> _configurationPlatforms =
            new Collection<ConfigurationPlatform>();

        /// <summary>
        ///     Creates a new instance of <see cref="SolutionProject" />
        /// </summary>
        /// <param name="id">The id</param>
        /// <param name="name">The name</param>
        /// <param name="typeGuid">The project-type id</param>
        /// <param name="type">The well-known project-type</param>
        /// <param name="fileInfo">The <see cref="FileInfo" /> for the Project-File</param>
        public SolutionProject(
            Guid id,
            string name,
            Guid typeGuid,
            ProjectType type,
            FileInfo fileInfo)
        {
            Id = id;
            Name = name;
            TypeGuid = typeGuid;
            Type = type;
            File = fileInfo;
        }

        /// <summary>
        ///     The File of the Project
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        ///     The <see cref="ConfigurationPlatform" />s configured for this solution
        /// </summary>
        public IReadOnlyCollection<ConfigurationPlatform> ConfigurationPlatforms =>
            _configurationPlatforms.ToList().AsReadOnly();

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Guid TypeGuid { get; }

        /// <inheritdoc />
        public ProjectType Type { get; }

        internal void AddConfigurationPlatform(ConfigurationPlatform configurationPlatform)
        {
            if (configurationPlatform == null) throw new ArgumentNullException(nameof(configurationPlatform));

            _configurationPlatforms.Add(configurationPlatform);
        }
    }
}
