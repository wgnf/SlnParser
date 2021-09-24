using System;

namespace SlnParser.Contracts.Helper
{
    internal interface IProjectTypeMapper
    {
        ProjectType Map(Guid typeGuid);
    }
}
