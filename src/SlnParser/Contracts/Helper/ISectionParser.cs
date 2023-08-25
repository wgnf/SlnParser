using System.Collections.Generic;

namespace SlnParser.Contracts.Helper
{
    internal interface ISectionParser
    {
        IEnumerable<string> GetFileContentsInGlobalSection(
            IEnumerable<string> fileContents,
            string sectionName);
    }
}
