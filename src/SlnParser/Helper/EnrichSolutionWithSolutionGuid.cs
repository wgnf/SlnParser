using SlnParser.Contracts;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlnParser.Helper
{
    internal sealed class EnrichSolutionWithSolutionGuid : IEnrichSolution
    {
        private readonly ISectionParser _sectionParser = new SectionParser();

        public void Enrich(Solution solution, IEnumerable<string> fileContents)
        {
            var extensibilityGlobals = _sectionParser.GetFileContentsInGlobalSection(
                fileContents,
                "ExtensibilityGlobals");

            solution.Guid = extensibilityGlobals
                .Select(ExtractSolutionGuid)
                .FirstOrDefault(x => x.HasValue);
        }

        private Guid? ExtractSolutionGuid(string line)
        {
            const string pattern = @"\s*SolutionGuid\s*=\s*{([A-Fa-f0-9\-]+)}";
            var match = Regex.Match(line, pattern);
            if (!match.Success)
            {
                return null;
            }

            var guidString = match.Groups[1].Value;
            return new Guid(guidString);
        }
    }
}
