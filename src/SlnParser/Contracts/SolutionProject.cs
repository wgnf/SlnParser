using System;
using System.IO;

namespace SlnParser.Contracts
{
	/// <summary>
	///		A Solution Project that can be contained in a <see cref="Solution"/>
	/// </summary>
	public class SolutionProject : IProject
    {
		/// <summary>
		///		Creates a new instance of <see cref="SolutionProject"/>
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="name">The name</param>
		/// <param name="projectTypeGuid">The project-type id</param>
		/// <param name="projectType">The well-known project-type</param>
		public SolutionProject(
			Guid id,
			string name,
			Guid projectTypeGuid,
			ProjectType projectType,
			FileInfo fileInfo)
		{
			Id = id;
			Name = name;
			ProjectTypeGuid = projectTypeGuid;
			ProjectType = projectType;
			File = fileInfo;
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
		///		The File of the Project
		/// </summary>
		public FileInfo File { get; }
    }
}
