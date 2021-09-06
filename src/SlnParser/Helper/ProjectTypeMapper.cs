using SlnParser.Contracts;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;

namespace SlnParser.Helper
{
	public class ProjectTypeMapper : IProjectTypeMapper
	{
		private readonly IDictionary<Guid, ProjectType> _mapping;

		public ProjectTypeMapper()
		{
			_mapping = GetMapping();
		}

		public ProjectType Map(Guid typeGuid)
		{
			return _mapping.ContainsKey(typeGuid)
				? _mapping[typeGuid]
				: ProjectType.Unknown;
		}

		private static IDictionary<Guid, ProjectType> GetMapping()
		{
			return new Dictionary<Guid, ProjectType>
			{
				{new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8"), ProjectType.SolutionFolder},
				{new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"), ProjectType.CSharpClassLibrary},
				{new Guid("9A19103F-16F7-4668-BE54-9A1E7A4F7556"), ProjectType.CSharpClassLibrary}
			};
		}
	}
}