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
        public void Enrich(Solution solution, IEnumerable<string> fileContents)
        {
            var extensibilityGlobals = SectionParser.GetFileContentsInGlobalSection(
                fileContents,
                "ExtensibilityGlobals");

            IEnumerable<Guid?> solutionGuids = extensibilityGlobals.Select(ExtractSolutionGuid).Where(x => x.HasValue);
            if (solutionGuids.Any())
            {
                solution.Guid = solutionGuids.Single();
            }
        }

        private Guid? ExtractSolutionGuid(string line)
        {
            const string pattern = @"\s*SolutionGuid\s*=\s*{([A-Fa-f0-9\-]+)}";
            var match = Regex.Match(line, pattern);
            if (!match.Success)
            {
                return null;
            }

            string guidString = match.Groups[1].Value;
            return new Guid(guidString);
        }
    }
}
