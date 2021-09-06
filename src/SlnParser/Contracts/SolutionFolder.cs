using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SlnParser.Contracts
{
	/// <summary>
	///		A Solution Folder that can be contained in a <see cref="Solution"/>
	/// </summary>
	public class SolutionFolder : IProject
	{
		private readonly ICollection<IProject> _projects = new Collection<IProject>();
		
		/// <summary>
		///		Creates a new instance of <see cref="SolutionFolder"/>
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="name">The name</param>
		/// <param name="typeGuid">The project-type id</param>
		/// <param name="type">The well-known project-type</param>
		public SolutionFolder(
			Guid id,
			string name,
			Guid typeGuid,
			ProjectType type)
		{
			Id = id;
			Name = name;
			TypeGuid = typeGuid;
			Type = type;
		}

		/// <inheritdoc/>
		public Guid Id { get; }

		/// <inheritdoc/>
		public string Name { get; }

		/// <inheritdoc/>
		public Guid TypeGuid { get; }

		/// <inheritdoc/>
		public ProjectType Type { get; }

		/// <summary>
		///		The contained <see cref="IProject"/>s in the Solution Folder
		/// </summary>
		public IReadOnlyCollection<IProject> Projects => _projects.ToList().AsReadOnly();

		internal void AddProject(IProject project)
		{
			_projects.Add(project);
		}
	}
}
