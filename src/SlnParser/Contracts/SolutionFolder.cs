using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SlnParser.Contracts
{
	/// <summary>
	///		A Solution Folder that can be contained in a <see cref="Solution"/>
	/// </summary>
	public class SolutionFolder : IProject
	{
		/// <summary>
		///		Creates a new instance of <see cref="SolutionFolder"/>
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="name">The name</param>
		/// <param name="projectTypeGuid">The project-type id</param>
		/// <param name="projectType">The well-known project-type</param>
		/// <param name="projects">The contained <see cref="IProject"/>s</param>
		public SolutionFolder(
			Guid id,
			string name,
			Guid projectTypeGuid,
			ProjectType projectType,
			IList<IProject> projects)
		{
			Id = id;
			Name = name;
			ProjectTypeGuid = projectTypeGuid;
			ProjectType = projectType;
			Projects = new ReadOnlyCollection<IProject>(projects);
		}

		/// <inheritdoc/>
		public Guid Id { get; }

		/// <inheritdoc/>
		public string Name { get; }

		/// <inheritdoc/>
		public Guid ProjectTypeGuid { get; }

		/// <inheritdoc/>
		public ProjectType ProjectType { get; }

		/// <summary>
		///		The contained <see cref="IProject"/>s in the Solution Folder
		/// </summary>
		public IReadOnlyCollection<IProject> Projects { get; }
	}
}
