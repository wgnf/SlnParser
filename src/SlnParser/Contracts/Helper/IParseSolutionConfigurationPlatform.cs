using System.Collections.Generic;

namespace SlnParser.Contracts.Helper
{
    internal interface IParseSolutionConfigurationPlatform
    {
        IEnumerable<ProjectConfigurationPlatform> Parse(
            IEnumerable<string> fileContents,
            string sectionName);
    }
}
