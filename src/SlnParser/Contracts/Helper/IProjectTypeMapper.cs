using System;

namespace SlnParser.Contracts.Helper
{
	public interface IProjectTypeMapper
	{
		ProjectType Map(Guid typeGuid);
	}
}